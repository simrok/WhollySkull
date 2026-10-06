using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 15f;

    public bool isRunning;
    public bool IsRunning => isRunning;

    [SerializeField] private float rotationSmooth = 15f;

    private Rigidbody rb;
    private Transform cam;  // 이동 방향의 기준이 되는 카메라

    private Vector3 dir;
    public Vector3 Dir => dir;

    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float maxSlopeAngle = 40f;     // 이 각도보다 가파르면 바닥이 아님
    public bool canMove;

    // 방향키 입력이 없으면 0, 뛰면 runSpeed, 걸으면 walkSpeed
    public float CurrentSpeed => (dir == Vector3.zero) ? 0 : ((isRunning == true) ? runSpeed : walkSpeed);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 방향키 입력을 카메라가 바라보는 방향(Y회전) 기준으로 돌림
        // 카메라 Y회전이 0이면(전투 맵) 기존과 완전히 같음
        float camYaw = cam != null ? cam.eulerAngles.y : 0f;
        dir = (Quaternion.Euler(0f, camYaw, 0f) * new Vector3(h, 0f, v)).normalized;

        // 좌쉬프트 => 달리기 true
        if (Input.GetKeyDown(KeyCode.LeftShift)) { isRunning = !isRunning; }
        // 움직임을 멈추면 달리기 false
        if (dir == Vector3.zero) { isRunning = false; }
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;  // 물리 충돌로 생긴 회전을 매번 지움
        
        // 공격 / 구르기 중이면 움직임 + 회전을 막음
        if (!canMove) return;

        // canMove 상태이면
        ApplyMove(dir, CurrentSpeed);

        // 플레이어 회전
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSmooth * Time.fixedDeltaTime);
            rb.MoveRotation(nextRotation);
        }

    }

    public void ForceMove(Vector3 direction, float speed)
    {
        ApplyMove(direction, speed);
    }

    private void ApplyMove(Vector3 moveDir, float speed)
    {
        // 1) 발밑 바닥 확인
        // Trigger 콜라이더(몬스터 감지 범위 등)는 무시
        bool hitGround = Physics.Raycast(rb.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.4f, ~0, QueryTriggerInteraction.Ignore);

        // 2) 바닥 면의 기울기(각도)가 maxSlopeAngle 이하일 때만 걸을 수 있는 바닥임
        // 바닥 면이 수평에서 몇 도 기울었는지 구함. 따라서 레이가 맞고 동시에 완만할 때 true
        bool isGrounded = hitGround && Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle;

        Vector3 velocity;
        if (isGrounded)
        {
            // 2) 바닥에 있으면: 이동 방향을 바닥 면에 맞게 눕혀서 그 방향으로 이동
            // -> 오르막은 위로, 내리막은 아래로, 경사로 끝에서는 바닥 normal이 (0,1,0)으로 바뀌면서 y 속도가 바로 0이 됨
            velocity = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized * speed;
        }
        else
        {
            // 3) 공중이면: 수평은 입력대로, y는 중력에 맡기되 더 세게 끌어내림
            // 떨어지는 속도는 살리고 올라가는 속도를 없앰
            velocity = moveDir * speed;
            float y = Mathf.Min(rb.linearVelocity.y, 0f);
            // 떨어질 때 중력 추가
            velocity.y = y + Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        rb.linearVelocity = velocity;   // 물리 엔진이 이동 + 충돌 처리
    }
}
