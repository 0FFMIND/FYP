using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public class ChapterLaunchButtonCtl : MonoBehaviour
    {
        [SerializeField] private Button launchButton;


        // 由外部Button调用：传入当前选中的章节号
        public void SetChapter(int index)
        {
            Rebind(index);
        }

        // 根据章节号绑定对应的启动函数
        private void Rebind(int chapterIndex)
        {
            if (!launchButton) return;

            // 先移除本脚本可能绑定的所有章节函数，避免重复叠加
            launchButton.onClick.RemoveListener(StartChapter1);
            launchButton.onClick.RemoveListener(StartChapter2);
            launchButton.onClick.RemoveListener(StartChapterDefault);

            // 再按当前章节号，绑定唯一对应的函数
            switch (chapterIndex)
            {
                case 1: launchButton.onClick.AddListener(StartChapter1); break;
                case 2: launchButton.onClick.AddListener(StartChapter2); break;
                default: launchButton.onClick.AddListener(StartChapterDefault); break;
            }
        }

        // 各章节的专属启动行为

        private void StartChapter1()
        {
            // 重置章节1进度
            SettingsMgr.Instance.ClearChapter1Progress();
            // 停止当前BGM
            AudioMgr.Instance.StopBGM();
            // 进入1-Scene-UI
            EventBus.Publish(
                new ESceneFade(
                    toScene: "1-Scene-UI",
                    fadeOutDuration: 0.5f,
                    fadeInDuration: 1f
                )
            );
        }

        private void StartChapter2()
        {
            // 停止当前BGM
            AudioMgr.Instance.StopBGM();
            // 进入2-Scene-UI
            EventBus.Publish(
                new ESceneFade(
                    toScene: "2-Scene-UI",
                    fadeOutDuration: 0.5f,
                    fadeInDuration: 1f
                )
            );
        }

        private void StartChapterDefault()
        {
            Debug.LogWarning($"[ChapterStartButtonCtl]: unsupported chapterIndex");
        }
    }
}
