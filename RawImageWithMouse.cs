using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageWithMouse : MonoBehaviour
{
	public Material Material;
	public RawImage Image;

	Vector3[] _Corners = new Vector3[4];
	Vector4 _Mouse = Vector4.zero;
	Vector4 _Resolution = Vector4.zero;
	Rect _Rect = Rect.zero;
	
	const string iMouse = "iMouse";
	const string iResolution = "iResolution";

	void Update()
	{
		Image.rectTransform.GetWorldCorners(_Corners);
		_Rect.position = _Corners[0];
		_Rect.size = _Corners[2] - _Corners[0];
		_Mouse.x = Input.mousePosition.x - _Rect.x;
		_Mouse.y = Input.mousePosition.y - _Rect.y;
		Material.SetVector(iMouse, _Mouse);
		_Resolution.x = Image.rectTransform.rect.width;
		_Resolution.y = Image.rectTransform.rect.height;
		Material.SetVector(iResolution, _Resolution);
	}
}

/* Script can be tested with following shader:

Shader "WhiteCircle"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex VSMain
			#pragma fragment PSMain
			
			float4 iMouse;
			float4 iResolution;

			float Sphere (float2 p, float2 c, float r)
			{
				return step(length(p-c)-r, 0.0);
			}

			void VSMain (inout float4 vertex:POSITION, inout float2 uv:TEXCOORD0)
			{
				vertex = UnityObjectToClipPos(vertex);
			}

			void PSMain (float4 vertex:POSITION, float2 uv:TEXCOORD0, out float4 fragColor:SV_TARGET)
			{
				float s = Sphere(uv, iMouse.xy/iResolution.xy, 0.1);
				fragColor = float4(s.xxx, 1.0);
			}
			ENDCG
		}
	}
}
*/
