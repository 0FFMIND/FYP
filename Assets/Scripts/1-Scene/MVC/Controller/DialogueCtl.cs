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
            // ��������е�����
            view.Render(null, null);
            // ��ȡtext
            model = new DialogueModel("1-Scene-1.txt");
            // ˢ��index
            index = 0;
            // ע���¼�
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

