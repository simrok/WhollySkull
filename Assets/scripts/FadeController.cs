using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeController : MonoBehaviour
{
    [Header("Time Setting")]
    public float duration = 0.2f;  // 维持时间
    public float fadeTime = 0.5f;  // 衰减总耗时

    [Header("Slope Rate")]
    [Range(0.1f, 5.0f)]
    public float slope = 1.0f; // 1.0是线性，>1先慢后快，<1先快后慢
    
    [Tooltip("Turn this on to draw complex curves manually")]
    public bool useCustomCurve = false;
    public AnimationCurve customCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private List<Material> childMaterials = new List<Material>();
    private Coroutine fadeCoroutine;

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            childMaterials.Add(rend.material);
        }
        SetAlpha(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(DoFadeEffect());
        }
    }

    IEnumerator DoFadeEffect()
    {
        // 1. 初始设为 1
		if(!useCustomCurve)
		{
			SetAlpha(1.0f);
		}

        // 2. 等待持续时间
        yield return new WaitForSeconds(duration);

        // 3. 按斜率衰减
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime; // 归一化的进度 (0 到 1)
            
            float alphaValue;
            if (useCustomCurve)
            {
                // 方式 A: 直接从动画曲线求值
                alphaValue = customCurve.Evaluate(t);
            }
            else
            {
                // 方式 B: 使用幂函数控制斜率 (1-t 是为了从1到0衰减)
                // slope 越大，曲线越向内凹（衰减感越重）
                alphaValue = 1.0f -  Mathf.Pow(t, slope);
            }

            SetAlpha(alphaValue);
            yield return null;
        }

        SetAlpha(0);
    }

    void SetAlpha(float value)
    {
        foreach (Material mat in childMaterials)
        {
            if (mat.HasProperty("_GlobalAlpha"))
                mat.SetFloat("_GlobalAlpha", value);
        }
    }
}