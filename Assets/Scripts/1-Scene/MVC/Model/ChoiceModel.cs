using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVC
{
    // 这个model用来读取so文件，作为so数据的临时储存
    public class ChoiceModel
    {
        // 私有只读字段
        private readonly Dictionary<int, ChoiceNode> nodeDict;
        // 公共只读字段，保存对话的第一个（起始）节点 ID
        public readonly int startNodeId;
        public ChoiceModel(ChoiceScript script)
        {
            // Linq
            nodeDict = script.nodes
                .ToDictionary(
                    n => n.nodeId,  // key：节点的唯一 ID
                    n => n          // value：这个节点本身
                );
            // 取起始节点 ID：如果数组非空，就取第 0 个元素的 nodeId；否则默认 0
            startNodeId = script.nodes.Length > 0
                ? script.nodes[0].nodeId
                : 0;
        }
        // 通过字典查找
        public ChoiceNode GetNode(int nodeId)
        {
            return nodeDict.TryGetValue(nodeId, out var node) ? node : null;
        }
        // 动态创建/返回dialogmodel
        public DialogueModel GetDialogueModel(int nodeId)
        {
            var node = GetNode(nodeId);
            return node != null
                ? new DialogueModel(node.dialogueTxt)
                : null;
        }
    }
}

