using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class HoleOverlayBinder : MonoBehaviour
{
    [SerializeField] private string holeShaderName = "Unlit/HoleColor";
    [SerializeField] private Vector2 holeCenter01 = new(0.5f, 0.5f);
    [SerializeField] private Vector2 holeSize01 = new(0.3f, 0.2f);
    [SerializeField] private float cornerRadius = 0.1f;
    [SerializeField] private float featherPx = 8f;

    private Material _instMat;
    private Graphic _g;

    void Awake()
    {
        _g = GetComponent<Graphic>();
        // 1) 如果 Inspector 已经指定了材质，就克隆它；否则就用 Shader 新建
        var src = _g.material != null ? _g.material : new Material(Shader.Find(holeShaderName));
        _instMat = new Material(src);          // <<< 关键：克隆，得到“独立实例”
        _g.material = _instMat;                // 绑回本 Graphic，只影响自己
        Apply();
    }

    public void Apply()
    {
        if (_instMat == null) return;
        _instMat.SetVector("_HoleCenter", new Vector4(holeCenter01.x, holeCenter01.y, 0, 0));
        _instMat.SetVector("_HoleSize", new Vector4(holeSize01.x, holeSize01.y, 0, 0));
        _instMat.SetFloat("_CornerRadius", cornerRadius);
        _instMat.SetFloat("_Feather", featherPx);
    }
}

