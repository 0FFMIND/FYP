using System;
using System.Collections.Generic;
using System.Text;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    // 右侧“可复用”的详情面板
    public class JournalDetailPanel : MonoBehaviour
    {
        // 日期文本
        [SerializeField]
        private TMP_Text dateText;

        private string dateTextKey = "{yyyy}年{MM}月{dd}日 {period}{hh}点{mm}分";

        private static readonly Color COLOR_WHITE = Color.white;
        private static readonly Color COLOR_PENDING = new Color(0.54f, 0.54f, 0.54f, 1f); // #8A8A8A

        [SerializeField]
        private Transform contentRoot; // 容器（VerticalLayoutGroup/ContentSizeFitter）

        [SerializeField]
        private GameObject linePrefab; // 你的 journalLineText 预制体（上面应挂有 LocalizedText + TMP_Text）

        // 简单池
        private readonly List<GameObject> _pool = new();

        [SerializeField]
        private GameObject completed;

        [SerializeField]
        private float lineSpacing = 6f;

        private void OnEnable()
        {
            EventBus.Subscribe<EJournalSelected>(OnSelected);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EJournalSelected>(OnSelected);
        }

        private void OnSelected(EJournalSelected e)
        {
            // 收到事件时，用事件携带的 key 展示对应日记详情
            ShowByKey(e.Key);
        }

        // 根据传入的 key 显示对应的 Journal 详情
        public void ShowByKey(string key)
        {
            // 防御式：空 key 直接返回，避免无意义查询
            if (string.IsNullOrEmpty(key))
            {
                if (completed)
                    completed.SetActive(false);
                dateText.gameObject.SetActive(false);
                Debug.LogWarning(
                    $"[JournalDetailPanel] ShowByKey received an empty key. GameObject=\"{name}\""
                );
                return;
            }
            dateText.gameObject.SetActive(true);
            var model = JournalMgr.Instance?.Model;

            // 防御式：管理器或数据模型未就绪时直接返回
            if (model == null)
            {
                Debug.LogWarning(
                    $"[JournalDetailPanel] JournalMgr.Instance is null. Cannot resolve model. key=\"{key}\", GameObject=\"{name}\""
                );
                return;
            }

            var it = model.Find(key);
            if (it == null)
            {
                Debug.LogWarning(
                    $"[JournalDetailPanel] JournalMgr.Model is null. Cannot display journal. key=\"{key}\", GameObject=\"{name}\""
                );
                return;
            }
            if (completed)
                completed.SetActive(it.status == JournalStatus.Completed);
            // 绑定 UI 文本（日期 + 正文）
            BindTexts(it);
        }

        // 把 JournalItem 的创建时间和正文信息绑定到右侧的文本上
        private void BindTexts(JournalItem it)
        {
            // 从数据项读出 ISO8601 时间（例如 "2025-10-26T10:11:12.3456789Z"；为空表示未设置）
            DateTime iso = it.createdAt;
            // 将 DateTime 转成用于模板占位符的字典：{ "yyyy","MM","dd","hh","mm","period" }
            var args = JournalDateParser.ParseToArgs(iso);

            // 在同节点上获取 LocalizedText 组件并注入占位符字典
            dateText.GetComponent<LocalizedText>().SetParams(args);
            dateText.GetComponent<LocalizedText>().SetKey(dateTextKey, true);

            // —— 行内容：逐行实例化/复用 Prefab 并填 key ——
            RebuildLines(it);

            // 刷新布局
            if (contentRoot is RectTransform rt)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        private void RebuildLines(JournalItem it)
        {
            if (!(contentRoot is RectTransform root))
                return;

            int need = it.contents?.Count ?? 0;

            // 扩容池并统一锚点/枢轴为左上
            while (_pool.Count < need)
            {
                var go = Instantiate(linePrefab, contentRoot);
                var crt = go.transform as RectTransform;
                if (crt)
                {
                    crt.anchorMin = new Vector2(0f, 1f);
                    crt.anchorMax = new Vector2(0f, 1f);
                    crt.pivot = new Vector2(0f, 1f);
                }
                go.SetActive(false);
                _pool.Add(go);
            }

            float innerWidth = root.rect.width; // 只用容器宽度，不做左右内边距
            float cursorY = 0f; // 从顶部开始往下排

            for (int i = 0; i < _pool.Count; i++)
            {
                var go = _pool[i];

                if (i < need)
                {
                    var ln = it.contents[i];
                    if (ln == null || ln.line == null)
                    {
                        go.SetActive(false);
                        continue;
                    }

                    // 1) 本地化 key
                    var loc = go.GetComponent<LocalizedText>();
                    if (loc != null)
                    {
                        loc.enabled = true;
                        loc.SetKey(ln.line.TextKey, refresh: true);
                    }

                    // 2) 颜色（Fixed/Done=白；Pending=灰）
                    var text = go.GetComponent<TMP_Text>();
                    if (text)
                    {
                        text.color =
                            (ln.line.Kind == JournalLineKind.Fixed || ln.State == StepState.Done)
                                ? COLOR_WHITE
                                : COLOR_PENDING;

                        text.enableWordWrapping = true;
                        text.ForceMeshUpdate();

                        // 计算在指定宽度下的高度
                        float prefH = Mathf.Ceil(
                            text.GetPreferredValues(text.text, innerWidth, 0f).y
                        );

                        // 3) 定位+尺寸（左上对齐）
                        var crt = go.transform as RectTransform;
                        if (crt)
                        {
                            crt.sizeDelta = new Vector2(innerWidth, prefH);
                            crt.anchoredPosition = new Vector2(0f, -cursorY);
                        }

                        // 光标向下推进：本行高度 + 行距
                        cursorY += prefH + lineSpacing;
                    }

                    go.SetActive(true);
                }
                else
                {
                    go.SetActive(false);
                }
            }

            // 去掉末尾多加的行距
            if (need > 0)
                cursorY -= lineSpacing;

            // 更新容器高度（配合 ScrollRect）
            root.sizeDelta = new Vector2(root.sizeDelta.x, Mathf.Max(0f, cursorY));

            // 强制重建布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        }
    }
}
