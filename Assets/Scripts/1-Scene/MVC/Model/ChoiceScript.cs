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
        [HideInInspector]
        public bool visited;    // 运行时：此分支对应的节点是否已被访问过
    }

    [System.Serializable]
    public class ChoiceNode
    {
        [Tooltip("节点的 ID，需唯一")]
        public int nodeId;
        [Tooltip("此选项播放完毕后要跳转到的节点 ID；留 -1 则结束对话")]
        public int postNodeId;
        [Tooltip("对话文本文件名（不含扩展名），由 DialogueModel 读取实际文本）")]
        public string dialogueTxt;
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
