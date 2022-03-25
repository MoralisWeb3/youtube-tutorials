/*
               ███████╗░█████╗░██████╗░░█████╗░███████╗  ███████╗██╗███████╗██╗░░░░░██████╗░
               ██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔════╝  ██╔════╝██║██╔════╝██║░░░░░██╔══██╗
               █████╗░░██║░░██║██████╔╝██║░░╚═╝█████╗░░  █████╗░░██║█████╗░░██║░░░░░██║░░██║
               ██╔══╝░░██║░░██║██╔══██╗██║░░██╗██╔══╝░░  ██╔══╝░░██║██╔══╝░░██║░░░░░██║░░██║
               ██║░░░░░╚█████╔╝██║░░██║╚█████╔╝███████╗  ██║░░░░░██║███████╗███████╗██████╔╝
               ╚═╝░░░░░░╚════╝░╚═╝░░╚═╝░╚════╝░╚══════╝  ╚═╝░░░░░╚═╝╚══════╝╚══════╝╚═════╝░

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

Shader "Ultimate 10+ Shaders/Force Field"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)

        _FresnelPower("Fresnel Power", Range(0, 10)) = 3
        _ScrollDirection ("Scroll Direction", float) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull Back
        Lighting Off
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #ifndef SHADER_API_D3D11
                #pragma target 3.0
            #else
                #pragma target 4.0
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float rim : TEXCOORD1;
                float4 position : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            half _FresnelPower;
            half2 _ScrollDirection;

            
            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            fixed3 viewDir;
            v2f vert (appdata vert)
            {
                v2f output;

                output.position = UnityObjectToClipPos(vert.vertex);
                output.uv = TRANSFORM_TEX(vert.uv, _MainTex);

                viewDir = normalize(ObjSpaceViewDir(vert.vertex));
                output.rim = 1.0 - saturate(dot(viewDir, vert.normal));

                output.uv += _ScrollDirection * _Time.y;

                return output;
            }

            fixed4 pixel;
            fixed4 frag (v2f input) : SV_Target
            {
                pixel = tex2D(_MainTex, input.uv) * _Color * pow(_FresnelPower, input.rim);
                pixel = lerp(0, pixel, input.rim);
                
                return clamp(pixel, 0, _Color);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
