using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField]
        private GameObject pauseMenuRoot;

        private void Awake()
        {
            if (pauseMenuRoot == null)
                pauseMenuRoot = this.gameObject;

            // ³õÊ¼Òþ²Ø
            pauseMenuRoot.SetActive(false);
        }

        public void Show()
        {
            pauseMenuRoot.SetActive(true);
        }

        public void Hide()
        {
            pauseMenuRoot.SetActive(false);
        }
    }
}
