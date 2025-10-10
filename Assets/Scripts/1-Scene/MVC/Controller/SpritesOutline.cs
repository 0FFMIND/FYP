using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpritesOutline : MonoBehaviour
    {
        [Header("描边参数")]
        [Range(1, 8)] public int thicknessPx = 1;
        public Material outlineMaterial;              // 建议用“Alpha仅遮罩、纯色输出”的材质
        public bool showOnStart = true;               // Start 后是否显示
        [Tooltip("克隆相对主图的排序偏移，负数在后面，正数在前面")]
        public int orderOffset = -1;

        [Header("节点命名")]
        public string outlineRootName = "__Outline__";
        public string maskNodeName = "__OutlineMask__";

        [Header("SpriteMask 控制")]
        public bool useSpriteMask = true;             // 打开后仅显示外圈
        [Tooltip("让克隆处于遮罩影响范围内的冗余边界（排序 Order 的±范围）")]
        public int maskRangePadding = 2;

        SpriteRenderer _main;
        Transform _root;
        SpriteMask _mask;
        readonly List<SpriteRenderer> _clones = new();

        bool _built;
        Sprite _lastSprite;
        int _lastLayerId, _lastOrder;

        void Awake()
        {
            _main = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            if (!Application.isPlaying) return;
            EnsureRoot();
            if (useSpriteMask) EnsureMask();
            RebuildOutline();
            SetOutlineVisible(showOnStart);
            CacheSorting();
        }

        // ============= 对外 API =============
        public void ShowOutline() => SetOutlineVisible(true);
        public void HideOutline() => SetOutlineVisible(false);

        public void SetOutlineVisible(bool visible)
        {
            EnsureBuilt();
            if (_root) _root.gameObject.SetActive(visible);
            if (_mask) _mask.enabled = visible; // 只影响克隆（主图默认不受遮罩影响）
        }

        // 若运行时改了厚度/材质等参数，手动重建
        public void RebuildOutline()
        {
            if (!_main || !_main.sprite) return;
            EnsureRoot();
            if (useSpriteMask) EnsureMask();

            ClearClones();

            float ppu = _main.sprite.pixelsPerUnit;
            Vector3 ls = transform.lossyScale;
            float pxX = (1f / ppu) / Mathf.Max(Mathf.Abs(ls.x), 1e-6f);
            float pxY = (1f / ppu) / Mathf.Max(Mathf.Abs(ls.y), 1e-6f);

            Vector2[] dirs = {
                Vector2.right, Vector2.left, Vector2.up, Vector2.down,
                new Vector2( 1,  1), new Vector2( 1, -1),
                new Vector2(-1,  1), new Vector2(-1, -1),
            };

            foreach (var d in dirs)
            {
                var go = new GameObject("Outline");
                go.transform.SetParent(_root, false);
                go.transform.localPosition = new Vector3(d.x * thicknessPx * pxX, d.y * thicknessPx * pxY, 0f);

                var sr = go.AddComponent<SpriteRenderer>();
                CopyFromMain(sr);
                sr.sprite = _main.sprite;

                // 排序：放在主图相对位置（默认后面）
                sr.sortingLayerID = _main.sortingLayerID;
                sr.sortingOrder = _main.sortingOrder + orderOffset;

                // 只显示外圈：让克隆受 SpriteMask 影响
                if (useSpriteMask) sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                _clones.Add(sr);
            }

            // 遮罩同步贴图与范围
            if (useSpriteMask) { UpdateMaskSprite(); UpdateMaskRange(); }

            _built = true;
            _lastSprite = _main.sprite;
            CacheSorting();
        }

        public void ChangeOrder(int order)
        {
            for (int i = 0; i < _clones.Count; i++)
                if (_clones[i]) _clones[i].sortingOrder = order;
            if (useSpriteMask) UpdateMaskRange();
        }

        // ============= 内部工具 =============
        void EnsureBuilt()
        {
            if (!_built && Application.isPlaying)
            {
                EnsureRoot();
                if (useSpriteMask) EnsureMask();
                RebuildOutline();
            }
        }

        void EnsureRoot()
        {
            if (_root) return;
            var exist = transform.Find(outlineRootName);
            if (exist) { if (Application.isPlaying) Destroy(exist.gameObject); else DestroyImmediate(exist.gameObject); }
            var rootGo = new GameObject(outlineRootName);
            rootGo.transform.SetParent(transform, false);
            _root = rootGo.transform;
        }

        void EnsureMask()
        {
            if (_mask) return;
            var exist = transform.Find(maskNodeName);
            if (exist) { if (Application.isPlaying) Destroy(exist.gameObject); else DestroyImmediate(exist.gameObject); }
            var go = new GameObject(maskNodeName);
            go.transform.SetParent(transform, false);
            _mask = go.AddComponent<SpriteMask>();
            _mask.sprite = _main ? _main.sprite : null;
            _mask.enabled = true;
        }

        void UpdateMaskSprite()
        {
            if (_mask && _main) _mask.sprite = _main.sprite;
        }

        void UpdateMaskRange()
        {
            if (!_mask || !_main) return;
            int layer = _main.sortingLayerID;
            int clonesOrder = _main.sortingOrder + orderOffset;
            _mask.backSortingLayerID = layer;
            _mask.frontSortingLayerID = layer;
            _mask.backSortingOrder = clonesOrder - Mathf.Abs(maskRangePadding);
            _mask.frontSortingOrder = clonesOrder + Mathf.Abs(maskRangePadding);
        }

        void CopyFromMain(SpriteRenderer sr)
        {
            sr.flipX = _main.flipX;
            sr.flipY = _main.flipY;
            sr.spriteSortPoint = _main.spriteSortPoint;
            sr.drawMode = _main.drawMode;
            sr.sharedMaterial = outlineMaterial ? outlineMaterial : _main.sharedMaterial;
            // 注意：主图通常保持 None，这样主图不被遮罩影响
            // 克隆在 RebuildOutline 里会被设成 VisibleOutsideMask
        }

        void ClearClones()
        {
            for (int i = 0; i < _clones.Count; i++)
            {
                var c = _clones[i];
                if (!c) continue;
                if (Application.isPlaying) Destroy(c.gameObject); else DestroyImmediate(c.gameObject);
            }
            _clones.Clear();
        }

        void OnDisable()
        {
            if (Application.isPlaying) ClearClones();
        }

        void OnDestroy()
        {
            if (_root) { if (Application.isPlaying) Destroy(_root.gameObject); else DestroyImmediate(_root.gameObject); }
            if (_mask) { if (Application.isPlaying) Destroy(_mask.gameObject); else DestroyImmediate(_mask.gameObject); }
            _root = null;
            _mask = null;
            _built = false;
        }

        void LateUpdate()
        {
            if (!Application.isPlaying || !_built || !_main) return;

            // 1) 主图换 sprite → 同步克隆与遮罩
            if (_main.sprite != _lastSprite)
            {
                for (int i = 0; i < _clones.Count; i++) if (_clones[i]) _clones[i].sprite = _main.sprite;
                if (useSpriteMask) UpdateMaskSprite();
                _lastSprite = _main.sprite;
            }

            // 2) 排序变化 → 跟随克隆的排序 & 遮罩范围
            bool layerChanged = (_main.sortingLayerID != _lastLayerId);
            bool orderChanged = (_main.sortingOrder != _lastOrder);
            if (layerChanged || orderChanged)
            {
                int layer = _main.sortingLayerID;
                int order = _main.sortingOrder + orderOffset;
                for (int i = 0; i < _clones.Count; i++)
                {
                    var c = _clones[i];
                    if (!c) continue;
                    if (layerChanged) c.sortingLayerID = layer;
                    if (orderChanged) c.sortingOrder = order;
                }
                if (useSpriteMask) UpdateMaskRange();
                CacheSorting();
            }
        }

        void CacheSorting()
        {
            _lastLayerId = _main.sortingLayerID;
            _lastOrder = _main.sortingOrder;
        }
    }
}
