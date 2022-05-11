/*
                ░██████╗██╗███╗░░░███╗██████╗░██╗░░░░░███████╗  ░██████╗██╗███╗░░██╗███████╗
                ██╔════╝██║████╗░████║██╔══██╗██║░░░░░██╔════╝  ██╔════╝██║████╗░██║██╔════╝
                ╚█████╗░██║██╔████╔██║██████╔╝██║░░░░░█████╗░░  ╚█████╗░██║██╔██╗██║█████╗░░
                ░╚═══██╗██║██║╚██╔╝██║██╔═══╝░██║░░░░░██╔══╝░░  ░╚═══██╗██║██║╚████║██╔══╝░░
                ██████╔╝██║██║░╚═╝░██║██║░░░░░███████╗███████╗  ██████╔╝██║██║░╚███║███████╗
                ╚═════╝░╚═╝╚═╝░░░░░╚═╝╚═╝░░░░░╚══════╝╚══════╝  ╚═════╝░╚═╝╚═╝░░╚══╝╚══════╝

           ░██╗░░░░░░░██╗░█████╗░██╗░░░██╗███████╗  ░██████╗██╗░░██╗░█████╗░██████╗░███████╗██████╗░
           ░██║░░██╗░░██║██╔══██╗██║░░░██║██╔════╝  ██╔════╝██║░░██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
           ░╚██╗████╗██╔╝███████║╚██╗░██╔╝█████╗░░  ╚█████╗░███████║███████║██║░░██║█████╗░░██████╔╝
           ░░████╔═████║░██╔══██║░╚████╔╝░██╔══╝░░  ░╚═══██╗██╔══██║██╔══██║██║░░██║██╔══╝░░██╔══██╗
           ░░╚██╔╝░╚██╔╝░██║░░██║░░╚██╔╝░░███████╗  ██████╔╝██║░░██║██║░░██║██████╔╝███████╗██║░░██║
           ░░░╚═╝░░░╚═╝░░╚═╝░░╚═╝░░░╚═╝░░░╚══════╝  ╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝

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

Shader "Ultimate 10+ Shaders/Simple Sine Wave"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Normal ("Normal Map", 2D) = "bump" {}
        _Speed ("Speed", float) = 1.25
        _Amplitude ("Amplitude", float) = 1.0

        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.5

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150
        Cull [_Cull]
        ZWrite On

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma vertex vert

        #ifndef SHADER_API_D3D11
            #pragma target 3.0
        #else
            #pragma target 4.0
        #endif

        fixed4 _Color;

        sampler2D _MainTex;
        sampler2D _Normal;

        half _Smoothness;
        half _Metallic;

        half _Speed;
        half _Amplitude;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 pixel;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            pixel = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = pixel.rgb;
            
            o.Smoothness = _Smoothness;
            o.Metallic = _Metallic;

            o.Normal = tex2D(_Normal, IN.uv_Normal);
        }
 
        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            fixed4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        void vert(inout appdata v){
            v.vertex.y = sin((_Time.y + v.vertex.x) * _Speed) * _Amplitude;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
