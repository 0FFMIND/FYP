using System.IO;
using System.Linq;
using Manager;
using UnityEngine.SceneManagement;

namespace MVC
{
    // 不继承mono，则可以通过new T()出来
    public class DialogueModel
    {
        // 原始 key（逐行读取后保存，随语言变化再翻译）
        private string[] _keys;

        // 翻译后的对话内容
        public string[] Lines { get; private set; }

        // 从文件读取构造文本
        public DialogueModel(string fileName)
        {
            // 拿到当前场景名
            string sceneName = SceneManager.GetActiveScene().name;
            string path = LocalizationMgr.Instance.GetDialoguePath(sceneName, fileName);
            // 先把文件的每一行都当作一个 key 读进来
            _keys = File.ReadAllLines(path);
            LoadDialogue(_keys);
        }

        // 从字符串数字组构造文本
        public DialogueModel(string[] lines)
        {
            _keys = lines;
            LoadDialogue(_keys);
        }

        // 读取对话文本
        private void LoadDialogue(string[] keys)
        {
            // 把每个 key 传给 LocalizationMgr，返回当前语言对应的文本
            Lines = keys.Select(k =>
                {
                    string txt = LocalizationMgr.Instance.GetText(k.Trim());
                    return txt;
                })
                .ToArray();
        }

        // 对外提供的重译入口：使用已缓存的 _keys 重新生成 Lines
        public void Reload()
        {
            if (_keys == null)
                return;
            LoadDialogue(_keys);
        }
    }
}
