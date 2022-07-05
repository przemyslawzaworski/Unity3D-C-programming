using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
	public int Size = 32;
	[Range(0.0f, 1.0f)] public float Range = 0.2f;

	readonly float[,] _M = new float[,] { {0.8f, 0.01f}, {0.01f, 0.8f}};
	Material _Material;

	float Hash(Vector2 p) 
	{
		return Mathf.Abs( (Mathf.Sin( p.x * 12.9898f + p.y * 78.233f )  * 43758.5453f) % 1);
	}

	float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(a, b, t);
	}

	float Noise(Vector2 p)
	{
		Vector2 i = new Vector2(Mathf.Floor(p.x), Mathf.Floor(p.y));
		Vector2 u = new Vector2 (Mathf.Abs (p.x % 1), Mathf.Abs (p.y % 1));
		u = new Vector2 (u.x*u.x*(3.0f-2.0f*u.x), u.y*u.y*(3.0f-2.0f*u.y));
		Vector2 a = new Vector2 (0.0f, 0.0f);
		Vector2 b = new Vector2 (1.0f, 0.0f);
		Vector2 c = new Vector2 (0.0f, 1.0f);
		Vector2 d = new Vector2 (1.0f, 1.0f);
		float r = Lerp(Lerp(Hash(i+a),Hash(i+b),u.x), Lerp(Hash(i+c),Hash(i+d),u.x), u.y);
		return r * r;
	}

	float FractalNoise(Vector2 p)
	{
		float f = 0.0f;
		f += 0.5000f*Noise( p );  p = p*2.02f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.2500f*Noise( p );  p = p*2.03f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.1250f*Noise( p );  p = p*2.01f;  p = new Vector2(p.x*_M[0,0]+p.y*_M[0,1], p.x*_M[1,0]+p.y*_M[1,1]);
		f += 0.0625f*Noise( p );
		return f/0.9375f;
	}

	void GenerateLabyrinth()
	{
		GameObject toDelete = GameObject.Find("GenerateLabyrinth");
		if (toDelete) Destroy(toDelete);
		if (_Material == null) _Material = new Material(Shader.Find("Standard"));
		Resources.UnloadUnusedAssets();
		GameObject labyrinth = new GameObject(name: "GenerateLabyrinth");
		labyrinth.transform.position = Vector3.zero;
		float shift = Random.Range(-256.0f, 256.0f);
		List<CombineInstance> instances = new List<CombineInstance>();
		for (int y = 0; y < Size; y++) 
		{
			for (int x = 0; x < Size; x++) 
			{
				Vector2 coordinates = new Vector2 ((float)x, (float)y); 
				float result = FractalNoise(new Vector2(Mathf.Ceil(coordinates.x + shift), Mathf.Ceil(coordinates.y + shift)));
				if (result < Range || y == 0 || y == (Size - 1))
				{
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3((float)x, 0.0f, (float)y);
					cube.transform.parent = labyrinth.transform;
					CombineInstance combineInstance = new CombineInstance();
					combineInstance.mesh = cube.GetComponent<MeshFilter>().sharedMesh;
					combineInstance.transform = cube.transform.localToWorldMatrix;
					cube.SetActive(false);
					instances.Add(combineInstance);
				}
			}
		}
		MeshFilter meshFilter = labyrinth.AddComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(instances.ToArray());
		MeshRenderer meshRenderer = labyrinth.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = _Material;
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GenerateLabyrinth();
		}
	}
}