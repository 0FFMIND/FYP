Shader "Unlit/HoleColor_MulDim_NoStencil"
{
    Properties
    {
        _Dim("Dim Strength (0..1)", Range(0,1)) = 0.6     // 洞外变暗强度
        _HoleCenter("Hole Center (0-1)", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize("Hole Size (0-1, w,h)", Vector) = (0.3, 0.2, 0, 0)
        _CornerRadius("Corner Radius (0-1 of min(w,h))", Range(0,0.5)) = 0.1
        _Feather("Edge Feather (pixels)", Range(0,50)) = 8
        _Highlight("Edge Highlight", Color) = (1,1,1,0.08) // 可选边缘微光
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Cull Off Lighting Off ZWrite Off

        CGINCLUDE
        #include "UnityCG.cginc"
        #pragma target 3.0

        float  _Dim;
        float4 _HoleCenter;
        float4 _HoleSize;
        float  _CornerRadius;
        float  _Feather;
        float4 _Highlight;

        struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
        struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

        v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

        float sdRoundRect(float2 p, float2 b, float r) {
            float2 q = abs(p) - (b - r);
            return length(max(q,0.0)) - r;
        }

        void holeDistance(float2 uv, out float d, out float featherUV) {
            float2 center = _HoleCenter.xy;
            float2 size = _HoleSize.xy;
            float2 half = size * 0.5;
            float2 dd = fwidth(uv);
            float pxToUv = max(dd.x, dd.y);
            featherUV = _Feather * pxToUv;
            float2 p = uv - center;
            float r = _CornerRadius * min(size.x, size.y);
            d = sdRoundRect(p, half, r);
        }

        // 边缘“环形”权重：靠近边界更亮
        float ringWeight(float d, float f) {
            float w = 1.0 - saturate(abs(d) / max(f, 1e-5)); // 0..1
            return w * w;
        }
        ENDCG

            // --- Pass 1：乘法式变暗（洞外生效、洞内不变） ---
            Pass
            {
            // 关键：用乘暗混合，不改变 alpha，不依赖/影响 Stencil
            // 结果：out = dst*(1 - src.a) + src*0
            Blend Zero OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                float d, f; holeDistance(i.uv, d, f);
        // edge: 0=洞内, 1=洞外
        float edge = smoothstep(0.0, f, d);

        // 洞外让 src.a = _Dim，洞内 src.a = 0
        float a = _Dim * edge;

        // 这里的颜色值无关紧要（因 SrcFactor=Zero），给 0 即可
        return fixed4(0,0,0, a);
    }
    ENDCG
}

// --- Pass 2（可选）：洞内边缘微高亮 ---
Pass
{
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    fixed4 frag(v2f i) : SV_Target
    {
        float d, f; holeDistance(i.uv, d, f);
        if (d >= 0) return 0; // 只在洞内
        float w = ringWeight(d, f);    // 边缘一圈
        return fixed4(_Highlight.rgb, _Highlight.a * w);
    }
    ENDCG
}
    }
}
