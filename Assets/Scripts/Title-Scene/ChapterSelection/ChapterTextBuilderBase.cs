using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC
{
    public abstract class ChapterTextBuilderBase
    {
        // 统一传入 folder（例如 "Title-Scene/Chapter1"）
        protected readonly string folder;

        protected ChapterTextBuilderBase(string folder = null)
        {
            this.folder = folder;
        }

        // 子类提供：段落定义（顺序就是最终拼接顺序）
        protected abstract IReadOnlyList<Segment> BuildSegments();

        public string Build()
        {
            var segments = BuildSegments();
            var allLines = new List<string>(256);

            int visibleCount = 0;
            for (int i = 0; i < segments.Count; i++)
                if (segments[i].IsVisible) visibleCount++;

            // 如果仅有一个段可见（通常是 Info），说明本章节尚未完整游玩
            if (segments.Count > 1 && visibleCount == 1)
            {
                allLines.AddRange(LoadLines(segments[0].FileName));
                allLines.AddRange(LoadLinesWithoutFolder("IncompleteHint.txt"));
                return string.Join("\n", allLines);
            }

            // 用 for 便于判断“是否最后一段”，从而决定要不要插入空行
            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                bool visible = seg.IsVisible;

                if (visible)
                {
                    // 该段已解锁：正常加载并追加所有行
                    var segLines = LoadLines(seg.FileName);
                    if (segLines != null && segLines.Length > 0)
                        allLines.AddRange(segLines);
                }
                else
                {
                    // 该段未解锁：只追加一行提示
                    allLines.AddRange(LoadLinesWithoutFolder("LockedHint.txt"));
                }

                // 段落分隔：每段后插入一个空行（最后一段不插入）
                if (i < segments.Count - 1)
                    allLines.Add(string.Empty);
            }

            // 最终用 \n 拼接成一段文本，赋值给 TMP
            return string.Join("\n", allLines);
        }
        protected virtual string[] LoadLinesWithoutFolder(string fileName)
        {
            // 通过 TextModel 来加载文本
            var m = new TextModel(fileName);
            return m.Lines;
        }
        protected virtual string[] LoadLines(string fileName)
        {
            // 通过 TextModel 来加载文本
            var m = new TextModel(fileName, folder);
            return m.Lines;
        }

        protected readonly struct Segment
        {
            public readonly string FileName;
            public readonly bool IsVisible;

            public Segment(string fileName, bool isVisible = true)
            {
                FileName = fileName;
                IsVisible = isVisible;
            }
        }
    }
}
