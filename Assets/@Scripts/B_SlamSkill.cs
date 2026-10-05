using System.Collections;
using UnityEngine;

// 보스 스킬 : 내려찍기
public class B_SlamSkill : BossSkill
{
    [SerializeField] float waitTime = 0.5f;    // 플레이어 후딜 시간
    [SerializeField] GameObject warningDecal;

    private Transform target;

    private void Awake()
    {
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float moveDistance = data.moveSpeed * Time.deltaTime;

        if (direction.magnitude <= moveDistance )
        {
            StartCoroutine("Run");
            return;
        }

        transform.Translate(direction.normalized * moveDistance, Space.World);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.radius);

        foreach (Collider collider in colliders)
        {
            Player player = collider.GetComponentInChildren<Player>();

            if (player != null)
            {
                player.TakeDamage(data.baseDamage);
            }
        }

        Destroy(gameObject);    // 피해 입힌 후 끄기
    }

    public override IEnumerator Run(BossMonsterController boss)
    {
        // [예고] 경고 원 켜기 -> telegraphTime만큼 기다리기
        warningDecal.SetActive(true);
        yield return new WaitForSeconds(data.telegraphTime);
        // [발동] 경고 원 끄기 -> 범위 안의 IDamageable에게 데미지
        Explode();
        warningDecal.SetActive(false);
        // [플레이어 후딜] 잠깐 대기
        yield return new WaitForSeconds(waitTime);
    }
}
