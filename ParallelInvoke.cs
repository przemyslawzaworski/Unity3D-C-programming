using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class ParallelInvoke : MonoBehaviour
{
	static readonly float[,] _M = new float[,] { {0.8f,0.01f}, {0.01f,0.8f}};   //skew factor  

	float Hash (Vector2 p )  //generate pseudorandom number from (0..1) range
	{
		return Mathf.Abs( (Mathf.Sin( p.x*12.9898f+p.y*78.233f )  * 43758.5453f) % 1);
	}

	float Lerp (float a,float b, float t)  //linear interpolation
	{
		return Mathf.Lerp(a,b,t);
	}

	float Noise(Vector2 p)  //make random tiles with bilinear interpolation to create smooth surface
	{
		Vector2 i = new Vector2(Mathf.Floor(p.x),Mathf.Floor(p.y));
		Vector2 u = new Vector2 (Mathf.Abs (p.x % 1),Mathf.Abs (p.y % 1));
		u = new Vector2 (u.x*u.x*(3.0f-2.0f*u.x), u.y*u.y*(3.0f-2.0f*u.y));
		Vector2 a = new Vector2 (0.0f, 0.0f);
		Vector2 b = new Vector2 (1.0f, 0.0f);
		Vector2 c = new Vector2 (0.0f, 1.0f);
		Vector2 d = new Vector2 (1.0f, 1.0f);
		float r = Lerp(Lerp(Hash(i+a),Hash(i+b),u.x),Lerp(Hash(i+c),Hash(i+d),u.x),u.y);
		return r*r;
	}

	float Fbm( Vector2 p )  //deform tiles to get more organic looking surface
	{
		float f = 0.0f;
		f += 0.5000f*Noise( p );  p = p*2.02f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.2500f*Noise( p );  p = p*2.03f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.1250f*Noise( p );  p = p*2.01f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.0625f*Noise( p );
		return f/0.9375f;
	}

	byte[] Execute (int size, float seed)  //generate procedural pattern
	{
		int width = size;
		int height = size;
		int i = 0;
		byte[] source = new byte[width * height * 4];
		for (int y = 0; y < height; y++) 
		{
			for (int x = 0; x < width; x++) 
			{
				Vector2 resolution = new Vector2 (width,height);  
				Vector2 coordinates = new Vector2 ((float)x,(float)y); 
				Vector2 uv = new Vector2( (2.0f*coordinates.x-resolution.x)/resolution.y+1.0f, (2.0f*coordinates.y-resolution.y)/resolution.y +1.0f );			
				float a = Fbm(new Vector2(uv.x*5.0f+seed, uv.y*5.0f+seed));
				float b = Fbm(new Vector2(uv.y*5.0f+seed, uv.x*5.0f+seed));
				float f = Fbm(new Vector2(a, b));
				if (f<0.05f)
				{
					source[0 + i * 4] = (byte)((0.32f+f)*255);  //R channel
					source[1 + i * 4] = (byte)((0.32f+f)*255);  //G channel
					source[2 + i * 4] = (byte)((0.32f+f)*255);  //B channel
					source[3 + i * 4] = 255;  //A channel
				}
				else
				{
					source[0 + i * 4] = (byte)((0.46f+0.1*f)*255);  //R channel
					source[1 + i * 4] = (byte)((0.40f+0.1*f)*255);  //G channel
					source[2 + i * 4] = (byte)((0.27f+0.1*f)*255);  //B channel
					source[3 + i * 4] = 255;  //A channel
				}
				i++;
			}
		}
		return source;	
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))  //run multithreaded version with Parallel.Invoke (when running, see CPU usage in Task Manager)
		{
			int start = Environment.TickCount;
			int size = 256;  //texture size
			List<byte[]> data = new List<byte[]>();
			for (int i=0; i<8; i++) data.Add(new byte[size * size * 4]);
			Parallel.Invoke( () => data[0] = Execute(size, 10.0f), () => data[1] = Execute(size, 20.0f), () => data[2] = Execute(size, 30.0f), () => data[3] = Execute(size, 40.0f),
				() => data[4] = Execute(size, 50.0f), () => data[5] = Execute(size, 60.0f), () => data[6] = Execute(size, 70.0f),() => data[7] = Execute(size, 80.0f) );
			for (int i=0; i<8; i++)
			{
				Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
				texture.LoadRawTextureData(data[i]);
				texture.Apply(false, true);
				GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
				plane.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
				plane.GetComponent<Renderer>().material.mainTexture = texture;
				plane.transform.position = new Vector3(i * 12.0f, 0.0f, 0.0f);
			}
			data.Clear();
			data.TrimExcess();
			Debug.Log("Multithreading result: " + ((Convert.ToSingle(Environment.TickCount) - Convert.ToSingle(start)) * 0.001f).ToString() + " seconds");
		}

		if (Input.GetKeyDown(KeyCode.L))  //run singlethreaded version
		{
			int start = Environment.TickCount;
			int size = 256;  //texture size
			for (int i=0; i<8; i++)
			{
				byte[] data = Execute(size, i*10.0f + 10.0f);
				Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
				texture.LoadRawTextureData(data);
				texture.Apply(false, true);
				GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
				plane.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
				plane.GetComponent<Renderer>().material.mainTexture = texture;
				plane.transform.position = new Vector3(i * 12.0f, 0.0f, 0.0f);
			}	
			Debug.Log("Singlethreading result: " + ((Convert.ToSingle(Environment.TickCount) - Convert.ToSingle(start)) * 0.001f).ToString() + " seconds");
		}
	}
}
