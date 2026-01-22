using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// SceneBootstrapper：用于“场景启动时统一校正可见性/激活状态”的引导脚本。
///
/// 典型用途：
/// - 你在编辑场景时可能会临时把某些 GameObject 设为可见/不可见（SetActive true/false）来方便调试或摆放；
/// - 但你不希望这些“编辑时的可见性状态”影响到真正的游戏启动状态；
/// - 所以把“启动时必须激活的对象”放到 activeOnStart，把“启动时必须关闭的对象”放到 inactiveOnStart，
///   运行时在 Awake（很早的生命周期）一次性应用，强制把状态纠正到你想要的初始配置。
///
/// 结果：
/// - 场景的最终初始状态由两个列表决定，而不是由你编辑场景时最后一次的可见性改动决定；
/// - 能避免因为手动修改场景可见性导致的“开局状态错乱”。 
/// </summary>
[DefaultExecutionOrder(-1000)] // 尽量早执行，避免别的脚本先访问到错误状态
public class SceneBootstrapper : MonoBehaviour
{
    [Header("启动时激活（SetActive(true)）")]
    [SerializeField] private List<GameObject> activeOnStart = new List<GameObject>();

    [Header("启动时禁用（SetActive(false)）")]
    [SerializeField] private List<GameObject> inactiveOnStart = new List<GameObject>();

    [Header("Options")]
    [SerializeField] private bool applyInAwake = true;

    private void Awake()
    {
        if (applyInAwake) Apply();
    }

    [ContextMenu("Apply Now")]
    public void Apply()
    {
        // 这里采用“先关后开”，便于覆盖掉误激活的对象
        SetListActive(inactiveOnStart, false);
        SetListActive(activeOnStart, true);
    }

    private static void SetListActive(List<GameObject> list, bool isActive)
    {
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            var go = list[i];
            // 忽略空引用
            if (!go)
            {
                continue;
            }

            // 仅在状态不同时才设置，避免重复触发事件
            if (go.activeSelf != isActive)
            {
                go.SetActive(isActive);
            }
        }
    }
}
