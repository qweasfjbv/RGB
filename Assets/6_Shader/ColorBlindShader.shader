Shader "Custom/ColorBlindShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Type ("Color Blindness Type", Range(0, 2)) = 0 // 0: Normal, 1: Protanopia, 2: Deuteranopia
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Type;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed3 SimulateProtanopia(fixed3 color)
            {
                fixed3x3 protanopiaMatrix = fixed3x3(
                    0.567, 0.433, 0.0,
                    0.558, 0.442, 0.0,
                    0.0, 0.242, 0.758
                );
                return mul(protanopiaMatrix, color);
            }

            fixed3 SimulateDeuteranopia(fixed3 color)
            {
                fixed3x3 deuteranopiaMatrix = fixed3x3(
                    0.625, 0.375, 0.0,
                    0.7, 0.3, 0.0,
                    0.0, 0.3, 0.7
                );
                return mul(deuteranopiaMatrix, color);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (_Type == 1.0)
                {
                    col.rgb = SimulateProtanopia(col.rgb);
                }
                else if (_Type == 2.0)
                {
                    col.rgb = SimulateDeuteranopia(col.rgb);
                }

                return col;
            }
            ENDCG
        }
    }
}