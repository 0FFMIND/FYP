using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class ChoiceCtl : MonoBehaviour
    {
        [Header("UI Refs")] // 在 Inspector 显示一个标题分组
        [SerializeField]
        private GameObject root; // 整个选择面板的根节点（用来整体显示/隐藏）

        [SerializeField]
        private GameObject[] panels;

        public Action onClosed;

        [Header("Anim")]
        [SerializeField]
        private float openDuration = 0.2f; // 开场：Y 0→1

        [SerializeField]
        private float closeDuration = 0.2f; // 退场：Y 1→0

        private RectTransform _currentPanelRT;
        private Coroutine _anim;
        private CanvasGroup _cg;

        public void ShowWithClosed(Action onClosed, ChoiceModel model)
        {
            this.onClosed = onClosed;
            Show(model);
        }

        public void Show(ChoiceModel model)
        {
            if (!root || panels == null || panels.Length == 0)
            {
                Debug.LogError("[ChoiceCtl] 引用未绑定（root/panels）");
                return;
            }

            // 1) 选择并激活面板
            int idx = Mathf.Clamp(model.choicePanel, 0, panels.Length - 1);
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i])
                    panels[i].SetActive(i == idx);
            }
            var panelGO = panels[idx];
            if (!panelGO)
            {
                Debug.LogError($"[ChoiceCtl] panels[{idx}] 为 null");
                return;
            }
            var panel = panelGO.transform;

            // 2) 找标题 TMP（直系子节点中的第一个 TMP_Text）
            TMP_Text title = FindDirectChildTMP(panel);
            if (title)
                title.text = model.choiceHeader ?? string.Empty;
            else
                Debug.LogWarning($"[ChoiceCtl] 面板 {panel.name} 未找到直系 TMP_Text 标题");

            // 3) 找 option 容器（优先精确名“option”，否则模糊大小写包含）
            Transform optionRoot = FindOptionContainer(panel);
            if (!optionRoot)
            {
                Debug.LogError($"[ChoiceCtl] 面板 {panel.name} 未找到 'option' 容器");
                return;
            }
            // 4) 绑定现有按钮：按 option 容器的子物体顺序对应 items[]
            var items = model.items ?? Array.Empty<ChoiceData>();
            int btnCount = optionRoot.childCount;
            int bindCount = Mathf.Min(btnCount, items.Length);

            // 逐个绑定
            for (int i = 0; i < bindCount; i++)
            {
                var data = items[i];
                var child = optionRoot.GetChild(i);
                var btn = child.GetComponent<Button>();
                if (!btn)
                {
                    Debug.LogWarning(
                        $"[ChoiceCtl] 选项容器第 {i} 个子物体缺少 Button 组件：{child.name}"
                    );
                    continue;
                }

                // 文案：在该按钮的子层级里找 TMP_Text
                var label = btn.GetComponentInChildren<TMP_Text>(true);
                if (label)
                    label.text = data.label ?? string.Empty;

                // 点击：先调用 UnityEvent，再关闭面板
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Hide(() => data.onClick?.Invoke());
                });

                // 确保可见可点
                if (!child.gameObject.activeSelf)
                    child.gameObject.SetActive(true);
                if (label)
                    label.raycastTarget = false; // 避免文本吞点击
            }

            // 多余按钮：没有对应 item 的，统一隐藏
            for (int i = bindCount; i < btnCount; i++)
            {
                optionRoot.GetChild(i).gameObject.SetActive(false);
            }

            // 开场动画（Y: 0 → 1）
            if (_anim != null)
                StopCoroutine(_anim);
            if (_cg == null)
            {
                _cg = root.GetComponent<CanvasGroup>();
                if (_cg == null)
                    _cg = root.AddComponent<CanvasGroup>();
            }

            root.SetActive(true);
            _currentPanelRT = panel as RectTransform;
            if (_currentPanelRT != null)
            {
                _currentPanelRT.localScale = new Vector3(1f, 0f, 1f);
                _cg.interactable = false;
                _cg.blocksRaycasts = false;
                _anim = StartCoroutine(
                    ScaleY(
                        _currentPanelRT,
                        0f,
                        1f,
                        openDuration,
                        () =>
                        {
                            _cg.interactable = true;
                            _cg.blocksRaycasts = true;
                            _anim = null;
                        },
                        null
                    )
                );
            }
            else
            {
                _cg.interactable = true;
                _cg.blocksRaycasts = true;
            }
        }

        // 收起选择面板
        public void Hide(Action afterClosed = null)
        {
            // 退场动画（Y: 1 → 0），结束后再回调并隐藏
            if (_anim != null)
                StopCoroutine(_anim);

            if (_cg == null)
            {
                _cg = root.GetComponent<CanvasGroup>();
                if (_cg == null)
                    _cg = root.AddComponent<CanvasGroup>();
            }
            _cg.interactable = false;
            _cg.blocksRaycasts = false;

            _anim = StartCoroutine(
                ScaleY(
                    _currentPanelRT,
                    1f,
                    0f,
                    closeDuration,
                    () =>
                    {
                        var cb = onClosed;
                        onClosed = null;
                        cb?.Invoke();
                        _anim = null;
                    },
                    afterClosed
                )
            );
        }

        // —— 辅助：找直系子里第一个 TMP_Text 作为标题 ——
        private TMP_Text FindDirectChildTMP(Transform parent)
        {
            int c = parent.childCount;
            for (int i = 0; i < c; i++)
            {
                var t = parent.GetChild(i);
                var tmp = t.GetComponent<TMP_Text>();
                if (tmp)
                    return tmp;
            }
            return null;
        }

        // —— 辅助：找名为 "option" 的容器（大小写不敏感；先严格等名再模糊包含） ——
        private Transform FindOptionContainer(Transform panel)
        {
            int c = panel.childCount;
            // 先找直系等名
            for (int i = 0; i < c; i++)
            {
                var t = panel.GetChild(i);
                if (string.Equals(t.name, "option", StringComparison.OrdinalIgnoreCase))
                    return t;
            }
            // 再找直系包含“option”的
            for (int i = 0; i < c; i++)
            {
                var t = panel.GetChild(i);
                if (t.name != null && t.name.ToLower().Contains("option"))
                    return t;
            }
            // 再不行就从整棵子树里找等名
            var found = panel.Find("option") ?? panel.Find("Option");
            return found;
        }

        // —— 动画：把 target.localScale.y 从 from → to（线性） ——
        private IEnumerator ScaleY(
            RectTransform target,
            float from,
            float to,
            float duration,
            Action onDone,
            Action afterClosed
        )
        {
            onDone?.Invoke();
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                float y = Mathf.Lerp(from, to, k);
                target.localScale = new Vector3(1f, y, 1f);
                yield return null;
            }
            target.localScale = new Vector3(1f, to, 1f);
            afterClosed?.Invoke();

            if (root && to != 1f)
            {
                root.SetActive(false);
            }
        }
    }
}
