Shader "VREasy/ColourImage" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Value("Interpolation value", Float) = 0
		_Colour("Colour", Color) = (1,1,1,1)
	}

		SubShader{
			Pass{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _RampTex;
			uniform half _Value;
			uniform float4 _Colour;
			half4 _MainTex_ST;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 original = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
				fixed4 output = lerp(original,_Colour,_Value);
				return output;
			}
			ENDCG

		}
	} Fallback off

}
