Shader "Unlit/HoleColor"
{
    Properties
    {
        _Color("Overlay Color", Color) = (0,0,0,0.6)
        _HoleCenter("Hole Center (0-1)", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize("Hole Size (0-1, w,h)", Vector) = (0.3, 0.2, 0, 0)
        _CornerRadius("Corner Radius (0-1 of min(w,h))", Range(0,0.5)) = 0.1
        _Feather("Edge Feather (pixels)", Range(0,50)) = 8
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "CanUseSpriteAtlas" = "True" }
        Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _HoleCenter;  // xy: [0,1] in local rect
            float4 _HoleSize;    // xy: [0,1] fraction of rect (w,h)
            float _CornerRadius; // 0..0.5
            float _Feather;      // pixels

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // UI 默认 mesh 的 uv0 = 顶点在 Rect 内的 0..1
                o.uv = v.uv;
                return o;
            }

            // Signed Distance for rounded-rect in uv space (0..1)
            float sdRoundRect(float2 p, float2 b, float r)
            {
                // p: point relative to center; b: half-size; r: radius
                float2 q = abs(p) - (b - r);
                return length(max(q,0.0)) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 将洞中心与大小从属性带入（单位：相对 UI Rect 的 0..1）
                float2 center = _HoleCenter.xy;
                float2 size = _HoleSize.xy; // 0..1
                float2 half = size * 0.5;

                // 像素大小估算：用于 Feather 从像素换算到 uv
                float2 dd = fwidth(i.uv); // 近似每像素的 UV 变化
                float pxToUv = max(dd.x, dd.y);
                float featherUV = _Feather * pxToUv;

                // 计算点到圆角矩形边界的有符号距离（<0 在内部）
                float2 p = i.uv - center;
                float r = _CornerRadius * min(size.x, size.y);
                float d = sdRoundRect(p, half, r);

                // d < 0：在洞内 -> alpha = 0；边缘用 smoothstep 软化
                float edge = smoothstep(0.0, featherUV, d); // 0..1, 0=洞内, 1=遮罩
                fixed4 col = _Color;
                col.a *= edge;

                return col;
            }
            ENDCG
        }
    }
}
