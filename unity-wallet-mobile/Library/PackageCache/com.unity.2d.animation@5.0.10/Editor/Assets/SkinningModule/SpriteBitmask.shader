Shader "Hidden/2D-Animation-SpriteBitmask"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend Off
        ColorMask A

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
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
