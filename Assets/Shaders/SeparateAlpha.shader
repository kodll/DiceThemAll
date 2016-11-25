Shader "SeparateAlpha" {

	Properties
	{
		_ColorTint("Colour Tint",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white"{}
	_Mask("Alpha Mask",2D) = "white"{}
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha
		struct Input
	{
		float4 color : COLOR;
		float2 uv_MainTex;
		float2 uv_Mask;
	};

	float4 _ColorTint;
	sampler2D _MainTex;
	sampler2D _Mask;

	void surf(Input IN, inout SurfaceOutput o)
	{
		IN.color = _ColorTint;
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
		o.Alpha = tex2D(_Mask, IN.uv_Mask).r;
	}
	ENDCG
	}

		Fallback "Diffuse"

}