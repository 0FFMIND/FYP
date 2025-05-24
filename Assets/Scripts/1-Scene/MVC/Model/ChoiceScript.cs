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
        [Tooltip("������ѡ��ǰ������ʹ��Ľڵ� ID �б�Ϊ������ǰ��Ҫ��")]
        public int[] prereqNodeIds;
        [HideInInspector]
        public bool visited;    // ����ʱ���˷�֧��Ӧ�Ľڵ��Ƿ��ѱ����ʹ�
    }

    [System.Serializable]
    public class ChoiceNode
    {
        [Tooltip("�ڵ�� ID����Ψһ")]
        public int nodeId;
        [Tooltip("��ѡ�����Ϻ�Ҫ��ת���Ľڵ� ID���� -1 ������Ի�")]
        public int postNodeId;
        [Tooltip("�Ի��ı��ļ�����������չ�������� DialogueModel ��ȡʵ���ı���")]
        public string dialogueTxt;
        [Tooltip("��ѡ��֧��Ϊ���򱾽ڵ���Ϊ��Ҷ�ӡ�����ȡ��Ϻ󷵻��ϼ��˵�")]
        public Choice[] choices;
    }

    [CreateAssetMenu(menuName = "Dialogue/Dialogue Script", fileName = "New Dialogue")]
    public class ChoiceScript : ScriptableObject
    {
        [Tooltip("�Ի��ڵ��б��ڵ� ID ������������Ҫ��֤Ψһ")]
        public ChoiceNode[] nodes;
    }
}
