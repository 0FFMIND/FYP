using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 字符串调试工具：
    /// - 控制字符保持为真实字符（\n 会真的换行）
    /// - 可选：打断 Unity 富文本标签（如 <color>），避免 Console 解析
    /// </summary>
    public static class DebugText
    {
        // 用于打断 Unity 富文本标签解析：插在 '<' 后面
        private const char Zws = '\u200B';

        /// <summary>
        /// 将字符串转为“可打印文本”用于 Debug.Log：
        /// - 默认保持真实控制字符（\n 会真的换行）
        /// - 默认打断富文本标签：<color=...> 会输出为 "<​color=...>"（中间插入零宽字符）
        /// </summary>
        public static string Escape(string s, int maxLen = 2048, bool breakRichText = true)
        {
            if (s == null) return "<null>";

            int len = s.Length;
            int n = Math.Min(len, maxLen);

            var sb = new StringBuilder(n + 32);

            for (int i = 0; i < n; i++)
            {
                char c = s[i];
                switch (c)
                {
                    // 保持真实控制字符：日志里会实际换行/回车/制表
                    case '\n': sb.Append('\n'); break;
                    case '\r': sb.Append('\r'); break;
                    case '\t': sb.Append('\t'); break;
                    case '\0': sb.Append('\0'); break;
                    case '\b': sb.Append('\b'); break;
                    case '\f': sb.Append('\f'); break;
                    case '\v': sb.Append('\v'); break;

                    // 打断 Unity 富文本：避免 <color> / <b> / <sprite> 等被解析
                    case '<':
                        if (breakRichText) sb.Append('<').Append(Zws);
                        else sb.Append('<');
                        break;

                    // 可选：也可以在 '>' 前插 ZWS（不是必须，主要靠 '<' 打断即可）
                    case '>':
                        if (breakRichText) sb.Append(Zws).Append('>');
                        else sb.Append('>');
                        break;

                    case '\\': sb.Append('\\'); break;
                    case '\"': sb.Append('\"'); break;

                    default:
                        // 其他不可见控制字符：用 \uXXXX 显示
                        if (char.IsControl(c))
                        {
                            sb.Append("\\u").Append(((int)c).ToString("X4"));
                            break;
                        }

                        // surrogate pair（emoji 等）
                        if (char.IsHighSurrogate(c) && i + 1 < n && char.IsLowSurrogate(s[i + 1]))
                        {
                            int codePoint = char.ConvertToUtf32(c, s[i + 1]);
                            sb.Append(char.ConvertFromUtf32(codePoint));
                            i++;
                            break;
                        }

                        sb.Append(c);
                        break;
                }
            }

            if (len > maxLen)
            {
                sb.Append("…");
                sb.Append($"(truncated {len - maxLen})");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 直接输出：带长度 + 内容（默认打断富文本标签，避免 Console 解析）。
        /// </summary>
        public static void Log(string s, int maxLen = 2048, string tag = "DebugText", bool breakRichText = true)
        {
            int length = (s == null) ? -1 : s.Length;
            Debug.Log($"[{tag}] len={length} => \"{Escape(s, maxLen, breakRichText)}\"");
        }

        /// <summary>
        /// 输出 Unicode code points（用于排查“肉眼相同但编码不同”的字符）。
        /// </summary>
        public static string CodePoints(string s, int maxPoints = 128)
        {
            if (s == null) return "<null>";

            var sb = new StringBuilder();
            int count = 0;

            for (int i = 0; i < s.Length && count < maxPoints; i++)
            {
                int codePoint;

                if (char.IsHighSurrogate(s[i]) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1]))
                {
                    codePoint = char.ConvertToUtf32(s[i], s[i + 1]);
                    i++;
                }
                else
                {
                    codePoint = s[i];
                }

                if (count > 0) sb.Append(' ');
                sb.Append("U+").Append(codePoint.ToString("X"));
                count++;
            }

            if (s.Length > 0 && count >= maxPoints) sb.Append(" …");
            return sb.ToString();
        }

        /// <summary>
        /// 把 string[] 打印成 ["a","b","c"] 的形式，并对每个元素做 Escape（可打断 rich text）。
        /// </summary>
        public static string FormatStringArray(string[] arr, int maxItemLen = 512, bool breakRichText = true)
        {
            if (arr == null) return "<null>";

            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append('"').Append(Escape(arr[i], maxItemLen, breakRichText)).Append('"');
            }
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// 打印 Dictionary<string, string[]> 的所有 pair。
        /// </summary>
        public static void LogTablePairs(
            IDictionary<string, string[]> dict,
            string tag = "table")
        {
            if (dict == null)
            {
                Debug.Log($"[{tag}] <null>");
                return;
            }

            foreach (var kv in dict)
            {
                Debug.Log($"[{tag}] key=\"{Escape(kv.Key)}\" => {FormatStringArray(kv.Value)}");
            }
        }
    }
}
