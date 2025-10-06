using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    [Serializable]
    public class InteractModel
    {
        [SerializeField]
        public string[] lines;

        [Header("第二段")]
        [SerializeField]
        public string[] secondLines;

        [SerializeField]
        public LineMapping[] mappings;

        [Header("标记")]
        [SerializeField]
        public bool isImportant = false; // 重要交互（主线/关键）

        [NonSerialized]
        public bool isTalked = false; // 是否已完整聊过（运行时置位）
    }
}
