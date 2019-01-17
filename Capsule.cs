using UnityEngine;

public class Capsule : MonoBehaviour
{	
	float smoothstep (float edge0, float edge1, float x)
	{
		float t = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
		return t * t * (3.0f - 2.0f * t);
	}
	
	float capsule( Vector2 p, Vector2 a, Vector2 b, float r, Vector2 s, float d )
	{
		Vector2 pa = p - a, ba = b - a;
		float h = Mathf.Clamp(Vector2.Dot(pa,ba) / Vector2.Dot(ba,ba), 0.0f, 1.0f);
		return smoothstep(s.x, s.y, (pa - ba*h).magnitude - r) * d;
	}

	void Start ()
	{
		Vector2 resolution = new Vector2 (512, 512); 
		Texture2D image = new Texture2D ((int)resolution.x,(int)resolution.y, TextureFormat.RGBA32, false);
		for (int y = 0; y < (int)resolution.y; y++) 
		{
			for (int x = 0; x <(int)resolution.x; x++) 
			{ 
				Vector2 uv = new Vector2(x, y) - 0.5f * resolution;  // from -256 to 256
				float k = capsule(uv, new Vector2(0.0f, -100.0f), new Vector2(0.0f, 100.0f), 30.0f, new Vector2(50.0f, 1.0f), 1.0f);
				Color color = new Color(k, k, k, 1.0f);
				image.SetPixel (x, y, color);
			}
		}
		image.Apply();
		this.GetComponent<Renderer>().material.SetTexture("_MainTex",image);
	}
}
