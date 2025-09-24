using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    [Serializable]
    public class Line
    {
        [TextArea]
        public string content;
    }

    [Serializable]
    public class InteractModel
    {
        [SerializeField]
        public List<Line> lines = new();
        private int _idx = 0;

        public string Current => (_idx >= 0 && _idx < lines.Count) ? lines[_idx].content : null;
        public bool IsFinished => _idx >= lines.Count || lines.Count == 0;

        public void Reset()
        {
            _idx = 0;
        }

        // 前进一行；返回 true 表示仍有内容，false 表示已结束
        public bool Next()
        {
            if (IsFinished)
            {
                return false;
            }
            _idx++;
            return !IsFinished;
        }
    }
}
