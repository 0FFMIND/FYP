using System;
using UnityEngine;
using UnityEngine.Events;

namespace MVC
{
    [Serializable]
    public class InteractEndEvent : UnityEvent<InteractCtl> { }

    [Serializable]
    public class InteractModel
    {
        [Header("从第几次起生效（含），默认0，对应区间 [visit, +∞)")]
        public int visit = 0;

        [Header("第一段")]
        [SerializeField]
        [TextArea]
        public string[] firstLines;

        [Header("第二段")]
        [SerializeField]
        [TextArea]
        public string[] secondLines;

        [SerializeField]
        public LineMapping[] secondMappings;

        [Header("选项")]

        [SerializeField]
        public ChoiceModel choiceModel;

        [Header("回调事件")]
        [SerializeField]
        public InteractEndEvent onInteractEnd;
    }
}
