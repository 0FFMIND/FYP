using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Manager;

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
            // （下面的 GetText 调用会在内部加载 strings.json）
            LocalizationMgr.Instance.GetText("");

            // 把每个 SO 里的 ChoiceNode 转成本地化后的 LocalizedChoiceNode
            nodeDict = script.nodes
                .ToDictionary(
                    n => n.nodeId,
                    n =>
                    {
                        // 本地化 choicesTxt
                        var locChoicesTxt = LocalizationMgr.Instance.GetText(n.choicesTxt);

                        // 本地化每个分支选项
                        var locChoices = n.choices?
                            .Select(c => new Choice
                            {
                                text = LocalizationMgr.Instance.GetText(c.text),
                                targetNodeId = c.targetNodeId,
                                prereqNodeIds = c.prereqNodeIds
                            })
                            .ToArray()
                            ?? new Choice[0];

                        return new ChoiceNode
                        {
                            nodeId = n.nodeId,
                            postNodeId = n.postNodeId,
                            nextNodeId = n.nextNodeId,
                            choicesTxt = locChoicesTxt,
                            choices = locChoices,
                            dialogueTxt = n.dialogueTxt
                        };
                    }
                );

            startNodeId = script.nodes.Length > 0
                ? script.nodes[0].nodeId
                : -1;
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

