using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

namespace MVC
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
            // 拿到当前场景名
            string sceneName = SceneManager.GetActiveScene().name;
            string path = LocalizationMgr.Instance.GetDialoguePath(sceneName, fileName);
            // 先把文件的每一行都当作一个 key 读进来
            string[] keys = File.ReadAllLines(path);
            // 再把每个 key 传给 LocalizationMgr，返回当前语言对应的文本
            Lines = keys
                .Select(k => {
                    string txt = LocalizationMgr.Instance.GetText(k.Trim());
                    return txt;
                })
                .ToArray();
        }
    }

}
