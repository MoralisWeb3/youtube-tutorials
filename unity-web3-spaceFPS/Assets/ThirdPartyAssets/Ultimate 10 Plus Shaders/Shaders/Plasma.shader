/*
    ██████╗░██╗░░░░░░█████╗░░██████╗███╗░░░███╗░█████╗░  ░██████╗██╗░░██╗░█████╗░██████╗░███████╗██████╗░
    ██╔══██╗██║░░░░░██╔══██╗██╔════╝████╗░████║██╔══██╗  ██╔════╝██║░░██║██╔══██╗██╔══██╗██╔════╝██╔══██╗
    ██████╔╝██║░░░░░███████║╚█████╗░██╔████╔██║███████║  ╚█████╗░███████║███████║██║░░██║█████╗░░██████╔╝
    ██╔═══╝░██║░░░░░██╔══██║░╚═══██╗██║╚██╔╝██║██╔══██║  ░╚═══██╗██╔══██║██╔══██║██║░░██║██╔══╝░░██╔══██╗
    ██║░░░░░███████╗██║░░██║██████╔╝██║░╚═╝░██║██║░░██║  ██████╔╝██║░░██║██║░░██║██████╔╝███████╗██║░░██║
    ╚═╝░░░░░╚══════╝╚═╝░░╚═╝╚═════╝░╚═╝░░░░░╚═╝╚═╝░░╚═╝  ╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░╚══════╝╚═╝░░╚═╝

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

Shader "Ultimate 10+ Shaders/Plasma"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Normal("Normal map", 2D) = "bump" {}

        _NoiseTex ("Noise", 2D) = "white" {}
        _MovementDirection ("Movement Direction", float) = (0, -1, 0, 1)
        
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull [_Cull]
        Lighting Off
        ZWrite On
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Normal;
        sampler2D _NoiseTex;

        half2 _MovementDirection;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Normal;
            float2 uv_NoiseTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 pixel, alphaPixel;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.uv_NoiseTex += _MovementDirection * _Time.y / 2.0;
            IN.uv_MainTex += _MovementDirection * _Time.y;
            IN.uv_Normal += _MovementDirection * _Time.y / 2.0;

            alphaPixel = tex2D (_NoiseTex, IN.uv_NoiseTex);
            
            pixel = tex2D (_MainTex, IN.uv_MainTex) * _Color * alphaPixel.r;
            o.Albedo = pixel.rgb;

            o.Normal = tex2D(_Normal, IN.uv_Normal);
            
            o.Alpha = alphaPixel.r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
