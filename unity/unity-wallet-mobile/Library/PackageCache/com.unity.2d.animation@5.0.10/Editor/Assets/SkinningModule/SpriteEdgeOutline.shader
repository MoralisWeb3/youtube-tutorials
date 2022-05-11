Shader "Hidden/2D-Animation-SpriteEdgeOutline"
{
    Properties
    {
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
        ZTest Less
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
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 clipUV : TEXCOORD1;
            };

            uniform float4x4 unity_GUIClipTextureMatrix;
            sampler2D _GUIClipTexture;
            uniform bool _AdjustLinearForGamma;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                float3 eyePos = UnityObjectToViewPos(IN.vertex);
                OUT.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));

                return OUT;
            }
            
            fixed4 _OutlineColor;

            fixed4 frag(v2f i) : SV_Target
            {
                if (tex2D(_GUIClipTexture, i.clipUV).a == 0)
                    discard;
                float4 c = _OutlineColor;
                if (_AdjustLinearForGamma)
                        c.rgb = LinearToGammaSpace(c.rgb);
                return c;
            }
            ENDCG
        }
    }
}
