using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MVC.Model
{
    // ���̳�mono�������ͨ��new T()����
    public class DialogueModel
    {
        // ����ĶԻ����ݣ�
        public string[] Lines { get; private set; }
        public DialogueModel(string fileName)
        {
            LoadDialogue(fileName);
        }
        // ��ȡ�Ի��ı�
        private void LoadDialogue(string fileName)
        {
            string path = LocalizationMgr.Instance.GetDialoguePath(fileName);
            Lines = File.ReadAllLines(path);
        }
    }

}
