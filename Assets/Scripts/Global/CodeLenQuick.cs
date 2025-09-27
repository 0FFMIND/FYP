#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public static class CodeLenQuick
{
    [MenuItem("Tools/Count Code Lines (no comments)")]
    static void Count()
    {
        int sum = 0;
        var reBlock = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);
        var reLine = new Regex(@"//.*?$", RegexOptions.Multiline);
        foreach (var g in AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets" }))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            var t = File.ReadAllText(p);
            t = reBlock.Replace(t, ""); t = reLine.Replace(t, "");
            foreach (var line in t.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries))
                if (!string.IsNullOrWhiteSpace(line)) sum++;
        }
        Debug.Log($"[Assets only] Code lines (no comments): {sum}");
    }
}
#endif
