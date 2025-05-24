using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVC
{
    // ���model������ȡso�ļ�����Ϊso���ݵ���ʱ����
    public class ChoiceModel
    {
        // ˽��ֻ���ֶ�
        private readonly Dictionary<int, ChoiceNode> nodeDict;
        // ����ֻ���ֶΣ�����Ի��ĵ�һ������ʼ���ڵ� ID
        public readonly int startNodeId;
        public ChoiceModel(ChoiceScript script)
        {
            // Linq
            nodeDict = script.nodes
                .ToDictionary(
                    n => n.nodeId,  // key���ڵ��Ψһ ID
                    n => n          // value������ڵ㱾��
                );
            // ȡ��ʼ�ڵ� ID���������ǿգ���ȡ�� 0 ��Ԫ�ص� nodeId������Ĭ�� 0
            startNodeId = script.nodes.Length > 0
                ? script.nodes[0].nodeId
                : 0;
        }
        // ͨ���ֵ����
        public ChoiceNode GetNode(int nodeId)
        {
            return nodeDict.TryGetValue(nodeId, out var node) ? node : null;
        }
        // ��̬����/����dialogmodel
        public DialogueModel GetDialogueModel(int nodeId)
        {
            var node = GetNode(nodeId);
            return node != null
                ? new DialogueModel(node.dialogueTxt)
                : null;
        }
    }
}

