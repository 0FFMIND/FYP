using System;

namespace MVC
{
    // 日记条目状态枚举
    public enum JournalStatus
    {
        Hidden, // 未显示（玩家还看不到）
        Active, // 已显示在清单上（可作为当前目标）
        Completed, // 已完成（打钩/置灰）
    }

    // 游戏中运行的数据，用来放入SaveData里面的
    [Serializable]
    public class JournalItem
    {
        // 任务唯一键（程序用来识别/去重/定位）
        public string key;

        // 在 UI 上展示的文案，如 "安全抵达天台"
        public string title;

        // 详情
        public string content;

        // 当前状态：Pending/Active/Done，用于渲染勾选、排序等
        public JournalStatus status;

        // 创建时间戳（可用于按时间排序、调试或统计）
        public DateTime createdAt;
    }

    // 仅用于读取 JSON 的轻量条目：含 key, title 和 content
    [Serializable]
    public class JournalItemDTO
    {
        public string key;
        public string title;
        public string content;
    }
}
