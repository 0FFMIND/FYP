using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MVC
{
    /// <summary>
    /// 日记纯数据模型：解析 DTO、管理状态机、产出快照；不依赖 Unity API。
    /// </summary>
    [Serializable]
    public class JournalModel
    {
        private static readonly DateTime DEFAULT_TIME = DateTime.MinValue;

        // 运行时的日记条目顺序表：保持加载时（或追加时）的顺序
        private readonly List<JournalItem> _items = new();

        // 只读访问
        public IReadOnlyList<JournalItem> RawItems => _items;

        // key → JournalItem 的索引表
        private readonly Dictionary<string, JournalItem> _byKey = new(StringComparer.Ordinal);

        // 从顶层数组 JSON 解析（JournalItemDTO[]），所有项初始 Hidden，createdAt=DateTime.MinValue
        public void LoadFromJsonArray(string jsonArray, Func<string, JournalItemDTO[]> arrayParser)
        {
            if (arrayParser == null)
                throw new ArgumentNullException(nameof(arrayParser));
            var dtos = arrayParser(jsonArray) ?? Array.Empty<JournalItemDTO>();
            _items.Clear();
            _byKey.Clear();

            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var dto in dtos)
            {
                if (dto == null || string.IsNullOrEmpty(dto.key))
                    continue;
                if (!seen.Add(dto.key))
                    continue;

                var it = new JournalItem
                {
                    key = dto.key,
                    title = string.IsNullOrEmpty(dto.title) ? dto.key : dto.title,
                    content = dto.content ?? string.Empty,
                    status = JournalStatus.Hidden,
                    createdAt = DEFAULT_TIME,
                };
                _items.Add(it);
                _byKey[it.key] = it;
            }
        }

        // 通过 key 查找JounralItem
        public JournalItem Find(string key)
        {
            return key != null && _byKey.TryGetValue(key, out var v) ? v : null;
        }
    }
}
