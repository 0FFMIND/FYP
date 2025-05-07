using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVC.View;
using MVC.Model;
using InputSystem;

namespace MVC.Controller
{
    public class DialogueCtl : MonoBehaviour
    {
        [SerializeField]
        private DialogueView view;
        [SerializeField]
        private Sprite[] sprites;
        //
        private int index;
        private DialogueModel model;
        private void Start()
        {
            // 清空现在有的内容
            view.Render(null, null);
            // 读取text
            model = new DialogueModel("1-Scene-1.txt");
            // 刷新index
            index = 0;
            // 注册事件
            InputManager.Instance.OnAction += OnInputAction;
        }
        private void OnInputAction(InputAction action)
        {
            if(action == InputAction.DialogueClick)
            {
                NextLine();
            }
        }
    }
}

