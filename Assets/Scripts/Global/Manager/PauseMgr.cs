using System.Collections;
using System.Collections.Generic;
using MVC;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Manager
{
    public class PauseMgr : SingletonMB<PauseMgr>
    {
        private PauseModel pauseModel;
        private PauseView pauseView;
        private const string PauseMenuPath = "Prefabs/1-Scene/PauseMenu";
        private bool canPause = true;

        // AutoSingletonMB
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureCreated();
        }

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
            EnsurePauseView();
            bool newState = !pauseModel.IsPaused;
            // 设置暂停状态
            pauseModel.SetPaused(newState);
            // 发布暂停事件
            EventBus.Publish(new EPauseChanged(newState));
            if (newState)
            {
                // 暂停时显示UI
                if (pauseView != null)
                    pauseView.Show();
            }
            else
            {
                // 恢复时隐藏UI
                if (pauseView != null)
                    pauseView.Hide();
            }
        }

        private void EnsurePauseView()
        {
            if (pauseView == null)
            {
                // 从 Resources 加载
                var prefab = Resources.Load<PauseView>(PauseMenuPath);
                if (prefab != null)
                {
                    pauseView = Instantiate(prefab);
                }
            }
        }
    }
}
