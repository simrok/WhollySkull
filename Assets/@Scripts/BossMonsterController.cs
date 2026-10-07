using UnityEngine;

public class BossMonsterController : Monster
{
    [SerializeField] BossSkill slamSkill;

    // 보스 고유 필드
    [SerializeField] int phaseCount;   // 해당 보스의 페이즈 개수
    private int currentPhase = 1;    // 현재 보스전 페이즈
    public int CurrentPhase => currentPhase;

    private bool isInvincible;
    private bool isPhaseChanged;    // 페이즈가 바꼈는지

    // 각 페이즈 별 스킬
    // 만들기

    private void Start()
    {
        //onDie += DropBone;
    }

    protected override void Awake()
    {
        base.Awake();
        // 보스만의 초기화
    }

    private void Update()
    {
        // 디버깅용
        if (Input.GetKeyDown(KeyCode.K))
            StartCoroutine(slamSkill.Execute(this));
    }

    public override void TakeDamage(int damage)
    {
        // 무적 상태이면 무시
        if (isInvincible) return;
        // 코드 줄 추가
        base.TakeDamage(damage);
        // 페이즈 체크

    }

    protected override void Die()
    {
        // 사망 애니메이션

        base.Die();
    }
}
