using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossMultiSlamSkill : BossSlamSkill
{
    [SerializeField] Transform indicatorPrefab;      // 인디케이터 프리팹
    [SerializeField] private float spread;    // 큐브가 떨어지는 범위:플레이어와 떨어진 지점
    [SerializeField] private int count;
    [SerializeField] private float interval;    // 각 큐브가 떨어지는 간격 시간(초)
    [SerializeField] private float disappearInterval;    // 각 큐브가 사라지는 간격 시간(초)

    public override IEnumerator Run(BossMonsterController boss)
    {
        Bounds bound = boss.ArenaCollider.bounds;
        Vector3 playerPosition = boss.playerTransform.position;
 
        for (int i = 0; i < count;i++)
        {
            float x = playerPosition.x + Random.Range(-spread, spread);
            float z = playerPosition.z + Random.Range(-spread, spread);

            // 원이 아레나 밖으로 나가지 않도록 반지름만큼 안쪽으로 제한
            x = Mathf.Clamp(x, bound.min.x + data.radius, bound.max.x - data.radius);
            z = Mathf.Clamp(z, bound.min.z + data.radius, bound.max.z - data.radius);

            // 큐브가 떨어질 위치. y는 플레이어 높이
            Vector3 pos = new Vector3(x, playerPosition.y, z);

            //Debug.Log(i + "개");
            StartCoroutine(DropOne(pos));
            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(data.recoverTime);
    }

    IEnumerator DropOne(Vector3 pos)
    {
        // [순서] 경고 원 생성 → telegraphTime 대기 → DoSlam(pos, r) → 정리

        // indicator를 Instantiate
        // 두 프로젝터의 부모를 바닥 위 Depth/2 지점에 배치
        Transform indicator = Instantiate(indicatorPrefab, pos + Vector3.up * (Depth * 0.5f), Quaternion.identity);
        DecalProjector myOutline = indicator.Find("Outline").GetComponent<DecalProjector>();
        DecalProjector myFill = indicator.Find("Fill").GetComponent<DecalProjector>();
        Transform myCube = indicator.Find("Cube");

        // 큐브 오브젝트가 높은 곳(부모로부터 12f)에 떠있음
        myCube.localPosition = new Vector3(0, hoverHeight, 0f);
        Vector3 from = new Vector3(0, hoverHeight, 0);
        Vector3 to = new Vector3(0, hoverHeight + windUpHeight, 0);

        float windupStartTime = data.telegraphTime - windupTime;      // [예고] 잠시 떠오르기의 지속시간

        // 피해범위 원
        float d = data.radius * 2f;
        myOutline.size = new Vector3(d, d, Depth);    // z(깊이)는 고정
        indicator.gameObject.SetActive(true);   // 경고 원 데칼 2개 + 손 오브젝트 켜기

        // vfx 재생
        VFXController vfx = VFXPool.Instance.GetFromPool(data.vfxId); ;
        vfx.transform.position = pos;
        vfx.Play();
 
        float t = 0f;
        while (t < data.telegraphTime)          //  telegraphTime(2s)만큼 원이 커짐
        {
            t += Time.deltaTime;
            float fillProgress = t / data.telegraphTime;
            float windupProgress = (t - windupStartTime) / windupTime; // progress를 이용해 크기나 위치를 바꿈. 0~1로 증가하는 진행률
            myFill.size = new Vector3(d * fillProgress, d * fillProgress, Depth);   // x, y값 늘리기

            // [예고] 큐브가 잠깐 위로 들림 (예비 동작)
            if (t >= windupStartTime)      // 1.7초 부터 떠오르기
            {
                myCube.localPosition = Vector3.Lerp(from, to, windupProgress);  // 6만큼 더 올라가기
            }
            yield return null;
        }

        // 큐브 오브젝트
        Vector3 groundPos = new Vector3(0, -2.5f + handHalfHeight, 0);  // 바닥 위치는 큐브의 절반 높이 보다 -2.5f;
        float hTime = data.telegraphTime + slamTime;    // 총 2 + 0.12 = 2.12초
        while (t < hTime)   // 2.12초 동안
        {
            t += Time.deltaTime;
            // 구간 진행률: 0~1로 증가하는 진행률
            float progress = (t - data.telegraphTime) / slamTime;
            // 빠르게 내려찍기
            myCube.localPosition = Vector3.Lerp(to, groundPos, progress);
            yield return null;
        }
        myCube.localPosition = groundPos;    // 바닥에 위치

        DoSlam(indicator.position, data.radius);

        yield return new WaitForSeconds(disappearInterval);

        // 마지막 위치 다시 맞추기
        myCube.localPosition = to;
        // vfx 끄기
        vfx.gameObject.SetActive(false);
        // indicator 끄기
        Destroy(indicator.gameObject);
    }
}
