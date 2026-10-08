using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 보스 스킬 : 내려찍기
public class BossSlamSkill : BossSkill
{
    // Decal 설정
    [SerializeField] private Transform indicatorRoot;   // SlamIndicator 오브젝트의 Transform
    [SerializeField] private DecalProjector outline;    // 연한 바깥 원
    [SerializeField] private DecalProjector fill;       // 안쪽 원. 원이 커지면서 공격 타이밍을 알려줌
    protected const float Depth = 3f; // 데칼 depth
    
    // 손 오브젝트
    [SerializeField] protected Transform cubeTransform;
    [SerializeField] protected float windUpHeight = 6f;   // 들어올린 높이
    [SerializeField] protected float handHalfHeight = 6f; // 손 Scale Y의 절반(바닥에 닿는 위치 계산용)
    [SerializeField] protected float hoverHeight = 12f;   // 떠 있는 높이
    [SerializeField] protected float windupTime = 0.3f;    // 손이 6f만큼 올라가는 시간
    [SerializeField] protected float slamTime = 0.12f;    // 내려찍는 시간

    // Inspector
    //[SerializeField] float radius = 3f;
    //[SerializeField] float telegraphTime = 2f;
    //[SerializeField] private float recoverTime = 1f;   // 손이 바닥에 머무르는 시간 = 플레이어 반격 시간

    // 초기 Slam 코루틴
    //public override IEnumerator Run(BossMonsterController boss)
    //{
    //    Vector3 center = playerTransform.position;  // 현재 플레이어의 위치를 복사 
    //    indicatorRoot.position = center + Vector3.up * (Depth * 0.5f);  // 두 프로젝터의 부모를 바닥 위 Depth/2 지점에 배치

    //    // 손 오브젝트가 높은 곳(부모로부터 12f)에 떠있음
    //    handTransform.localPosition = new Vector3(0, hoverHeight, 0f);
    //    Vector3 from = new Vector3(0, hoverHeight, 0);
    //    Vector3 to = new Vector3(0, hoverHeight + windUpHeight, 0);

    //    float windupStartTime = data.telegraphTime - windupTime;      // 1.7초 부터 떠오르기

    //    // 피해범위 원
    //    float d = data.radius * 2f;
    //    outline.size = new Vector3(d, d, Depth);    // z(깊이)는 고정
    //    indicatorRoot.gameObject.SetActive(true);   // 경고 원 데칼 2개 + 손 오브젝트 켜기

    //    // vfx 재생
    //    if (!data.vfxPlayed)
    //    {
    //        data.vfxPlayed = true;
    //        VFXController vfx = VFXPool.Instance.GetFromPool(1);
    //        vfx.transform.position = center;
    //        vfx.Play();
    //    }

    //    float t = 0f;
    //    while (t < data.telegraphTime)          //  telegraphTime(2s)만큼 원이 커짐
    //    {
    //        t += Time.deltaTime;
    //        float fillProgress = t / data.telegraphTime;
    //        float windupProgress = (t - windupStartTime) / windupTime; // progress를 이용해 크기나 위치를 바꿈. 0~1로 증가하는 진행률
    //        fill.size = new Vector3(d * fillProgress, d * fillProgress, Depth);   // x, y값 늘리기

    //        // [예고] 손이 잠깐 위로 들림 (예비 동작)
    //        if (t >= windupStartTime)      // 1.7초 부터 떠오르기
    //        {
    //            handTransform.localPosition = Vector3.Lerp(from, to, windupProgress);  // 6만큼 더 올라가기
    //        }
    //        yield return null;
    //    }

    //    // 손 오브젝트
    //    Vector3 groundPos = new Vector3(0, -2.5f + handHalfHeight, 0);
    //    float hTime = data.telegraphTime + slamTime;    // 총 2 + 0.12 = 2.12초
    //    while (t < hTime)   // 2.12초 동안
    //    {
    //        t += Time.deltaTime;
    //        // 구간 진행률: 0~1로 증가하는 진행률
    //        float progress = (t - data.telegraphTime) / slamTime;
    //        // 빠르게 내려찍기
    //        handTransform.localPosition = Vector3.Lerp(to, groundPos, progress);
    //        yield return null;
    //    }
    //    handTransform.localPosition = groundPos;    // 바닥에 위치

    //    // 플레이어의 콜라이더를 통해 데미지 입히기
    //    Collider[] colliders = Physics.OverlapSphere(center, data.radius);

