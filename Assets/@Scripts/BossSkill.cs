using System.Collections;
using UnityEngine;

public abstract class BossSkill : MonoBehaviour
{
    [SerializeField] protected BossSkillData data; 
    BossMonsterController bossMonsterController;

    float lastUseTime = -999f; // 마지막 스킬 사용 시간

    // 1. 스킬 사용 가능 여부 확인
    public virtual bool CanUseSkill(BossMonsterController boss)
    {
        // 쿨타임, 거리, 페이즈를 확인해서 스킬 사용 가능 여부를 반환
        if (Time.time - lastUseTime < data.cooldown)
            return false; 
        if (Vector3.Distance(boss.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > data.range)
            return false;
        if (bossMonsterController.CurrentPhase < data.minPhase)
            return false;
        return true;
    }

    // 2. 스킬 실행
    public IEnumerator Execute(BossMonsterController boss)
    {
        lastUseTime = Time.time;
        yield return Run(boss); // 실제 내용은 자식이 채움
    }

    public abstract IEnumerator Run(BossMonsterController boss);
}
