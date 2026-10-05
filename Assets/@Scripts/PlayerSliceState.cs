using UnityEngine;
using System.Collections.Generic;

public class PlayerSliceState : PlayerState
{
    private float elapsed;
    private Vector3 sliceDir;
    private int sliceIndex;     // 0: 정방향 베기, 1: 반대 베기
    private float lastSliceEndTime = -999f;

    // 이번 공격에서 이미 맞은 적 목록
    private HashSet<Monster> hitTargets = new HashSet<Monster>();

    // Slice VFX
    private bool vfxPlayed; // Slice VFX가 재생되었는지 여부

    public PlayerSliceState(Player player) : base(player) { }


    public bool IsDone => elapsed >= context.sliceDuration + context.sliceRecoveryTime;

    public override void OnStateEnter()
    {
        elapsed = 0f;
        sliceDir = owner.transform.forward;
        context.PlayerMovement.canMove = false;

        if (Time.time - lastSliceEndTime > 0.5f) sliceIndex = 0;    // 콤보가 끊겼으면 리셋
        context.Animator.SetInteger("SliceIndex", sliceIndex);
        // 현재 바라보고 있는 방향으로 전진하면서 칼을 휘두르는 애니메이션
        context.Animator.SetTrigger("Slice");
        
        hitTargets.Clear(); // 이번 공격에서 맞은 적 목록 비우기

        vfxPlayed = false;
    }

    public override void OnStateFixedUpdate()
    {
        // vfx 재생
        SliceData data = context.slices[sliceIndex];        // sliceIndex==0 : 정방향 베기, sliceIndex==1 : 반대 방향 베기
        if (!vfxPlayed && elapsed >= data.vfxTime) // 타격 시작 타이밍(공격1, 공격2의 vfx 재생 타이밍이 서로 다름)
        {
            vfxPlayed = true;
            VFXPool.Instance.GetFromPool(0).Play(data.vfxAnchor);
        }

        // 타격 타이밍: 0.28초 ~ 0.4초 사이에 적에게 공격이 들어감
        if (elapsed >= data.hitStart && elapsed <= data.hitEnd)
        {
            //공격이 Enemy 한테 맞으면 적의 hp 깎기
            Collider[] colls = Physics.OverlapSphere(owner.transform.position, 0.7f);
            foreach (Collider coll in colls)
            {
                Monster monster = coll.GetComponentInParent<Monster>();
                if (monster == null) continue;
                if (hitTargets.Add(monster))    // 이번 공격에서 처음 맞는 적일 때만 true
                {
                    monster.TakeDamage(context.sliceDamage);
                }
            }
        }
        // 앞으로 내딛는 이동
        if (elapsed >= data.moveStart && elapsed <= data.moveEnd)   // 0.3초 동안 앞으로 전진
        {
            float t = (elapsed - data.moveStart) / (data.moveEnd - data.moveStart);
            float speed = context.sliceMoveSpeed * context.sliceSpeedCurve.Evaluate(t);   // t = elapsed / context.sliceDuration
            context.PlayerMovement.ForceMove(sliceDir, speed);
        }
        else
        {
            // 칼로 베기 이동이 끝나면 제자리
            context.PlayerMovement.ForceMove(Vector3.zero, 0f);
        }
        elapsed += Time.fixedDeltaTime;
    }

    public override void OnStateUpdate()
    {
    }

    public override void OnStateExit()
    {
        context.PlayerMovement.canMove = true;  // 움직일 수 있음
        lastSliceEndTime = Time.time;  // 마지막 공격 종료 시간 기록
        sliceIndex = 1 - sliceIndex;   // 다음 공격은 반대 방향으로 베기
    }
}
