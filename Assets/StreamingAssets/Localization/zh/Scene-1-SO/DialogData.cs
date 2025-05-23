using UnityEngine;

namespace MVC
{
    [System.Serializable]
    public struct Choice
    {
        [Tooltip("ѡ��������ʾ")]
        public string text;
        [Tooltip("ѡ�к���ת���Ľڵ� ID")]
        public int targetNodeId;
    }

    [System.Serializable]
    public class DialogueNode
    {
        [Tooltip("�ڵ�� ID����Ψһ")]
        public int nodeId;
        [Tooltip("������ʾ���ı�")]
        [TextArea(2, 5)]
        public string[] lines;
        [Tooltip("���ڵ������չʾ�ķ�֧ѡ�Ϊ�����Զ�������һ���ڵ�ID")]
        public Choice[] choices;
    }
}
