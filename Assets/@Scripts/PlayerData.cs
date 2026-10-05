using UnityEngine;

// 플레이어 컨텍스트
[System.Serializable]
public class PlayerData
{
    // 컴포넌트 참조
    public Rigidbody Rb { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }

    // sliceState VFX 앵커
    public Transform sliceVfxAnchor;

    public void Init(Rigidbody _rb, Animator _anim, PlayerMovement _playerMovement)
    {
        Rb = _rb;
        Animator = _anim;
        PlayerMovement = _playerMovement;
    }

    // 체력
    public float playerHp;
    public float playerMaxHp = 100f;

    // Slice
    public int sliceDamage = 10; // 조정 필요
    public float sliceMoveSpeed = 8f;   // 8 * 0.3초 동안 2.4 유닛 이동
    public AnimationCurve sliceSpeedCurve; // Slice 애니메이션 시, 마지막에 걷는 속도 곡선
    public float sliceDuration = 0.6f;    // 공격 시간
    public float sliceRecoveryTime = 0.15f; // 재입력 불가 구간

    // Kick
    public int kickDamage = 5;
    public float kickDuration = 0.7f;    // 공격 시간
    public float kickRecoveryTime = 0.15f; // 재입력 불가 구간 

    // Roll
    public float rollDuration = 0.67f; // 지속시간 0.35~0.45
    public float rollMoveSpeed = 20f; // 구르기 이동 속도(거리 = 속력 * 시간)
    public float invincibleRatio = 0.65f;  // 무적상태는 전체 중 앞 65% 구간 지속
    public AnimationCurve rollSpeedCurve;
    public float rollRecoveryTime = 0.1f;    // 재입력 불가 구간 0.1~0.15초
    public bool isInvincible; // 무적 상태인지

    // GetHit
    public bool gotHit;    //  적에게 피격당하고 있는지

    // Dead
    public float respawnDelay;
    public Vector3 respawnPoint;

    // 공격 한 번의 데이터를 묶은 클래스
    public SliceData[] slices; 
}

// 공격 한 번의 데이터를 묶은 클래스
[System.Serializable]
public class SliceData
{
    public float vfxTime;   // 해당 공격에서 VFX가 재생되는 시간
    public Transform vfxAnchor; // VFX가 재생되는 위치
    public float hitStart, hitEnd;
    public float moveStart, moveEnd;
    public AnimationCurve speedCurve;
    public float duration;
}