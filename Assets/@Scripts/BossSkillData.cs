using UnityEngine;

//public enum SkillType { }
[System.Serializable]
public class BossSkillData
{
    //public string skillId;        // 저장과 동기화에 사용할 고유 ID
    public string skillName;        // UI에 표시할 이름
    [TextArea]
    public string description;

    //public SkillType skillType;

    public int minPhase;            // 스킬 사용 최소 보스 단계
    public float cooldown;          // 스킬 쿨타임
    public float duration;
    public float range;             // 사용 가능 사거리
    public float radius;            // 효과 범위

    public int baseDamage;          // 기본 데미지
    //public float baseHeal = 0f;   // 기본 회복량

    // 속도
    public float attackSpeed;       // 공격 속도
    public float moveSpeed;         // 이동 속도

    // Decal
    public float telegraphTime;     // 예고 시간

    public GameObject skillPrefab;  // 생성할 스킬 오브젝트
}
