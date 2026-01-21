using System;
using System.Collections.Generic;
using Manager;

namespace MVC
{
    public sealed class Chapter1TextBuilder : ChapterTextBuilderBase
    {
        public Chapter1TextBuilder(string folder) : base(folder) { }

        protected override IReadOnlyList<Segment> BuildSegments()
        {
            // 从存档/设置里读取进度状态
            bool chapter1Completed = SettingsMgr.Instance.GetChapter1Completed();
            bool hiddenCompleted = SettingsMgr.Instance.GetChapter1HiddenCompleted();

            // 每段的可见性由 bool 条件控制；不可见时基类会按行数替换为 <???>
            return new List<Segment>
            {
                new Segment("Info.txt",   true),
                new Segment("1.txt",      chapter1Completed),
                new Segment("Hidden.txt", hiddenCompleted),
                new Segment("2.txt",      chapter1Completed),
            };
        }
    }
}
