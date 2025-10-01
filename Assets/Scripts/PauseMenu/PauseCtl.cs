using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class PauseCtl : MonoBehaviour
    {
        private PauseView _view;
        // 当前页索引
        private int _index;
        private void Awake()
        {
            _view = GetComponent<PauseView>();
            _index = 0;
        }
        private void OnEnable()
        {
            // 组件启用时刷新并显示当前页
            _view.ShowPage(_index);
        }
        public void SwitchLeft()
        {
            // 向左切换界面（上一个）
            _index = (_index - 1 + _view.PageCount) % _view.PageCount;
            _view.ShowPage(_index);
        }

        public void SwitchRight()
        {
            // 向右切换界面（下一个）
            _index = (_index + 1) % _view.PageCount;
            _view.ShowPage(_index);
        }
    }
}
