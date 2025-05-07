using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MVC.Model
{
    // 不继承mono，则可以通过new T()出来
    public class DialogueModel
    {
        // 储存的对话内容，
        public string[] Lines { get; private set; }
        public DialogueModel(string fileName)
        {
            LoadDialogue(fileName);
        }
        // 读取对话文本
        private void LoadDialogue(string fileName)
        {
            string path = LocalizationMgr.Instance.GetDialoguePath(fileName);
            Lines = File.ReadAllLines(path);
        }
    }

}
