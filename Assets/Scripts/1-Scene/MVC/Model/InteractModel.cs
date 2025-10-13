using System;
using UnityEngine;
using UnityEngine.Events;

namespace MVC
{
    [Serializable]
    public class InteractModel
    {
        [Header("从第几次起生效（含），默认0，对应区间 [visit, +∞)")]
        public int visit = 0;

        [SerializeField]
        public string[] lines;

        [Header("第二段")]
        [SerializeField]
        public string[] secondLines;

        [SerializeField]
        public LineMapping[] mappings;

        [Header("回调事件")]
        [SerializeField]
        public UnityEvent onInteractEnd; // 回调事件
    }
}
