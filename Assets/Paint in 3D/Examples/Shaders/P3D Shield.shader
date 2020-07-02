Shader "Paint in 3D/Shield"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue"      = "Transparent"
			"RenderType" = "Transparent"
		}
		LOD 200
		Blend One One
		Cull Off
		
		CGPROGRAM
		#pragma surface Surf NoLighting

		fixed4    _Color;
		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

		void Surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color * _Color;
			o.Albedo = c.rgb;
			o.Alpha  = 0.0f;
		}
		ENDCG
	} // SubShader
	FallBack "Standard"
} // Shader