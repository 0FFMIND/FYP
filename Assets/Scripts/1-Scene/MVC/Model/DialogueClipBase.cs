using System;
using UnityEngine;

namespace MVC
{
    /// <summary>
    /// 所有对话片段共有的部分：文本 + 行映射
    /// </summary>
    [Serializable]
    public class DialogueClipBase
    {
        [Tooltip("对话文本资源（可以是 TextAsset 名称 / 地址等）")]
        public string textFile;

        [Tooltip("每一行对应的立绘/事件映射")]
        public LineMapping[] mappings;
    }
}
