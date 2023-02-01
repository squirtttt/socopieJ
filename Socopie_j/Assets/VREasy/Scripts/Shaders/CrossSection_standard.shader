// Simple shader based on the work by https://github.com/Dandarawy/Unity3DCrossSectionShader/wiki
Shader "VREasy/CrossSection_standard" {

	Properties{
		_Colour("Colour", Color) = (1,1,1,1)
		_CrossColour("Cross Section Colour", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_PlaneNormal("PlaneNormal",Vector) = (0,1,0,0)
		_PlanePosition("PlanePosition",Vector) = (0,0,0,1)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		Stencil
		{
		Ref[_StencilMask]
		CompBack Always
		PassBack Replace

		CompFront Always
		PassFront Zero
		}
		Cull Back
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;

			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Colour;
		fixed3 _PlaneNormal;
		fixed3 _PlanePosition;
		bool isNotVisible(fixed3 worldPos)
		{
			return dot(worldPos - _PlanePosition, _PlaneNormal) > 0;
		}
	
		void surf(Input IN, inout SurfaceOutputStandard o) {

		
			if (isNotVisible(IN.worldPos)) discard;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Colour;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		
		}
	ENDCG

	Cull Front
	CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0

	struct Input {
		half2 uv_MainTex;
		float3 worldPos;
	};
	fixed4 _CrossColour;
	fixed3 _PlaneNormal;
	fixed3 _PlanePosition;

	bool isNotVisible(fixed3 worldPos)
	{
		return dot(worldPos - _PlanePosition, _PlaneNormal) > 0;
	}

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		if (isNotVisible(IN.worldPos)) discard;
		o.Albedo = _CrossColour;
	}
	ENDCG

	} //Fallback "Diffuse"
}

