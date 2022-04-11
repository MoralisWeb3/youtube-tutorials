/*
        ░█████╗░░█████╗░███████╗░█████╗░███╗░░██╗  ░██████╗██╗░░██╗░█████╗░██████╗░███████╗██████╗░
        ██╔══██╗██╔══██╗██╔════╝██╔══██╗████╗░██║  ██╔════╝██║░░██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
        ██║░░██║██║░░╚═╝█████╗░░███████║██╔██╗██║  ╚█████╗░███████║███████║██║░░██║█████╗░░██████╔╝
        ██║░░██║██║░░██╗██╔══╝░░██╔══██║██║╚████║  ░╚═══██╗██╔══██║██╔══██║██║░░██║██╔══╝░░██╔══██╗
        ╚█████╔╝╚█████╔╝███████╗██║░░██║██║░╚███║  ██████╔╝██║░░██║██║░░██║██████╔╝███████╗██║░░██║
        ░╚════╝░░╚════╝░╚══════╝╚═╝░░╚═╝╚═╝░░╚══╝  ╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝

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

Shader "Ultimate 10+ Shaders/Ocean"
{
    Properties
    {
        _Color ("Color", Color) = (0.0,0.25,0.35,0.0)

        _Normal1 ("Normal Map (1)", 2D) = "white" {}
        _NormalStrength1 ("Normal Strength (1)", Range(0, 2)) = 0.17
        _FlowDirection1("Flow Direction (1)", float) = (0.05, 0, 0, 1)

        _Normal2 ("Normal Map (2)", 2D) = "white" {}
        _NormalStrength2 ("Normal Strength (2)", Range(0, 2)) = 0.8
        _FlowDirection2("Flow Direction (2)", float) = (0, 0.05, 0, 1)

        _Glossiness ("Smoothness", Range(0,1)) = 0.6
        _Metallic ("Metallic", Range(0,1)) = 0.2
        
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 150
        Cull [_Cull]
        Lighting Off
        ZWrite On

        CGINCLUDE
        #define _GLOSSYENV 1
        #define UNITY_SETUP_BRDF_INPUT SpecularSetup
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
        ENDCG

        CGPROGRAM
        // Physically based Standard lighting model, and disabled shadows (the ocean doesn't have a shadow :D)
        #pragma surface surf Standard alpha

        #ifndef SHADER_API_D3D11
            #pragma target 3.0
        #else
            #pragma target 4.0
        #endif

        fixed4 _Color;

        sampler2D _Normal1;
        half _NormalStrength1;
        half2 _FlowDirection1;

        sampler2D _Normal2;
        half _NormalStrength2;
        half2 _FlowDirection2;

        half _Glossiness;
        half _Metallic;

        struct Input
        {
            float2 uv_Normal1;
            float2 uv_Normal2;
        };

        fixed4 pixel;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.uv_Normal1 += _Time.y * _FlowDirection1;
            IN.uv_Normal2 += _Time.y * _FlowDirection2;

            pixel = (tex2D (_Normal1, IN.uv_Normal1) * _NormalStrength1 + tex2D(_Normal2, IN.uv_Normal2) * _NormalStrength2);
            
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
            
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Normal = UnpackNormal(pixel);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
