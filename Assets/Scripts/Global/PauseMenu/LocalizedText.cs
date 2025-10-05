using Manager;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Utils;

namespace MVC
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;

        // 运行期传入的“命名参数”，例如 { "action": "Sprint", "key": "Right Shift" }
        private Dictionary<string, string> _namedArgs = new Dictionary<string, string>();

        private TMP_Text tmpText;

        private void OnEnable()
        {
            EventBus.Subscribe<ELanguageChanged>(OnLang);
            Refresh();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ELanguageChanged>(OnLang);
        }

        private void Awake()
        {
            AutoWire();
            if (!tmpText)
            {
                Debug.LogWarning($"[LocalizedText] 缺少 Text/TMP_Text，已禁用: {name}");
                enabled = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning($"[LocalizedText] 缺少 key，已禁用: {name}");
                enabled = false;
                return;
            }
        }

        private void OnLang(ELanguageChanged e) => Refresh();

        public void Refresh()
        {
            tmpText.text = LocalizationMgr.Instance.GetText(key);

            if (_namedArgs.Count > 0)
            {
                string template = LocalizationMgr.Instance.GetText(key);
                var sb = new StringBuilder(template.Length + 32);

                int i = 0;
                while (i < template.Length)
                {
                    char c = template[i];

                    // 处理字面量 "{{" -> "{"
                    if (c == '{')
                    {
                        if (i + 1 < template.Length && template[i + 1] == '{')
                        {
                            sb.Append('{');
                            i += 2;
                            continue;
                        }

                        // 查找与之配对的 '}'；不支持嵌套
                        int close = template.IndexOf('}', i + 1);
                        if (close == -1)
                        {
                            // 没找到右括号：警告一次，并把剩余文本原样追加
                            Debug.LogWarning($"[LocalizedText] 模板花括号不匹配（缺少 '}}'）: 模板片段：\"{template.Substring(i)}\"");
                            sb.Append(template, i, template.Length - i);
                            break;
                        }

                        // 提取占位符名并 Trim 空白
                        string rawName = template.Substring(i + 1, close - i - 1);
                        string name = rawName.Trim();

                        if (name.Length == 0)
                        {
                            // 空占位符 "{ }"：提示并原样保留
                            Debug.LogError($"[LocalizedText] 空占位符：: 模板片段：\"{template.Substring(i)}\"，位置={i}。将保留 \"{{}}\" 原样。");
                            sb.Append('{').Append(rawName).Append('}');
                            i = close + 1;
                            continue;
                        }

                        // 查字典
                        if (_namedArgs.TryGetValue(name, out var value))
                        {
                            sb.Append(LocalizationMgr.Instance.GetText(value) ?? string.Empty);
                        }
                        else
                        {
                            // 缺少键：错误日志 + 不替换（保留原样）
                            Debug.LogError($"[LocalizedText] 模板缺少参数 需要 '{name}'，但未提供。占位符将保留原样。");
                            sb.Append('{').Append(rawName).Append('}');
                        }

                        i = close + 1;
                        continue;
                    }

                    // 处理字面量 "}}" -> "}"
                    if (c == '}' && i + 1 < template.Length && template[i + 1] == '}')
                    {
                        sb.Append('}');
                        i += 2;
                        continue;
                    }

                    // 普通字符
                    sb.Append(c);
                    i++;
                }
                tmpText.text = sb.ToString();
            }
        }

        // 自动查找同节点上的 TMP_Text 组件
        private void AutoWire()
        {
            if (!tmpText)
            {
                tmpText = GetComponent<TMP_Text>();
            }
        }
        public void SetKey(string key)
        {
            this.key = key;
        }

        // 传入命名参数字典，并立即刷新
        public void SetParams(Dictionary<string, string> dict)
        {
            _namedArgs = dict;
        }
    }
}
