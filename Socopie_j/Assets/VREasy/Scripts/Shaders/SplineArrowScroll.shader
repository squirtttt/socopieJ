// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VREasy/SplineArrowScroll" {
		Properties
		{
			_DiffuseTex("Main Texture", 2D) = "white" {}
			_ScrollFlow("Scroll direction and speed", VECTOR) = (-1,0,0,0)
		}
			SubShader
		{
			Tags{ "Queue" = "Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha
				Cull Front

			Pass
		{

			CGPROGRAM
#pragma vertex v
#pragma fragment p

			sampler2D _DiffuseTex;
		float4 _DiffuseTex_ST;

		float4 _ScrollFlow;

		struct VertOut
		{
			float4 position : POSITION;
			float2 uv : TEXCOORD0;
		};

		VertOut v(float4 position : POSITION, float3 norm : NORMAL, float2 uv : TEXCOORD0)
		{
			VertOut OUT;

			OUT.position = UnityObjectToClipPos(position);
			OUT.uv = uv;


			return OUT;
		}

		struct PixelOut
		{
			float4 color : COLOR;
		};

		PixelOut p(VertOut input)
		{
			PixelOut OUT;

			float2 flowUV = input.uv *_DiffuseTex_ST.xy + _DiffuseTex_ST.zw + float2(_ScrollFlow.x * _Time.y, _ScrollFlow.y * _Time.y);
			OUT.color = tex2D(_DiffuseTex, flowUV);

			return OUT;
		}
		ENDCG
		}
				Cull Back
			Pass
		{

			CGPROGRAM
#pragma vertex v
#pragma fragment p

			sampler2D _DiffuseTex;
		float4 _DiffuseTex_ST;

		float4 _ScrollFlow;

		struct VertOut
		{
			float4 position : POSITION;
			float2 uv : TEXCOORD0;
		};

		VertOut v(float4 position : POSITION, float3 norm : NORMAL, float2 uv : TEXCOORD0)
		{
			VertOut OUT;

			OUT.position = UnityObjectToClipPos(position);
			OUT.uv = uv;


			return OUT;
		}

		struct PixelOut
		{
			float4 color : COLOR;
		};

		PixelOut p(VertOut input)
		{
			PixelOut OUT;

			float2 flowUV = input.uv *_DiffuseTex_ST.xy + _DiffuseTex_ST.zw + float2(_ScrollFlow.x * _Time.y, _ScrollFlow.y * _Time.y);
			OUT.color = tex2D(_DiffuseTex, flowUV);

			return OUT;
		}
		ENDCG
		}
				
	}
	FallBack "Diffuse"
}