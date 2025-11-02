using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1JournalCtl : MonoBehaviour
    {
        [SerializeField]
        private GameObject journalCtl;

        private Coroutine _currentCoroutine;

        private void Awake()
        {
            // 在开始时关闭 journalCtl
            if (journalCtl != null)
            {
                journalCtl.SetActive(false);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EJournalUIChanged>(OnChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EJournalUIChanged>(OnChanged);

            // 在禁用时也停止协程
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
        }

        private void OnChanged(EJournalUIChanged e)
        {
            // 如果有协程正在运行，先停止它
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                // 确保 journalCtl 被禁用
                if (journalCtl != null)
                {
                    journalCtl.SetActive(false);
                }
            }

            // 开始新的协程
            _currentCoroutine = StartCoroutine(StartJournalAnim());
        }

        private IEnumerator StartJournalAnim()
        {
            // 激活 journalCtl 播放动画
            if (journalCtl != null)
            {
                journalCtl.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                journalCtl.SetActive(false);
            }

            _currentCoroutine = null;
        }
    }
}