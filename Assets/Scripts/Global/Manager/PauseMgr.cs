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
        private GameObject pausePanel;
        private const string PauseMenuPath = "Prefabs/1-Scene/PauseMenu";
        private bool canPause = true;

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
                    pausePanel.GetComponent<PauseCtl>().Hide(ClosePausePanel);
                }
            }
        }

        private void ClosePausePanel()
        {
            pausePanel.SetActive(false);
            pauseModel.SetPaused(false);
            EventBus.Publish(new EPauseChanged(false));
        }

        private void EnsurePausePanel()
        {
            if (pausePanel == null)
            {
                // 从 Resources 加载
                var prefab = Resources.Load<GameObject>(PauseMenuPath);
                if (prefab != null)
                {
                    pausePanel = Instantiate(prefab);
                }
                pausePanel.SetActive(false);
            }
        }
    }
}
