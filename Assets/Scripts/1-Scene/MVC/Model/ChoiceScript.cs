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
        [Tooltip("解锁此选项前必须访问过的节点 ID 列表；为空则无前置要求")]
        public int[] prereqNodeIds;
    }

    [System.Serializable]
    public class ChoiceNode
    {
        [Tooltip("节点的 ID，需唯一")]
        public int nodeId;
        [Tooltip("此选项播放完毕后要跳转到的之前的节点 ID，留-1表示invalid")]
        public int postNodeId;
        [Tooltip("此选项播放完毕后要跳转到的之后的节点 ID，留-1表示invalid")]
        public int nextNodeId;
        [Tooltip("对话文本文件名（不含扩展名），由 DialogueModel 读取实际文本）")]
        public string dialogueTxt;
        [Tooltip("Choice上的提示文字")]
        public string choicesTxt;
        [Tooltip("可选分支；为空则本节点视为“叶子”，读取完毕后返回上级菜单")]
        public Choice[] choices;
    }

    [CreateAssetMenu(menuName = "Dialogue/Dialogue Script", fileName = "New Dialogue")]
    public class ChoiceScript : ScriptableObject
    {
        [Tooltip("对话节点列表；节点 ID 不必连续，但要保证唯一")]
        public ChoiceNode[] nodes;
    }
}
