using System;
using UnityEngine.Events;

namespace MVC
{
    [Serializable] // 允许在 Inspector 序列化（主要用于 label；Action 不可序列化）
    public struct ChoiceData
    {
        public string label; // 按钮展示的文案
        public UnityEvent onClick; // 点击时的回调（运行时注入，不参与序列化）

        public ChoiceData(string label, UnityEvent onClick)
        {
            this.label = label;
            this.onClick = onClick;
        }
    }

    [Serializable] // 允许在 Inspector 序列化（主要用于 label；Action 不可序列化）
    public struct ChoiceModel
    {
        public ChoiceData[] items;
        public string choiceHeader;
        public int choicePanel;

        public ChoiceModel(ChoiceData[] items, string choiceHeader, int choicePanel)
        {
            this.items = items;
            this.choiceHeader = choiceHeader;
            this.choicePanel = choicePanel;
        }
    }
}
