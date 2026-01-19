using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MVC
{
    public class Scene1DrainGameCtl : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private GameObject uiRoot;          // UI 面板（打开/关闭）

        [Header("逻辑")]
        [SerializeField]
        private Scene1Drain drain;          // 对应的交互脚本（继承自 InteractCtl）

        // 用 0/1/2/3 这几个离散档来表示概率：0, 1/3, 2/3, 1
        private const int MaxStep = 3;
        private int _curStep = 0;           // 初始给 0/3 概率
        private bool _finished = false;     // 是否已经完成（成功勾到）

        /// <summary>
        /// 打开这个 UI 时由外部调用，带上当前玩家。
        /// 你可以在 Scene1Drain 里调用 drainCtl.Init(player) 再激活 uiRoot。
        /// </summary>
        public void Init()
        {
            _curStep = 0;      // 重置为 0/3
            _finished = false;

            if (uiRoot != null)
            {
                uiRoot.SetActive(true);
            }
        }

        /// <summary>
        /// 按钮回调：尝试勾取一次。
        /// 挂到 Button.onClick 上即可。
        /// </summary>
        public void TryHook()
        {
            if (_finished)
            {
                return;
            }

            // 判定是否成功
            float r = Random.value; // [0,1)
            if (r > 0.5f)
            {
                // 勾取成功，提升一档
                _curStep += 1;
                // 如果达到最大档位，判定为成功
                if (_curStep >= MaxStep)
                {
                    _curStep = MaxStep;
                    _finished = true;
                    // 成功勾到，关闭 UI 并通知 drain 脚本
                    uiRoot.SetActive(false);
                }
                // _finished = true;
                // 如果需要，你可以在这里调用 drain 的某个标记成功的方法
                // 比如：drain.OnHookSuccess();
            }

            // 勾取失败，下降一档
            _curStep = Mathf.Clamp(_curStep - 1, 0, MaxStep);

            // TODO: 如需在 UI 上显示当前概率，可以在这里更新一个进度条/文本：
            // float displayChance = _curStep / (float)MaxStep; // 0, 0.33, 0.67, 1
        }
    }
}
