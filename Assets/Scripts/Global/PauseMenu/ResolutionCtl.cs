using System;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace MVC
{
    public class ResolutionCtl : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentResText;

        // 只按宽高去重后的分辨率列表（升序）
        private List<Vector2Int> _resList = new List<Vector2Int>();
        private int _index;

        private void Awake()
        {
            // 若未在 Inspector 赋值
            if (!currentResText)
            {
                Debug.LogError($"[ResolutionCtl] 未挂载 TMP_Text（{name}）");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            // 构建并排序分辨率列表
            BuildResList();
            var (savedW, savedH) = SettingsMgr.Instance.GetResolution();
            // 找到与保存分辨率最接近的列表索引
            _index = FindIndexClosest(savedW, savedH);
            // 刷新 UI 文本
            ApplyAndUpdateText(applyNow: false);
        }

        public void SwitchLeft()
        {
            if (_resList.Count == 0)
                return;
            _index = (_index - 1 + _resList.Count) % _resList.Count;
            ApplyAndUpdateText(applyNow: true);
        }

        public void SwitchRight()
        {
            if (_resList.Count == 0)
                return;
            _index = (_index + 1) % _resList.Count;
            ApplyAndUpdateText(applyNow: true);
        }

        // ========== 内部实现 ==========

        private void BuildResList()
        {
            _resList.Clear();
            Vector2Int[] common169 =
            {
                new Vector2Int(1280, 720), // 720p
                new Vector2Int(1366, 768), // 常见笔记本面板，近似 16:9
                new Vector2Int(1920, 1080), // 1080p
                new Vector2Int(2560, 1440), // 1440p
            };

            // 去重（防止你以后重复添加）
            var set = new HashSet<string>();
            foreach (var v in common169)
            {
                string key = $"{v.x}x{v.y}";
                if (set.Add(key))
                    _resList.Add(v);
            }

            // 升序排序（先宽后高）
            _resList.Sort(
                (a, b) =>
                {
                    int cmp = a.x.CompareTo(b.x);
                    return cmp != 0 ? cmp : a.y.CompareTo(b.y);
                }
            );
        }

        private int FindIndexClosest(int w, int h)
        {
            // 精确匹配优先
            int exact = _resList.FindIndex(v => v.x == w && v.y == h);
            if (exact >= 0)
                return exact;

            // 否则找最接近的（欧氏距离）
            float best = float.MaxValue;
            int idx = 0;
            for (int i = 0; i < _resList.Count; i++)
            {
                var v = _resList[i];
                float d = (v.x - w) * (v.x - w) + (v.y - h) * (v.y - h);
                if (d < best)
                {
                    best = d;
                    idx = i;
                }
            }
            return idx;
        }

        private void ApplyAndUpdateText(bool applyNow)
        {
            // 应用当前分辨率，并通知系统更新
            var v = _resList[_index];

            // 显示文本
            currentResText.text = $"{v.x} × {v.y}";

            if (!applyNow)
                return;

            SettingsMgr.Instance.SetResolution(v.x, v.y);
        }
    }
}
