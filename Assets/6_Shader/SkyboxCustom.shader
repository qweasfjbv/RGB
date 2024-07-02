Shader "Custom/SkyboxCustom"
{
    Properties
    {
        _FrontTex ("Front (+Z)", 2D) = "white" {}
        _BackTex ("Back (-Z)", 2D) = "white" {}
        _LeftTex ("Left (-X)", 2D) = "white" {}
        _RightTex ("Right (+X)", 2D) = "white" {}
        _UpTex ("Up (+Y)", 2D) = "white" {}
        _DownTex ("Down (-Y)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            sampler2D _FrontTex;
            sampler2D _BackTex;
            sampler2D _LeftTex;
            sampler2D _RightTex;
            sampler2D _UpTex;
            sampler2D _DownTex;
            uniform float4x4 _Rotation;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = mul(_Rotation, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (abs(i.texcoord.z) > abs(i.texcoord.x) && abs(i.texcoord.z) > abs(i.texcoord.y))
                {
                    if (i.texcoord.z > 0)
                        return tex2D(_FrontTex, i.texcoord.xy * 0.5 + 0.5);
                    else
                        return tex2D(_BackTex, i.texcoord.xy * 0.5 + 0.5);
                }
                else if (abs(i.texcoord.x) > abs(i.texcoord.y))
                {
                    if (i.texcoord.x > 0)
                        return tex2D(_RightTex, i.texcoord.yz * 0.5 + 0.5);
                    else
                        return tex2D(_LeftTex, i.texcoord.yz * 0.5 + 0.5);
                }
                else
                {
                    if (i.texcoord.y > 0)
                        return tex2D(_UpTex, i.texcoord.xz * 0.5 + 0.5);
                    else
                        return tex2D(_DownTex, i.texcoord.xz * 0.5 + 0.5);
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}