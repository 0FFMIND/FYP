using TMPro;
using UnityEngine;
using Utils;

namespace MVC
{
    public class ChapterInfoTextCtl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mainText;

        // 由按钮点击调用：切换当前要显示的章节文本
        public void SetChapter(int chapterIndex)
        {
            string folder = $"Chapter{chapterIndex}";

            // 根据章节号选择对应的 TextBuilder
            ChapterTextBuilderBase builder = chapterIndex switch
            {
                1 => new Chapter1TextBuilder(folder),
                _ => null
            };
            // 生成最终文本并赋值到 TMP
            mainText.text = builder.Build();
        }
    }
}
