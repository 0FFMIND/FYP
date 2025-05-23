using UnityEngine;

namespace MVC
{
    [System.Serializable]
    public struct Choice
    {
        [Tooltip("选项文字显示")]
        public string text;
        [Tooltip("选中后跳转到的节点 ID")]
        public int targetNodeId;
    }

    [System.Serializable]
    public class DialogueNode
    {
        [Tooltip("节点的 ID，需唯一")]
        public int nodeId;
        [Tooltip("逐字显示的文本")]
        [TextArea(2, 5)]
        public string[] lines;
        [Tooltip("本节点结束后展示的分支选项；为空则自动跳到下一个节点ID")]
        public Choice[] choices;
    }
}
