using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] pages;

        [SerializeField]
        private GameObject pauseMenuRoot;
        public int PageCount => pages?.Length ?? 0;

        private void Awake()
        {
            if (pauseMenuRoot == null)
            {
                pauseMenuRoot = this.gameObject;
            }
            // 初始隐藏
            pauseMenuRoot.SetActive(false);
        }

        public void Show()
        {
            if (!pauseMenuRoot)
            {
                Debug.LogError("[PauseView] Show() 失败：pauseMenuRoot 为 null。", this);
                return;
            }
            pauseMenuRoot.SetActive(true);
        }

        public void Hide()
        {
            pauseMenuRoot.SetActive(false);
        }

        public void ShowPage(int i)
        {
            // 只显示第 i 页，其他页全部隐藏
            for (int k = 0; k < pages.Length; k++)
            {
                if (pages[k])
                {
                    pages[k].SetActive(k == i);
                }
            }
        }
    }
}
