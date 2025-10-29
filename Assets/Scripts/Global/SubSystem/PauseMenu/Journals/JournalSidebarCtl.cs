using System;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public class JournalSidebarCtl : MonoBehaviour
    {
        [SerializeField]
        private Transform viewRoot;

        [SerializeField]
        private JournalItemView itemPrefab;

        [SerializeField]
        private ToggleGroup toggleGroup;

        // 当前选中条目的 key
        private string _selectedKey;

        private void OnEnable()
        {
            EventBus.Subscribe<EJournalSelected>(OnJournalSelected);
            Rebuild();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EJournalSelected>(OnJournalSelected);
        }

        private void OnJournalSelected(EJournalSelected e)
        {
            _selectedKey = e.Key;
        }

        // 重建侧边栏
        public void Rebuild()
        {
            if (!viewRoot || !itemPrefab || !toggleGroup)
            {
                Debug.LogError("[JournalSidebarCtl] 请绑定 viewRoot / itemPrefab / toggleGroup");
                return;
            }
            // 清空,销毁每个旧行的 GameObject
            for (int i = viewRoot.childCount - 1; i >= 0; i--)
                Destroy(viewRoot.GetChild(i).gameObject);

            var m = JournalMgr.Instance?.Model;
            if (m == null)
                return;

            // 将原始集合转为可 LINQ 枚举
            var all = m.RawItems.AsEnumerable();
            // 取 Active 组
            var active = all.Where(it => it.status == JournalStatus.Active);
            // 取 Completed 组
            var completed = all.Where(it => it.status == JournalStatus.Completed);

            // Active / Completed 各自排序（MinValue 视作“无时间”，放到该组的最后）
            active = active.OrderBy(it =>
                it.createdAt == DateTime.MinValue ? DateTime.MaxValue : it.createdAt
            );
            completed = completed.OrderBy(it =>
                it.createdAt == DateTime.MinValue ? DateTime.MaxValue : it.createdAt
            );

            // 组合顺序：Active 一定在 Completed 上面
            var src = active.Concat(completed);
            // 物化为列表，后续多次使用
            var list = src.ToList();

            // —— 设定默认选中：若当前未选或选中项不在列表中，则默认第一个 ——
            if (string.IsNullOrEmpty(_selectedKey) || !list.Any(it => it.key == _selectedKey))
            {
                // 有数据则取首个 key，否则保持 null
                _selectedKey = list.Count > 0 ? list[0].key : null;
            }
            JournalItemView selectedRow = null;
            foreach (var it in src) // 逐条实例化行项
            {
                // 在容器下生成一行
                var row = Instantiate(itemPrefab, viewRoot);
                // 用 key 命名，便于后续反查
                row.name = $"JournalTitle_{it.key}";
                // 让行加入同一 ToggleGroup
                row.Bind(it, toggleGroup);
                // 统一先静默为 false
                row.SetSelected(false, notify: false);
                if (it.key == _selectedKey)
                    selectedRow = row;
            }
            // 最后“带通知”点亮默认选中项（触发视觉 & 事件 & 组内互斥）
            if (selectedRow != null)
            {
                selectedRow.SetSelected(true, notify: true);
            }
        }
    }
}
