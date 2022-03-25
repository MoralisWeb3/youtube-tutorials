/*
            ██████╗░██╗░░░░░███████╗██╗░░██╗██╗░░░██╗░██████╗  ██╗░░░░░██╗███╗░░██╗███████╗
            ██╔══██╗██║░░░░░██╔════╝╚██╗██╔╝██║░░░██║██╔════╝  ██║░░░░░██║████╗░██║██╔════╝
            ██████╔╝██║░░░░░█████╗░░░╚███╔╝░██║░░░██║╚█████╗░  ██║░░░░░██║██╔██╗██║█████╗░░
            ██╔═══╝░██║░░░░░██╔══╝░░░██╔██╗░██║░░░██║░╚═══██╗  ██║░░░░░██║██║╚████║██╔══╝░░
            ██║░░░░░███████╗███████╗██╔╝╚██╗╚██████╔╝██████╔╝  ███████╗██║██║░╚███║███████╗
            ╚═╝░░░░░╚══════╝╚══════╝╚═╝░░╚═╝░╚═════╝░╚═════╝░  ╚══════╝╚═╝╚═╝░░╚══╝╚══════╝

                        ░██████╗██╗░░██╗░█████╗░██████╗░███████╗██████╗░
                        ██╔════╝██║░░██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
                        ╚█████╗░███████║███████║██║░░██║█████╗░░██████╔╝
                        ░╚═══██╗██╔══██║██╔══██║██║░░██║██╔══╝░░██╔══██╗
                        ██████╔╝██║░░██║██║░░██║██████╔╝███████╗██║░░██║
                        ╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝
                █▀▀▄ █──█ 　 ▀▀█▀▀ █──█ █▀▀ 　 ░█▀▀▄ █▀▀ ▀█─█▀ █▀▀ █── █▀▀█ █▀▀█ █▀▀ █▀▀█ 
                █▀▀▄ █▄▄█ 　 ─░█── █▀▀█ █▀▀ 　 ░█─░█ █▀▀ ─█▄█─ █▀▀ █── █──█ █──█ █▀▀ █▄▄▀ 
                ▀▀▀─ ▄▄▄█ 　 ─░█── ▀──▀ ▀▀▀ 　 ░█▄▄▀ ▀▀▀ ──▀── ▀▀▀ ▀▀▀ ▀▀▀▀ █▀▀▀ ▀▀▀ ▀─▀▀
____________________________________________________________________________________________________________________________________________

        ▄▀█ █▀ █▀ █▀▀ ▀█▀ ▀   █░█ █░░ ▀█▀ █ █▀▄▀█ ▄▀█ ▀█▀ █▀▀   ▄█ █▀█ ▄█▄   █▀ █░█ ▄▀█ █▀▄ █▀▀ █▀█ █▀
        █▀█ ▄█ ▄█ ██▄ ░█░ ▄   █▄█ █▄▄ ░█░ █ █░▀░█ █▀█ ░█░ ██▄   ░█ █▄█ ░▀░   ▄█ █▀█ █▀█ █▄▀ ██▄ █▀▄ ▄█
____________________________________________________________________________________________________________________________________________
License:
    The license is ATTRIBUTION 3.0

    More license info here:
        https://creativecommons.org/licenses/by/3.0/
____________________________________________________________________________________________________________________________________________
This shader has NOT been tested on any other PC configuration except the following:
    CPU: Intel Core i5-6400
    GPU: NVidia GTX 750Ti
    RAM: 16GB
    Windows: 10 x64
    DirectX: 11
____________________________________________________________________________________________________________________________________________
*/

Shader "Ultimate 10+ Shaders/Plexus Line"
{
    Properties
    {
        _Color ("Color", Color) = (0, 1, 0, 1)
        [HDR] _Emission1 ("Emission1", Color) = (2.56, 0, 0, 1)
        [HDR] _Emission2 ("Emission2", Color) = (0, 1.95, 2.52, 1)
        _BoxDims ("Box Dimensions", float) = (5, 5, 5, 1) // Controlled by Plexus.cs

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull [_Cull]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 col : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _Emission1, _Emission2;
            half4 _BoxDims;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.col = fixed4((clamp(o.vertex.xyz/_BoxDims.xyz, -1, 1) + 1.0) / 2.0, 1);
                
                return o;
            }

            fixed4 pixel;
            fixed4 frag (v2f i) : SV_Target
            {
                pixel = _Color + lerp(_Emission1, _Emission2, i.col);
                
                return pixel;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
