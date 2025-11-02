using MVC;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Manager
{
    public class PauseMgr : SingletonMB<PauseMgr>
    {
        private PauseModel pauseModel;
        private GameObject pausePanel;
        private const string PauseMenuPath = "Prefabs/Global/PauseMenu";
        public bool canPause = true;

        private void Awake()
        {
            pauseModel = new PauseModel();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(InputAction.PauseGame, TogglePause);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(InputAction.PauseGame, TogglePause);
        }

        // 外部接口
        public void SetPauseEnabled(bool enabled)
        {
            canPause = enabled;
        }

        private void TogglePause()
        {
            // 如果禁止暂停，直接返回
            if (!canPause)
            {
                return;
            }
            // 如果是标题场景，直接返回
            if (SceneManager.GetActiveScene().name == "Title-Scene")
            {
                return;
            }
            EnsurePausePanel();

            // 如果正在播放动画，则return
            var view = pausePanel ? pausePanel.GetComponent<PauseView>() : null;
            // 动画过渡中不重复触发
            if (view != null && view.IsTransitioning)
            {
                return;
            }

            // 修改当前暂停状态
            bool newState = !pauseModel.IsPaused;
            // 发布暂停事件
            if (newState)
            {
                // 暂停时间
                pauseModel.SetPaused(newState);
                // 暂停时显示UI
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                    pausePanel.GetComponent<PauseCtl>().Show();
                }
                EventBus.Publish(new EPauseChanged(newState));
            }
            else
            {
                // 恢复时隐藏UI
                if (pausePanel != null)
                {
                    // 播放收起动画，动画结束回调ClosePausePanel
                    pausePanel.GetComponent<PauseCtl>().Hide(ClosePausePanel);
                }
            }
        }

        // 收起动画结束后的收尾
        private void ClosePausePanel()
        {
            pausePanel.SetActive(false); // 隐藏暂停菜单根节点
            pauseModel.SetPaused(false); // 更新模型为“未暂停”
            EventBus.Publish(new EPauseChanged(false)); // 广播“已恢复”事件
        }

        // 确保暂停面板已被加载实例化
        private void EnsurePausePanel()
        {
            if (pausePanel == null)
            {
                // 从 Resources 加载
                var prefab = Resources.Load<GameObject>(PauseMenuPath);
                if (prefab != null)
                {
                    pausePanel = Instantiate(prefab);
                    // 跨场景存活，防止切场景过程中对象被销毁/失活而无法启动协程
                    DontDestroyOnLoad(pausePanel);
                }
                pausePanel.SetActive(false);
            }
        }

        public PauseView GetPauseView()
        {
            EnsurePausePanel();
            if (pausePanel == null)
            {
                Debug.LogWarning("[PauseMgr] GetPauseView: pausePanel 仍为 null（可能资源路径不对？）");
                return null;
            }
            var view = pausePanel.GetComponent<PauseView>();
            if (view == null)
            {
                Debug.LogWarning("[PauseMgr] GetPauseView: 未在 pausePanel 上找到 PauseView 组件");
            }
            return view;
        }

        public void SetShowGuide(bool on)
        {
            var view = GetPauseView();
            if (view == null) return;
            view.showGuide = on;
        }
    }
}