    //    foreach (Collider collider in colliders)
    //    {
    //        Player player = collider.GetComponentInParent<Player>();

    //        if (player != null)
    //        {
    //            // [발동] 경고 원 끄기 -> 범위 안의 IDamageable에게 데미지, 데미지는 손이 바닥에 닿는 순간
    //            player.TakeDamage(data.baseDamage);
    //        }
    //    }
    //    // 플레이어 반격 시간
    //    yield return new WaitForSeconds(data.recoverTime);

    //    // 마지막 위치 다시 맞추기
    //    handTransform.localPosition = to;
    //    data.vfxPlayed = false;
    //    // 모든 오브젝트 끄기
    //    indicatorRoot.gameObject.SetActive(false);
    //}

    public override IEnumerator Run(BossMonsterController boss)
    {
        Vector3 center = boss.playerTransform.position;  // 현재 플레이어의 위치를 복사 
        indicatorRoot.position = center + Vector3.up * (Depth * 0.5f);  // 두 프로젝터의 부모를 바닥 위 Depth/2 지점에 배치

        // 손 오브젝트가 높은 곳(부모로부터 12f)에 떠있음
        cubeTransform.localPosition = new Vector3(0, hoverHeight, 0f);
        Vector3 from = new Vector3(0, hoverHeight, 0);
        Vector3 to = new Vector3(0, hoverHeight + windUpHeight, 0);

        float windupStartTime = data.telegraphTime - windupTime;      // 1.7초 부터 떠오르기

        // 피해범위 원
        float d = data.radius * 2f;
        outline.size = new Vector3(d, d, Depth);    // z(깊이)는 고정
        indicatorRoot.gameObject.SetActive(true);   // 경고 원 데칼 2개 + 손 오브젝트 켜기

        // vfx 재생
        if (!data.vfxPlayed)
        {
            data.vfxPlayed = true;
            VFXController vfx = VFXPool.Instance.GetFromPool(data.vfxId);
            vfx.transform.position = center;
            vfx.Play();
        }

        float t = 0f;
        while (t < data.telegraphTime)          //  telegraphTime(2s)만큼 원이 커짐
        {
            t += Time.deltaTime;
            float fillProgress = t / data.telegraphTime;
            float windupProgress = (t - windupStartTime) / windupTime; // progress를 이용해 크기나 위치를 바꿈. 0~1로 증가하는 진행률
            fill.size = new Vector3(d * fillProgress, d * fillProgress, Depth);   // x, y값 늘리기

            // [예고] 손이 잠깐 위로 들림 (예비 동작)
            if (t >= windupStartTime)      // 1.7초 부터 떠오르기
            {
                cubeTransform.localPosition = Vector3.Lerp(from, to, windupProgress);  // 6만큼 더 올라가기
            }
            yield return null;
        }

        // 손 오브젝트
        Vector3 groundPos = new Vector3(0, -2.5f + handHalfHeight, 0);
        float hTime = data.telegraphTime + slamTime;    // 총 2 + 0.12 = 2.12초
        while (t < hTime)   // 2.12초 동안
        {
            t += Time.deltaTime;
            // 구간 진행률: 0~1로 증가하는 진행률
            float progress = (t - data.telegraphTime) / slamTime;
            // 빠르게 내려찍기
            cubeTransform.localPosition = Vector3.Lerp(to, groundPos, progress);
            yield return null;
        }
        cubeTransform.localPosition = groundPos;    // 바닥에 위치

        DoSlam(center, data.radius);

        yield return new WaitForSeconds(data.recoverTime);

        // 마지막 위치 다시 맞추기
        cubeTransform.localPosition = to;
        data.vfxPlayed = false;
        // 모든 오브젝트 끄기
        indicatorRoot.gameObject.SetActive(false);
    }

    protected void DoSlam(Vector3 pos, float r) // 플레이어 위치, 반지름
    {
        // 플레이어의 콜라이더를 통해 데미지 입히기
        Collider[] colliders = Physics.OverlapSphere(pos, r);

        foreach (Collider collider in colliders)
        {
            Player player = collider.GetComponentInParent<Player>();

            if (player != null)
            {
                // [발동] 경고 원 끄기 -> 범위 안의 IDamageable에게 데미지, 데미지는 손이 바닥에 닿는 순간
                player.TakeDamage(data.baseDamage);
            }
        }
    }
}
