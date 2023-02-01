// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Based on: https://en.wikibooks.org/wiki/Cg_Programming/Unity/Skyboxes

Shader "VREasy/PanoramaSkybox" {
		Properties{
			_Cube("Cubemap", Cube) = "" {}
		}
			SubShader{
			Tags{ "Queue" = "Geometry" }

			Pass{
			//ZWrite Off
			//Cull Front

			CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

#include "UnityCG.cginc"

			// User-specified uniforms
			uniform samplerCUBE _Cube;

		struct vertexInput {
			float4 vertex : POSITION;
		};
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float3 viewDir : TEXCOORD1;
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;

			float4x4 modelMatrix = unity_ObjectToWorld;
			output.viewDir = mul(modelMatrix, input.vertex).xyz
				- _WorldSpaceCameraPos;
			output.pos = UnityObjectToClipPos(input.vertex);
			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
			return texCUBE(_Cube, input.viewDir);
		}

			ENDCG
		}
		}
	}