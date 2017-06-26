using UnityEngine;
using System.Collections;
using System.IO;

public class raymarching : MonoBehaviour 
{	
	float sphere (Vector3 p,Vector3 c,float r)
	{
		return Vector3.Distance(p,c)-r;
	}
	
	float map (Vector3 p)
	{
		return sphere (p,new Vector3(0.0f,0.0f,0.0f),1.0f);
	}
	
	Vector3 set_normal (Vector3 p)
	{
		Vector3 x = new Vector3 (0.01f,0.00f,0.00f);
		Vector3 y = new Vector3 (0.00f,0.01f,0.00f);
		Vector3 z = new Vector3 (0.00f,0.00f,0.01f);
		return Vector3.Normalize(new Vector3(map(p+x)-map(p-x), map(p+y)-map(p-y), map(p+z)-map(p-z))); 
	}
		
	Vector3 lighting ( Vector3 p)
	{
		Vector3 AmbientLight = new Vector3 (0.1f,0.1f,0.1f);
		Vector3 LightDirection = Vector3.Normalize(new Vector3(4.0f,10.0f,-10.0f));
		Vector3 LightColor = new Vector3 (1.0f,1.0f,1.0f);
		Vector3 NormalDirection = set_normal(p);
		return  (Vector3)(Mathf.Max ( Vector3.Dot(LightDirection, NormalDirection),0.0f) * LightColor + AmbientLight);
	}

	Vector4 raymarch (Vector3 ro,Vector3 rd)
	{
		for (int i=0;i<16;i++)
		{
			float t = map(ro);
			Vector3 l = lighting(ro);
			if (t<0.01f) return new Vector4(l.x,l.y,l.z,1.0f); 
			else ro+=t*rd;
		}
		return new Vector4(0.0f,0.0f,0.0f,1.0f);
	}	
		
	void Start ()
	{
		Vector2 resolution = new Vector2 (512,512); 
		Texture2D image = new Texture2D ((int)resolution.x,(int)resolution.y, TextureFormat.RGBA32, false);
		for (int y = 0; y < (int)resolution.y; y++) 
		{
			for (int x = 0; x <(int)resolution.x; x++) 
			{ 
				Vector2 coordinates = new Vector2 (x,y);
				Vector2 p = new Vector2 ((2.0f*coordinates.x-resolution.x)/resolution.y,(2.0f*coordinates.y-resolution.y)/resolution.y);
				Vector3 ro = new Vector3 (0.0f,0.0f,-10.0f);
				Vector3 rd = Vector3.Normalize( new Vector3(p.x,p.y,2.0f));
				Vector4 c = raymarch(ro,rd);
				Color color = new Color (c.x,c.y,c.z, 1.0f);
				image.SetPixel (x, y, color);
			}
		}
		image.Apply ();
		byte[] bytes = image.EncodeToPNG ();
		Object.Destroy (image);
		File.WriteAllBytes ("C:\\raymarching.png", bytes);
	}

}