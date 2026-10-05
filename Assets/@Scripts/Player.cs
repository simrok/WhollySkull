using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour, IDamageable
{
    public PlayerData context = new PlayerData();
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
        context.Init(GetComponent<Rigidbody>(), GetComponentInChildren<Animator>(), GetComponent<PlayerMovement>());

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        rollState = new PlayerRollState(this);
        kickState = new PlayeyKickState(this);
        sliceState = new PlayerSliceState(this);
        getHitState = new PlayerGetHitState(this);
        deadState = new PlayerDeadState(this);

        stateMachine = new StateMachine(idleState);     // 시작 상태

        context.playerHp = context.playerMaxHp;
        context.kickDamage = 10;
        context.isInvincible = false;
    }

    private void Update()
    {
        // 애니메이션: 방향키 입력이 있으면 1:Move, 없으면 0:Idle 
        float speed = context.PlayerMovement.CurrentSpeed;  // 멈춤 0, 걷기 3, 뛰기 8
        context.Animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

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
        if (context.playerHp <=0 )
        {
            stateMachine.ChangeState(deadState);
            return;
        }
        //2) 피격 (무적이면 무시)
        if (context.gotHit)
        {
            context.gotHit = false; // 피격 신호 끄기
            if (!context.isInvincible)
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
        stateMachine.ChangeState(context.PlayerMovement.Dir != Vector3.zero ? moveState : idleState);
    }

    public void TakeDamage(int damage)
    {
        context.playerHp -= damage;
        Debug.Log("Player가 " + damage + " 만큼의 데미지를 받았다. HP: " + context.playerHp);
        if (context.playerHp <= 0)
        {
            Debug.Log("Player가 사망했다.");
            context.playerHp = 0;
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




