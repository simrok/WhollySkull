using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [Header("VFX의 지속시간(이 시간이 지나면 disable)")]
    [SerializeField] private float mLifeTime;   // vfx가 몇 초 후에 Disable 될지

    private Coroutine mCodisable;  // 비활성화 코루틴. 코루틴 중복 호출을 막음

    // VFX 효과 재생
    public void Play(Transform targetTransform = null) 
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;  // 해당 트랜스폼 위치에서 재생
            transform.rotation = targetTransform.rotation;
        }

        GetComponentInChildren<ParticleSystem>().Play();   // 파티클 재생

        if (mCodisable != null) // 코루틴이 이미 실행 중이면 중지
            StopCoroutine(mCodisable);

        mCodisable = StartCoroutine(CoDisable());
    }

    // VFX 효과 재생 중지
    private IEnumerator CoDisable()
    {
        yield return new WaitForSeconds(mLifeTime);
        gameObject.SetActive(false);    // 비활성화되면 오브젝트풀링에서 이 오브젝트 재사용 가능
    }
}
