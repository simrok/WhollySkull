using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    protected GameObject Player;
    
    // 컴포넌트
    protected Rigidbody rb;
    protected Animator animator;
    [SerializeField] protected MonsterData data;

    // 데이터
    protected int currentHealth;
    protected bool isDead;

    //[SerializeField] private Collider[] colls;
    //Collider[] playerInsideZone;
    //Collider[] playerOutsideZone;
    protected virtual void Awake()
    {
        currentHealth = data.maxHealth;
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    public void DropBone() 
    {
        int dropBone = Random.Range(data.minRewardBone, data.maxRewardBone+1);
        Debug.Log("평온의 뼛가루 드랍 개수: " + dropBone);
       // PlayerState.instance.GetBone += dropBone;
    }

    public virtual void TakeDamage(int damage)
    {
        // 이미 죽었는지 확인 (가드)
        if (isDead) return;

        // 체력 깎기
        currentHealth -= damage;
        Debug.Log(data.monsterName + "가 " + damage + " 만큼의 데미지를 받았다. / hp: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log(data.monsterName + "가 사망했다.");
            currentHealth = 0;
            Die();
        }
    }
    protected virtual void Die()
    {
        // 평온의 뼛가루 드랍
        DropBone();
        // 오브젝트 정리
        Destroy(gameObject, 2f);    // 2초 뒤 삭제
    }

    // Player에게 공격을 함

    // Player가 가까이 오면 근처에 접근하면서 공격을 함

    // 주변 동료 몬스터가 공격당했을 때 같이 '플레이어 공격' 태세로 전환
    //public void NearEnemyAttack(Vector3 pos)
    //{
    //    // 반지름 2의 구 안에 콜라이더 붙은 오브젝트들 추출해서 배열에 저장
    //   Collider[] colls = Physics.OverlapSphere(transform.position, 2f);

    //   foreach (Collider coll in colls)
    //   {
    //        Player player = coll.GetComponent<Player>();
    //   }
    //}
}
