using System;

namespace MVC
{
    public enum JournalStatus
    {
        Pending,
        Active,
        Done,
    } // 日记条目状态枚举：待办 / 激活(当前目标) / 已完成

    [Serializable]
    public class JournalItem
    {
        // 任务唯一键（程序用来识别/去重/定位）
        public string key;

        // 在 UI 上展示的文案，如 "安全抵达天台"
        public string title;

        // 当前状态：Pending/Active/Done，用于渲染勾选、排序等
        public JournalStatus status;

        // 创建时间戳（可用于按时间排序、调试或统计）
        public DateTime createdAt;
    }
}
