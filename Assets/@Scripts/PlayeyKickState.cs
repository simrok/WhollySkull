using UnityEngine;
using System.Collections.Generic;
public class PlayeyKickState : PlayerState
{
    private float elapsed;
    private Vector3 kickDir;

    // 이번 공격에서 이미 맞은 적 목록
    private HashSet<Monster> hitTargets = new HashSet<Monster>();
    

    public PlayeyKickState(Player player) : base(player) { }

    // 구현 해야함
    public bool IsDone => elapsed >= context.kickDuration + context.kickRecoveryTime;

    public override void OnStateEnter()
    {
        elapsed = 0f;
        kickDir = owner.transform.forward;
        context.PlayerMovement.canMove = false;

        hitTargets.Clear(); // 이번 공격에서 맞은 적 목록 비우기

        // 현재 바라보고 있는 방향으로 전진하면서 공격하는 애니메이션
        context.Animator.SetTrigger("Kick");
    }
    public override void OnStateUpdate()
    {
    }

    public override void OnStateFixedUpdate()
    {
        // 타격 구간:0.33초 ~ 0.4초 사이에 공격이 적에게 맞으면 적의 hp를 깎는다.
        if (elapsed >= 0.33 && elapsed <= 0.4)
        {
            //공격이 Enemy 한테 맞으면 적의 hp 깎기
            Collider[] colls = Physics.OverlapSphere(owner.transform.position, 0.7f);
            foreach (Collider coll in colls)
            {
                Monster monster = coll.GetComponentInParent<Monster>();
                if (monster == null) continue;
                if (hitTargets.Add(monster))    // 이번 공격에서 처음 맞는 적일 때만 true
                {
                    monster.TakeDamage(context.kickDamage);
                }
            }
        }
        elapsed += Time.fixedDeltaTime;
    }

    public override void OnStateExit()
    {
        context.PlayerMovement.canMove = true;  // 움직일 수 있음
    }
}