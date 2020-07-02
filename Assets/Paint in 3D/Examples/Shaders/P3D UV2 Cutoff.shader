Shader "Paint in 3D/UV2 Cutoff"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Cutoff("Alpha cutoff", Range(0,.9)) = .5
	}
	SubShader
	{
		Tags
		{
			"Queue"      = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}
		LOD 200
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface Surf Standard alphatest:_Cutoff addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		//#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv2_MainTex;
			float4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void Surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv2_MainTex) * IN.color * _Color;
			o.Albedo     = c.rgb;
			o.Alpha      = c.a;
			o.Metallic   = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	} // SubShader
	FallBack "Standard"
} // Shader