using System.IO;
using System.Linq;
using System.Text;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVC
{
    // 不继承mono，则可以通过new T()出来
    public class TextModel
    {
        // 原始 key（逐行读取后保存，随语言变化再翻译）
        private string[] _keys;

        // 翻译后的对话内容
        public string[] Lines { get; private set; }

        // 从文件读取构造文本，接收可选的 folder 参数
        public TextModel(string fileName, string folder = null)
        {
            // 拿到当前场景名
            string sceneName = SceneManager.GetActiveScene().name;
            string finalName = string.IsNullOrEmpty(folder)
                ? fileName
                : Path.Combine(folder, fileName);
            string path = LocalizationMgr.Instance.GetDialoguePath(sceneName, finalName);
            // 不按行拆
            string raw = File.ReadAllText(path);
            // 统一换行
            raw = raw.Replace("\r\n", "\n");
            // 整段作为一个 key
            _keys = new[] { raw };
            LoadDialogue(_keys);
        }

        // 读取对话文本
        private void LoadDialogue(string[] keys)
        {
            // 把每个 key 传给 LocalizationMgr，返回当前语言对应的文本
            Lines = keys.Select(k =>
                {
                    string txt = LocalizationMgr.Instance.GetText(k, false);
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

        private static string EscapeForLog(string s)
        {
            if (s == null) return "<null>";

            var sb = new StringBuilder(s.Length + 32);
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\\': sb.Append(@"\\"); break;
                    case '\n': sb.Append(@"\n"); break;
                    case '\r': sb.Append(@"\r"); break;
                    case '\t': sb.Append(@"\t"); break;
                    case '\0': sb.Append(@"\0"); break;
                    default:
                        // 其他控制字符也显式输出
                        if (char.IsControl(c))
                            sb.Append(@"\u").Append(((int)c).ToString("X4"));
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
