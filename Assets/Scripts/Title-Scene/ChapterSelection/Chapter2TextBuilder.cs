using System;
using System.Collections.Generic;
using Manager;

namespace MVC
{
    public sealed class Chapter2TextBuilder : ChapterTextBuilderBase
    {
        public Chapter2TextBuilder(string folder) : base(folder) { }

        protected override IReadOnlyList<Segment> BuildSegments()
        {
            // 从存档/设置里读取进度状态
            bool chapter2Completed = SettingsMgr.Instance.GetChapter2Completed();

            // 每段的可见性由 bool 条件控制；不可见时基类会按行数替换为 <???>
            return new List<Segment>
            {
                new Segment("Info.txt",   true),
                new Segment("1.txt",      chapter2Completed),
                new Segment("2.txt",      chapter2Completed),
            };
        }
    }
}
