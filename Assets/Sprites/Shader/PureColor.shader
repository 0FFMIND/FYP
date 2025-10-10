Shader "Unlit/PureColor"
{
    Properties{
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,1,0,1)
    }
        SubShader{
            Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
            Cull Off ZWrite Off Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _OutlineColor;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv     : TEXCOORD0;
                    fixed4 color : COLOR;
                };
                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };
                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }
                fixed4 frag(v2f i) : SV_Target{
                    fixed a = tex2D(_MainTex, i.uv).a;
                    return fixed4(_OutlineColor.rgb, _OutlineColor.a * a);
                }
                ENDCG
            }
        }
}
