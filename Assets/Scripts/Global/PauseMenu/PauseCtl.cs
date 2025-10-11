using System;
using UnityEngine;

namespace MVC
{
    public class PauseCtl : MonoBehaviour
    {
        private PauseView _view;

        private void Awake()
        {
            _view = GetComponent<PauseView>();
        }

        public void Show()
        {
            _view.Show();
        }

        public void Hide(Action callback)
        {
            _view.Hide(callback);
        }
    }
}
