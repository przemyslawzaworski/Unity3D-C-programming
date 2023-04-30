Shader "Vertex Ambient Occlusion" 
{
	Properties 
	{
		_BaseColor ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex VSMain
			#pragma fragment PSMain

			float4 _BaseColor;

			float4 VSMain (float4 vertex : POSITION, inout float4 color : COLOR) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			float4 PSMain (float4 vertex : SV_POSITION, float4 color : COLOR) : SV_Target 
			{
				return float4(color.www * _BaseColor.rgb, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}