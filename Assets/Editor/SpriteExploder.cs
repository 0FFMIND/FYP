#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SpriteExploder
{
    [MenuItem("Tools/Explode Sliced Sprite To Children")]
    public static void ExplodeSelection()
    {
        var go = Selection.activeGameObject;
        if (go == null) { EditorUtility.DisplayDialog("Sprite Exploder", "请先在层级里选中那张‘完整大图’的GameObject。", "OK"); return; }

        var parentSR = go.GetComponent<SpriteRenderer>();
        if (parentSR == null || parentSR.sprite == null)
        {
            EditorUtility.DisplayDialog("Sprite Exploder", "选中的物体没有 SpriteRenderer 或没有 Sprite。", "OK");
            return;
        }

        // 取整张贴图与PPU
        var parentSprite = parentSR.sprite;
        var tex = parentSprite.texture;
        if (tex == null) { EditorUtility.DisplayDialog("Sprite Exploder", "找不到贴图。", "OK"); return; }

        // 载入同一资源路径下的所有子Sprite（Multiple切片）
        string path = AssetDatabase.GetAssetPath(tex);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        if (sprites.Length <= 1)
        {
            EditorUtility.DisplayDialog("Sprite Exploder", "这张贴图似乎不是 Multiple（或没有切片）。", "OK");
            return;
        }

        // 校验Pivot是否为Center（非硬性，但Center→Center最稳）
        bool allCenter = sprites.All(s =>
        {
            var r = s.rect;
            var p = s.pivot;
            return Mathf.Approximately(p.x, r.width * 0.5f) && Mathf.Approximately(p.y, r.height * 0.5f);
        });
        if (!allCenter)
        {
            if (!EditorUtility.DisplayDialog("注意", "检测到有切片的Pivot不是 Center。位置也可以还原，但可能有微小偏移。继续吗？", "继续", "取消"))
                return;
        }

        Undo.IncrementCurrentGroup();
        Undo.RecordObject(go.transform, "Explode Sliced Sprites");

        float ppu = parentSprite.pixelsPerUnit;
        Vector2 texSizePx = new Vector2(tex.width, tex.height);
        Vector2 texCenterPx = texSizePx * 0.5f;

        // 可选：生成一个容器
        var container = new GameObject(go.name + "_Exploded");
        Undo.RegisterCreatedObjectUndo(container, "Create Container");
        container.transform.SetParent(go.transform, false);
        container.transform.localPosition = Vector3.zero;

        // 让父的Sprite先隐藏，保留当参考或备份
        parentSR.enabled = false;

        // 稳定排序：按y从大到小、再按x从小到大（方便同格子顺序一致）
        var ordered = sprites.OrderByDescending(s => s.rect.center.y).ThenBy(s => s.rect.center.x);
        foreach (var s in ordered)
        {
            var child = new GameObject(s.name);
            Undo.RegisterCreatedObjectUndo(child, "Create Child");
            child.transform.SetParent(container.transform, false);

            // 核心：把切片中心相对整图中心的像素偏移，换算到单位坐标
            Vector2 centerPx = s.rect.center;
            Vector2 offsetPx = centerPx - texCenterPx;

            // 若切片Pivot不是Center，做一个小修正（把pivot偏移换算进来）
            Vector2 pivotDeltaPx = s.pivot - (s.rect.size * 0.5f);

            Vector2 localUnits = (offsetPx + pivotDeltaPx) / ppu;
            child.transform.localPosition = new Vector3(localUnits.x, localUnits.y, 0f);

            var sr = child.AddComponent<SpriteRenderer>();
            sr.sprite = s;
            // 继承父的Sorting设置
            sr.sortingLayerID = parentSR.sortingLayerID;
            sr.sortingOrder = parentSR.sortingOrder;
            sr.sortingLayerName = parentSR.sortingLayerName;
            sr.material = parentSR.sharedMaterial;
            sr.maskInteraction = parentSR.maskInteraction;
        }

        EditorUtility.DisplayDialog("完成", $"已生成 {sprites.Length} 个子Sprite，并保持原位置关系。", "OK");
    }
}
#endif
