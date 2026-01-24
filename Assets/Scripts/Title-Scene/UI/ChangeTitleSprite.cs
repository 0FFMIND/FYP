using UnityEngine;
using Manager;
using Utils;
using MVC;
using System.Collections;

/// <summary>
/// 根据语言切换标题 Sprite：EN -> enSprite，ZH -> zhSprite。
/// - 会缓存上一次语言，避免重复切换
/// </summary>
public class ChangeTitleSprite : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite enSprite;
    [SerializeField] private Sprite zhSprite;

    private LanguageCode _cachedLanguage;

    // alpha 渐变协程句柄
    [SerializeField] private float durationSeconds;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (!targetRenderer)
        {
            Debug.LogError("[ChangeTitleSprite]: Target SpriteRenderer 缺失");
            return;
        }
        targetRenderer.color = new Color(targetRenderer.color.r, targetRenderer.color.g, targetRenderer.color.b, 0f);
    }

    protected virtual void OnEnable()
    {
        EventBus.Subscribe<ELanguageChanged>(OnLanguageChanged);
        // 启用时先按当前语言刷新一次
        Refresh(force: true);
    }

    protected virtual void OnDisable()
    {
        EventBus.Unsubscribe<ELanguageChanged>(OnLanguageChanged);
    }

    private void OnLanguageChanged(ELanguageChanged _)
    {
        Refresh(force: false);
    }

    private void Refresh(bool force)
    {
        if (targetRenderer == null) return;

        var current = SettingsMgr.Instance.GetLanguage();

        // 避免重复设置
        if (!force && current == _cachedLanguage)
        {
            return;
        }

        _cachedLanguage = current;

        ApplySpriteByLanguage(current);
    }

    private void ApplySpriteByLanguage(LanguageCode code)
    {
        if (code == LanguageCode.en)
        {
            if (enSprite != null) targetRenderer.sprite = enSprite;
        }
        else if (code == LanguageCode.zh)
        {
            if (zhSprite != null) targetRenderer.sprite = zhSprite;
        }
    }

    /// <summary>
    /// 在 durationSeconds 秒内，把 SpriteRenderer 的 alpha 从 0 渐变到 1（也就是 0% -> 100%）。
    /// - 会停止上一次 fade，避免叠加
    /// </summary>
    public void FadeInAlpha()
    {
        if (targetRenderer == null) return;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        _fadeCoroutine = StartCoroutine(FadeAlphaRoutine(durationSeconds));
    }

    private IEnumerator FadeAlphaRoutine(float durationSeconds)
    {
        // 处理极端情况：time <= 0 直接设为 1
        if (durationSeconds <= 0f)
        {
            var c0 = targetRenderer.color;
            c0.a = 1f;
            targetRenderer.color = c0;
            yield break;
        }

        // 起始 alpha 设为 0
        var c = targetRenderer.color;
        c.a = 0f;
        targetRenderer.color = c;

        float t = 0f;
        while (t < durationSeconds)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / durationSeconds);

            c = targetRenderer.color;
            c.a = a;
            targetRenderer.color = c;

            yield return null;
        }

        // 收尾确保 1
        c = targetRenderer.color;
        c.a = 1f;
        targetRenderer.color = c;

        _fadeCoroutine = null;
    }
}
