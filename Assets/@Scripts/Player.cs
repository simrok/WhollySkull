using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour, IDamageable
{
    [UnityEngine.Serialization.FormerlySerializedAs("context")] public PlayerData data = new PlayerData();
    private StateMachine stateMachine;

    // 상태 객체는 Awake에서 한 번 만들어두고 계속 재사용
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerRollState rollState;
    private PlayeyKickState kickState;
    private PlayerSliceState sliceState;
    private PlayerGetHitState getHitState;
    private PlayerDeadState deadState;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 0.7f);
    }

    private void Awake()
    {
        data.Init(GetComponent<Rigidbody>(), GetComponentInChildren<Animator>(), GetComponent<PlayerMovement>());

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        rollState = new PlayerRollState(this);
        kickState = new PlayeyKickState(this);
        sliceState = new PlayerSliceState(this);
        getHitState = new PlayerGetHitState(this);
        deadState = new PlayerDeadState(this);

        stateMachine = new StateMachine(idleState);     // 시작 상태

        data.playerHp = data.playerMaxHp;
        data.kickDamage = 10;
        data.isInvincible = false;
    }

    private void Update()
    {
        // 애니메이션: 방향키 입력이 있으면 1:Move, 없으면 0:Idle 
        float speed = data.PlayerMovement.CurrentSpeed;  // 멈춤 0, 걷기 3, 뛰기 8
        data.Animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        DecideState();  // Player가 State를 판단
        stateMachine.UpdateState(); // 현재 상태를 계속 행동함
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateState();
    }

    public void DecideState()
    {
        IState cur = stateMachine.CurrentState;

        // 1) 죽음
        if (data.playerHp <=0 )
        {
            stateMachine.ChangeState(deadState);
            return;
        }
        //2) 피격 (무적이면 무시)
        if (data.gotHit)
        {
            data.gotHit = false; // 피격 신호 끄기
            if (!data.isInvincible)
            {
                stateMachine.ChangeState(getHitState);
                return;
            }
        }
        // 3) 행동 중이면 끝날 때까지 기다림
        if (cur == rollState && !rollState.IsDone) return;
        if (cur == sliceState && !sliceState.IsDone) return;
        if (cur == kickState && !kickState.IsDone) return;
        if (cur == getHitState && !getHitState.IsDone) return;

        // 4) 입력
        if (Input.GetKeyDown(KeyCode.LeftShift)) { }    // 뛰기
        if (Input.GetKeyDown(KeyCode.S)) { stateMachine.ChangeState(rollState); return; }   // 구르기
        if (Input.GetKeyDown(KeyCode.A)) { stateMachine.ChangeState(kickState); return; }     // 발차기
        if (Input.GetKeyDown(KeyCode.D)) { stateMachine.ChangeState(sliceState); return; }     // 칼로 베기

        // 5) 기본: 방향키가 있으면 Move, 없으면 Idle
        stateMachine.ChangeState(data.PlayerMovement.Dir != Vector3.zero ? moveState : idleState);
    }

    public void TakeDamage(int damage)
    {
        // 무적 상태이면 무시
        if (data.isInvincible) return;

        data.playerHp -= damage;
        Debug.Log("Player가 " + damage + " 만큼의 데미지를 받았다. HP: " + data.playerHp);
        if (data.playerHp <= 0)
        {
            Debug.Log("Player가 사망했다.");
            data.playerHp = 0;
        }
    }

    //private IEnumerator GetHitRoutine()
    //{
    //    gotHit = true;
    //    // 리스폰 시간
    //    yield return new WaitForSeconds(respawnDelay);

    //    ChangeState(playerMovement.Dir != Vector3.zero ? PlayerState.Move : PlayerState.Idle);
    //}

    //private IEnumerator DeadRoutine()
    //{

    //}
}




