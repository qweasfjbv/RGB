Shader "Custom/GradientBG"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0, 0.5, 1, 1)
        _BottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_BottomColor, _TopColor, i.uv.y);
            }
            ENDCG
        }
    }
}