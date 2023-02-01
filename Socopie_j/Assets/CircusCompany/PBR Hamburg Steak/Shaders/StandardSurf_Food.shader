Shader "CircusCompany/StandardSurf_Food" {
	Properties {
		_MainColor("Color", Color) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white" {}		
		_MainTexDouble ("MainTex Color Double", float) = 2
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Intensity", float) = 1
		_RoughnessTex("RoughnessTex", 2D) = "white" {}
		_Roughness ("Roughness", Range(0,1)) = 0.5
		_MetallicTex ("MetallicTex", 2D) = "white" {}
		_Metallic ("Metallic", Range(0,1)) = 0.5		
		_OcclusionTex("OcclusionTex", 2D) = "white" {}
		_OcclusionIntensity("Occlusion Intensity", Range(0,1)) = 1
		
		[Space(30)]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Range(0.01,8.0)) = 3

		[Space(30)]
		[HDR]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
		_EmissiveTex("EmissiveTex", 2D) = "white" {}

		[Space(30)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("2side Setting", Float) = 2
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }		

		Cull[_Cull]

		CGPROGRAM		
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _NoiseTex;
		sampler2D _MetallicTex;
		sampler2D _RoughnessTex;
		sampler2D _EmissiveTex;
		sampler2D _OcclusionTex;

		struct Input {
			float2 uv_MainTex;			
			float2 uv_NoiseTex;
			fixed3 viewDir;
		};			

		float _BumpScale;
		float _MainTexDouble;
		float _OcclusionIntensity;
		float4 _EmissiveColor;
		half _Roughness;
		half _Metallic;
		fixed4 _MainColor;

		fixed4 _RimColor;
		fixed _RimPower;

		void surf (Input IN, inout SurfaceOutputStandard o) {			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _MainColor;
			fixed3 nm = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			fixed4 Mt = tex2D(_MetallicTex, IN.uv_MainTex);
			fixed4 Rh = tex2D(_RoughnessTex, IN.uv_MainTex);
			fixed4 AO = tex2D(_OcclusionTex, IN.uv_MainTex);
			fixed3 Em = tex2D(_EmissiveTex, IN.uv_MainTex) * _EmissiveColor;

			//float3 AP = saturate(c.rgb) * _MainTexDouble;
			
			o.Albedo = c.rgb;
			o.Albedo *= c.rgb * _MainTexDouble;
			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
			o.Occlusion = (AO * _OcclusionIntensity ) + 0.5;
			fixed3 view = normalize(IN.viewDir);
			fixed3 nml = o.Normal;
			fixed VdN = dot(view, nml);
			fixed rim = 1.0 - saturate(VdN);
			o.Emission = (Em + _RimColor.rgb) * pow(rim, _RimPower);
			o.Metallic = _Metallic * Mt;
			o.Smoothness = _Roughness * (1-Rh);

		}
		ENDCG
	}
	FallBack "Diffuse"
			
}