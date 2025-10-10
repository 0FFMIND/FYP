Shader "Unlit/SpriteMultiply"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
            Cull Off ZWrite Off
            Blend DstColor OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _Color;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 vc : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.vc = v.color * _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, i.uv) * i.vc;
                    fixed a = c.a;
                    c.rgb *= a; // Ô¤³Ë
                    c.a = a;
                    return c;
                }
                ENDCG
            }
        }
}
