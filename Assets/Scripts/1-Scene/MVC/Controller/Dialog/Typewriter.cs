using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
namespace MVC
{
    /// <summary>
    /// 负责逐字显示、打字音效、跳过显示全文、维护进度（供语言切换映射）。
    /// </summary>
    public class Typewriter : MonoBehaviour
    {
        private Coroutine typingCo;
        private TMP_Text tmp;
        private ArrowIndicator arrowIndicator;
        private bool enableTypingSfx;
        private float typingRate;
        private float typeSpeed;
        private int typingTotal;
        private float progress; // 0..1
        public bool IsTyping => typingCo != null;

        /// <summary>
        /// 开始对指定 TMP_Text 逐字打字，并处理音效和箭头显示
        /// </summary>
        public void StartTyping(
            TMP_Text tmp,
            string fullText,
            float typeSpeed,
            float typingRate,
            bool enableTypingSfx,
            ArrowIndicator arrowIndicator
        )
        {
            StopTypingInternal();

            this.tmp = tmp;
            this.arrowIndicator = arrowIndicator;
            this.enableTypingSfx = enableTypingSfx;
            this.typingRate = typingRate;
            this.typeSpeed = typeSpeed;

            typingCo = StartCoroutine(TypeRoutine(fullText ?? string.Empty));
        }

        /// <summary>
        /// 立即显示全文
        /// </summary>
        public void Skip()
        {
            if (tmp == null) return;

            // 停止逐字协程，但不清空 tmp 引用
            StopTypingInternal(false);

            progress = 1f;
            // 直接拉满可见字符
            tmp.maxVisibleCharacters = int.MaxValue;
            // 强制刷新
            tmp.ForceMeshUpdate();
        }

        /// <summary>
        /// 语言切换后，按旧进度百分比映射到新文本长度
        /// </summary>
        public void RebindAfterLanguageChange(string newText)
        {
            if (tmp == null) return;
            tmp.text = newText ?? string.Empty;
            tmp.ForceMeshUpdate();

            typingTotal = tmp.textInfo.characterCount;
            // 按百分比映射到新长度
            tmp.maxVisibleCharacters = Mathf.RoundToInt(progress * typingTotal);
        }

        private IEnumerator TypeRoutine(string fullText)
        {
            // 若文本组件引用为空
            if (tmp == null)
            {
                typingCo = null;
                yield break;
            }
            // 设置文本内容
            tmp.text = fullText;
            tmp.ForceMeshUpdate();
            tmp.maxVisibleCharacters = 0;

            // 设置打字进度
            typingTotal = tmp.textInfo.characterCount;
            int cnt = 0;
            progress = 0f;

            for (; ; )
            {
                int curTotal = typingTotal;
                int visNow = tmp.maxVisibleCharacters;
                if (visNow >= curTotal)
                    break;

                tmp.maxVisibleCharacters = visNow + 1;
                progress = Mathf.Clamp01(
                    (float)tmp.maxVisibleCharacters / Mathf.Max(1, typingTotal)
                );
                // 英文两字符一个音效；中文每个字符一个音效
                bool isEn = SettingsMgr.Instance.GetLanguage() == LanguageCode.en;
                if (isEn)
                {
                    cnt++;
                    if (cnt >= 2)
                    {
                        cnt = 0;
                        if (enableTypingSfx)
                        {
                            AudioManager.Instance.PlaySFX("typing");
                        }
                    }
                }
                else
                {
                    if (enableTypingSfx)
                    {
                        AudioManager.Instance.PlaySFX("typing");
                    }
                }
                float wait = typeSpeed / typingRate;
                if (isEn)
                    wait *= 0.5f;
                yield return new WaitForSeconds(wait);
            }

            progress = 1f;
            typingCo = null;

            // 打字结束显示箭头
            arrowIndicator?.PositionArrowUnderText(tmp);
        }

        private void StopTypingInternal(bool clearRefs = true)
        {
            if (typingCo != null)
            {
                StopCoroutine(typingCo);
                typingCo = null;
            }

            if (clearRefs)
            {
                tmp = null;
                arrowIndicator = null;
            }
        }
    }
}