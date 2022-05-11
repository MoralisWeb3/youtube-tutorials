Shader "Hidden/2D-Animation-SpriteOutline"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _OutlineSize("Outline Size", Float) = 1
        _OutlineColor("Outline Color", Color) = (1,0,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                float2 clipUV : TEXCOORD1;
            };

            float _Outline;
            fixed4 _OutlineColor;
            uniform float4 _MainTex_ST;
            uniform float4x4 unity_GUIClipTextureMatrix;
            uniform bool _AdjustLinearForGamma;
            sampler2D _GUIClipTexture;
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color;
                float3 eyePos = UnityObjectToViewPos(IN.vertex);
                OUT.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _MainTex_TexelSize;
            fixed _OutlineSize;
            fixed4 _ObjectSize;

            fixed4 frag(v2f i) : SV_Target
            {
                if (tex2D(_GUIClipTexture, i.clipUV).a == 0)
                    discard;

                float width = _OutlineSize*_MainTex_TexelSize.x;
                float height = _OutlineSize*_MainTex_TexelSize.y;

                float2 texPos = i.texcoord + float2(-width, -height);
                half a1 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;
                texPos = i.texcoord + float2(0, -height);
                half a2 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;
                texPos = i.texcoord + float2(+width, -height);
                half a3 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;

                texPos = i.texcoord + float2(-width, 0);
                half a4 =texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;
                texPos = i.texcoord + float2(+width, 0);
                half a6 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;

                texPos = i.texcoord + float2(-width, +height);
                half a7 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;
                texPos = i.texcoord + float2(0, +height);
                half a8 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;
                texPos = i.texcoord + float2(+width, +height);
                half a9 = texPos.x >= 0 && texPos.y >= 0 && texPos.x <= 1 && texPos.y <= 1 ? tex2D(_MainTex, texPos).a : 0;

                half gx = -a1 - a2  - a3 + a7 + a8  + a9;
                half gy = -a1 - a4  - a7 + a3 + a6  + a9;

                half w = sqrt(gx * gx + gy * gy) * 1.25;

                float4 c = _OutlineColor;
                if (w >= 1)
                {
                    if (_AdjustLinearForGamma)
                        c.rgb = LinearToGammaSpace(c.rgb);
                }
                else
                    discard;

                return c;

            }
            ENDCG
        }
    }
}
