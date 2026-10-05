using UnityEngine;

// 적 타입
//public enum MonsterType { Normal, Boss }

[System.Serializable]
public class MonsterData
{
    // 이름 + 몬스터 타입
    public string monsterName;
    //public MonsterType monsterType;

    // 체력
    public int maxHealth;

    // 이동
    public float moveSpeed;  // 이동 속도

    // 일반 몬스터 공격
    //public float attackSpeed;  // 공격 속도
    //public int damageToBase;  // 기본 공격 데미지

    // 보상: 평온의 뼛가루 개수
    public int minRewardBone;
    public int maxRewardBone;
}
