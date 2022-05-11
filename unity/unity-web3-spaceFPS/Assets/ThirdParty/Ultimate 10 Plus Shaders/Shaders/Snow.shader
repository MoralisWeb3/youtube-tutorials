/*
        ░██████╗███╗░░██╗░█████╗░░██╗░░░░░░░██╗  ░██████╗██╗░░██╗░█████╗░██████╗░███████╗██████╗░
        ██╔════╝████╗░██║██╔══██╗░██║░░██╗░░██║  ██╔════╝██║░░██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
        ╚█████╗░██╔██╗██║██║░░██║░╚██╗████╗██╔╝  ╚█████╗░███████║███████║██║░░██║█████╗░░██████╔╝
        ░╚═══██╗██║╚████║██║░░██║░░████╔═████║░  ░╚═══██╗██╔══██║██╔══██║██║░░██║██╔══╝░░██╔══██╗
        ██████╔╝██║░╚███║╚█████╔╝░░╚██╔╝░╚██╔╝░  ██████╔╝██║░░██║██║░░██║██████╔╝███████╗██║░░██║
        ╚═════╝░╚═╝░░╚══╝░╚════╝░░░░╚═╝░░░╚═╝░░  ╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝

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

Shader "Ultimate 10+ Shaders/Snow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Normal ("Normal Map", 2D) = "bump" {}

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _SnowColor ("Snow Color", Color) = (1,1,1,1)
        _SnowNormal ("Snow Normal Map", 2D) = "bump" {}

        _SnowGlossiness ("Snow Smoothness", Range(0,1)) = 0.5
        _SnowMetallic ("Snow Metallic", Range(0,1)) = 0.0

        _SnowDirection ("Snow Direction", Vector) = (0, 1, 0, 1)
        _SnowAmount ("Snow Amount", Range(0, 1)) = 0.75

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150
        Cull [_Cull]

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert

        #ifndef SHADER_API_D3D11
            #pragma target 3.0
        #else
            #pragma target 4.0
        #endif

        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _Normal;

        half _Glossiness;
        half _Metallic;

        fixed4 _SnowColor;
        sampler2D _SnowNormal;

        half _SnowGlossiness;
        half _SnowMetallic;

        half3 _SnowDirection;
        fixed _SnowAmount;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
            float2 uv_SnowNormal;
            float dotProduct;
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
            pixel = lerp(tex2D (_MainTex, IN.uv_MainTex) * _Color, _SnowColor, IN.dotProduct);

            o.Albedo = pixel.rgb;
            o.Normal = UnpackNormal(lerp(tex2D(_Normal, IN.uv_Normal), tex2D(_SnowNormal, IN.uv_SnowNormal), IN.dotProduct));
            
            o.Metallic = lerp(_Metallic, _SnowMetallic, IN.dotProduct);
            o.Smoothness = lerp(_Glossiness, _SnowGlossiness, IN.dotProduct);
        }
            
        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            fixed4 color : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        fixed4 texPixel;
        void vert (inout appdata vert, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.dotProduct = saturate(dot(UnityObjectToWorldDir(vert.normal), normalize(_SnowDirection)));
	    o.dotProduct = (o.dotProduct < 1.0 - _SnowAmount) ? 0 : o.dotProduct;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
