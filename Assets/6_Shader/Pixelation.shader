Shader "Custom/Pixelation"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _PixelDensity ("Pixel Density", Range(1, 1024)) = 256
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _PixelDensity;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv *= _PixelDensity;
                uv = floor(uv) / _PixelDensity;
                return tex2Dlod(_MainTex, float4(uv, 0, 0));
            }
            ENDCG
        }
    }
}