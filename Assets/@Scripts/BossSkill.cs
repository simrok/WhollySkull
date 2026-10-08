using System.Collections;
using UnityEngine;

public abstract class BossSkill : MonoBehaviour
{
    [SerializeField] protected BossSkillData data;

    float lastUseTime = -999f; // 마지막 스킬 사용 시간

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

    // 1. 스킬 사용 가능 여부 확인
    public virtual bool CanUseSkill(BossMonsterController boss)
    {
        // 아레나 콜라이더 내부에 있는지 확인
        if (boss.PlayerInArena())
        {
            // 쿨타임, 거리, 페이즈를 확인해서 스킬 사용 가능 여부를 반환
            //Debug.Log("쿨타임 중" + data.cooldown + ", 플탐: " + (Time.time - lastUseTime));
            if (Time.time - lastUseTime < data.cooldown)
            {
                return false;
            }

            float dist = Vector3.Distance(boss.transform.position, boss.playerTransform.position);
            if (dist > data.range)
            {
                Debug.Log($"너무 멂: 거리 {dist}, range {data.range}");
                return false;
            }

            if (boss.CurrentPhase < data.minPhase)
            {
                Debug.Log($"페이즈 부족: 현재 {boss.CurrentPhase}, 필요 {data.minPhase}");
                return false;
            }
            return true;
        }
        return false;
    }

    // 2. 스킬 실행
    public IEnumerator Execute(BossMonsterController boss)
    {
        lastUseTime = Time.time;
        yield return Run(boss); // 실제 내용은 자식이 채움
    }

    public abstract IEnumerator Run(BossMonsterController boss);
}
