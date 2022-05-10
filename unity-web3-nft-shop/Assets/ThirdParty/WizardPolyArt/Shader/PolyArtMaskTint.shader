// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyArtMaskTint"
{
	Properties
	{
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Hair("Hair", Color) = (0,0,0,0)
		_InnerCloth("InnerCloth", Color) = (0,0,0,0)
		_OuterChlothes("OuterChlothes", Color) = (0,0,0,0)
		_PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
		_PolyArtMask("PolyArtMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _PolyArtAlbedo;
		uniform float4 _PolyArtAlbedo_ST;
		uniform sampler2D _PolyArtMask;
		uniform float4 _PolyArtMask_ST;
		uniform float4 _OuterChlothes;
		uniform float4 _InnerCloth;
		uniform float4 _Hair;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PolyArtAlbedo = i.uv_texcoord * _PolyArtAlbedo_ST.xy + _PolyArtAlbedo_ST.zw;
			float4 tex2DNode16 = tex2D( _PolyArtAlbedo, uv_PolyArtAlbedo );
			float2 uv_PolyArtMask = i.uv_texcoord * _PolyArtMask_ST.xy + _PolyArtMask_ST.zw;
			float4 tex2DNode13 = tex2D( _PolyArtMask, uv_PolyArtMask );
			float4 temp_cast_0 = (tex2DNode13.r).xxxx;
			float4 temp_cast_1 = (tex2DNode13.g).xxxx;
			float4 temp_cast_2 = (tex2DNode13.b).xxxx;
			float4 blendOpSrc22 = tex2DNode16;
			float4 blendOpDest22 = ( min( temp_cast_0 , _OuterChlothes ) + min( temp_cast_1 , _InnerCloth ) + min( temp_cast_2 , _Hair ) );
			float4 lerpResult4 = lerp( tex2DNode16 , ( ( saturate( ( blendOpSrc22 * blendOpDest22 ) )) * 2.0 ) , ( tex2DNode13.r + tex2DNode13.g + tex2DNode13.b ));
			o.Albedo = lerpResult4.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
-113;105;1906;1004;2144.636;504.9833;1.29636;True;False
Node;AmplifyShaderEditor.ColorNode;9;-1525.123,67.83549;Float;False;Property;_OuterChlothes;OuterChlothes;4;0;Create;True;0;0;False;0;0,0,0,0;0.07552986,0.4010895,0.9338235,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-1579.669,-173.1747;Float;True;Property;_PolyArtMask;PolyArtMask;6;0;Create;True;0;0;False;0;c47c9719298650c47a7f2042127ff9d8;c47c9719298650c47a7f2042127ff9d8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-1506.95,284.0871;Float;False;Property;_InnerCloth;InnerCloth;3;0;Create;True;0;0;False;0;0,0,0,0;0.2509804,0,0.7803922,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-1514.219,469.4455;Float;False;Property;_Hair;Hair;2;0;Create;True;0;0;False;0;0,0,0,0;1,0.6431373,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMinOpNode;15;-1169.775,-59.03537;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;17;-1171.07,193.755;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;18;-1191.812,416.7289;Float;True;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-883.2778,109.4914;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-1362.932,-392.1999;Float;True;Property;_PolyArtAlbedo;PolyArtAlbedo;5;0;Create;True;0;0;False;0;4960428282afaef44ab798134d3c2db0;4960428282afaef44ab798134d3c2db0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;22;-550.1132,39.488;Float;False;Multiply;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-481.4059,213.2003;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-267.5063,3.189942;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-1002.543,-224.9695;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;4;-267.633,-332.1622;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-326,261;Float;False;Property;_Metallic;Metallic;1;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-325,351;Float;False;Property;_Smoothness;Smoothness;0;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89,-105;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PolyArtMaskTint;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;13;1
WireConnection;15;1;9;0
WireConnection;17;0;13;2
WireConnection;17;1;10;0
WireConnection;18;0;13;3
WireConnection;18;1;11;0
WireConnection;19;0;15;0
WireConnection;19;1;17;0
WireConnection;19;2;18;0
WireConnection;22;0;16;0
WireConnection;22;1;19;0
WireConnection;23;0;22;0
WireConnection;23;1;24;0
WireConnection;20;0;13;1
WireConnection;20;1;13;2
WireConnection;20;2;13;3
WireConnection;4;0;16;0
WireConnection;4;1;23;0
WireConnection;4;2;20;0
WireConnection;0;0;4;0
WireConnection;0;3;1;0
WireConnection;0;4;2;0
ASEEND*/
//CHKSM=BFFB068BC4C2B03824531D934752F2EA40D51013