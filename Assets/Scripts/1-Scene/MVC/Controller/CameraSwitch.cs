using System;
using System.Collections;
using Cinemachine;
using Manager;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [Header("Cameras")]
    public Camera worldCam; // 主相机（挂着 CinemachineBrain）
    public Camera uiCam; // 专门渲染 UI 的相机（不挂 Brain）

    public Canvas uiCanvas;

    [Header("Options")]
    // 进入UI时是否暂停Brain，避免机位抖动/Blend
    public bool freezeBrainOnUI = true;

    // 暂停前把默认过渡改成 Cut（退出时还原）
    public bool useCutBlendWhenFreeze = true;

    [Header("Fade Options")]
    public bool useFade = true; // 是否在切换时淡入淡出
    public float fadeOutDuration = 0.5f; // 进入切换前：0→1（变黑）
    public float fadeInDuration = 0.5f; // 切换完成后：1→0（恢复画面）

    private CinemachineBrain _brain;
    private CinemachineBlendDefinition _prevBlend;
    private Coroutine _switchCo;

    void Start()
    {
        // 若未手动指定主相机，回退到场景中的 Camera.main
        if (!worldCam)
        {
            worldCam = Camera.main;
        }

        // 拿 Brain 并保存默认过渡
        _brain = worldCam ? worldCam.GetComponent<CinemachineBrain>() : null;
        if (_brain != null)
            _prevBlend = _brain.m_DefaultBlend;

        // 把 UI 相机叠在主相机之上
        if (uiCam)
        {
            uiCam.clearFlags = CameraClearFlags.Depth; // 只清深度
            uiCam.depth = (worldCam ? worldCam.depth : 0f) + 1f; // 渲染顺序更靠后
            uiCam.stereoTargetEye = StereoTargetEyeMask.None;
            // 一开始整台 UI 相机 GameObject 关闭（含其子层的 Canvas 一并关闭）
            uiCam.gameObject.SetActive(false);
        }

        // Screen Space - Camera 绑定 UI 相机
        if (uiCanvas && uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            uiCanvas.worldCamera = uiCam;
        if (uiCanvas)
            uiCanvas.enabled = false; // 初始关闭
    }

    /// <summary>进入 UI（带可选淡入淡出）。</summary>
    public void EnterUI(Action onCompleted)
    {
        if (_switchCo != null)
        {
            StopCoroutine(_switchCo);
        }
        _switchCo = StartCoroutine(Co_EnterUI(onCompleted));
    }

    /// <summary>退出 UI（带可选淡入淡出）。</summary>
    public void ExitUI(Action onCompleted = null)
    {
        if (_switchCo != null)
            StopCoroutine(_switchCo);
        _switchCo = StartCoroutine(Co_ExitUI(onCompleted));
    }

    private IEnumerator Co_EnterUI(Action onCompleted)
    {
        // 1) 先淡到黑
        if (useFade && TransitionMgr.Instance != null)
            yield return TransitionMgr.Instance.FadeOut(fadeOutDuration);

        // 打开整套 UI（包含相机 GameObject）
        SetUIActive(true);

        if (freezeBrainOnUI && _brain)
        {
            if (useCutBlendWhenFreeze)
                _brain.m_DefaultBlend = new CinemachineBlendDefinition(
                    CinemachineBlendDefinition.Style.Cut,
                    0f
                );
            _brain.enabled = false; // 冻结机位更新
        }
        onCompleted?.Invoke();

        _switchCo = null;
    }

    private IEnumerator Co_ExitUI(Action onCompleted)
    {

        if (useFade && TransitionMgr.Instance != null)
            yield return TransitionMgr.Instance.FadeOut(fadeOutDuration);

        if (freezeBrainOnUI && _brain)
        {
            _brain.enabled = true;
            if (useCutBlendWhenFreeze)
                _brain.m_DefaultBlend = _prevBlend;
        }

        SetUIActive(false);

        if (useFade && TransitionMgr.Instance != null)
            yield return TransitionMgr.Instance.FadeIn(fadeInDuration);

        // 这里才是“退出完成”的时机
        onCompleted?.Invoke();

        _switchCo = null;
        yield return null;
    }

    /// <summary>统一开关 UI：相机 GameObject、Camera 组件、Canvas 一并处理。</summary>
    private void SetUIActive(bool on)
    {
        if (uiCam)
        {
            // 先切 GameObject 激活态，再同步组件开关
            uiCam.gameObject.SetActive(on);
            uiCam.enabled = on;
        }
        if (uiCanvas)
        {
            uiCanvas.enabled = on;
            // 若你的 UI 还有 CanvasGroup/可交互控制，也可以在这里一起处理
            // var cg = uiCanvas.GetComponent<CanvasGroup>();
            // if (cg) { cg.blocksRaycasts = on; cg.interactable = on; }
        }
    }
}
