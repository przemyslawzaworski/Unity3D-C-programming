Shader "Hidden/GenerateIsolines"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_SampleBase ("Sample Base", Range (0.001,1.0)) = 0.05
		_SampleDetail ("Sample Detail", Range (0.001,1.0)) = 0.005
		_Inverted ("Color Inversion", Float) = 0.0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex VSMain
			#pragma fragment PSMain

			float _SampleBase, _SampleDetail, _Inverted;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float4 VSMain(float4 vertex : POSITION, inout float2 uv : TEXCOORD0) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex); 
			}

			float4 PSMain(float4 vertex : SV_POSITION, float2 uv : TEXCOORD0) : SV_Target0
			{
				float3 delta = float3(_MainTex_TexelSize.xy, 0.0);
				float m = tex2D(_MainTex, float2(uv)).r;
				float a = tex2D(_MainTex, float2(uv + delta.xz)).r;
				float b = tex2D(_MainTex, float2(uv + delta.zy)).r;
				float c = tex2D(_MainTex, float2(uv - delta.xz)).r;
				float d = tex2D(_MainTex, float2(uv - delta.zy)).r;
				float e = abs(frac(m / _SampleBase + 0.5) - 0.5);
				float f = abs(frac(m / _SampleDetail + 0.5) - 0.5);
				float base = 1.0 - clamp(abs(e) / length(float2(a - c, b - d) / _SampleBase), 0.0, 1.0);
				float detail = 1.0 - clamp(abs(f) / length(float2(a - c, b - d) / _SampleDetail), 0.0, 1.0);
				float isoline  = 0.5 * base + 0.5 * detail;     
				return (_Inverted < 1.0) ? float4(1.0 - isoline.xxx, 1.0) : float4(isoline.xxx, 1.0);
			}
			ENDCG
		}
	}
}