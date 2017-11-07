//Author: Przemyslaw Zaworski
//Add script to Main Camera. Play.
using UnityEngine;
using System.Collections;
using System.IO;

public class heightmap_generator : MonoBehaviour 
{
	static readonly float[,] m = new float[,] { {0.8f,0.01f}, {0.01f,0.8f}};   
	
	float hash (Vector2 p ) 
	{
		return Mathf.Abs( (Mathf.Sin( p.x*12.9898f+p.y*78.233f )  * 43758.5453f) % 1);
	}
	
	float lerp (float a,float b, float t)
	{
		return Mathf.Lerp(a,b,t);
	}

	float noise(Vector2 p)
	{
		Vector2 i = new Vector2(Mathf.Floor(p.x),Mathf.Floor(p.y));
		Vector2 u = new Vector2 (Mathf.Abs (p.x % 1),Mathf.Abs (p.y % 1));
		u=new Vector2 (u.x*u.x*(3.0f-2.0f*u.x),u.y*u.y*(3.0f-2.0f*u.y));
		Vector2 a = new Vector2 (0.0f,0.0f);
		Vector2 b = new Vector2 (1.0f,0.0f);
		Vector2 c = new Vector2 (0.0f,1.0f);
		Vector2 d = new Vector2 (1.0f,1.0f);
		float r = lerp(lerp(hash(i+a),hash(i+b),u.x),lerp(hash(i+c),hash(i+d),u.x),u.y);
		return r*r;
	}
			
	float fbm( Vector2 p )
	{
		float f = 0.0f;
		f += 0.5000f*noise( p ); p = p*2.02f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
		f += 0.2500f*noise( p ); p = p*2.03f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
		f += 0.1250f*noise( p ); p = p*2.01f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
		f += 0.0625f*noise( p );
		return f/0.9375f;
	}

	void generate_terrain ()
	{
		float r = Random.Range(0.0f,100.0f);
		TerrainData _terraindata = new TerrainData();
		GameObject terrain = Terrain.CreateTerrainGameObject(_terraindata);
		float[,] table = new float[512,512];
		for (int y = 0; y < 512; y++) 
		{
			for (int x = 0; x < 512; x++) 
			{
				Vector2 resolution = new Vector2 (512,512);  
				Vector2 coordinates = new Vector2 ((float)x,(float)y); 
				Vector2 uv = new Vector2( (2.0f*coordinates.x-resolution.x)/resolution.y+1.0f,(2.0f*coordinates.y-resolution.y)/resolution.y +1.0f );			
				table[x,y]=fbm(new Vector2(uv.x*5.0f+r,uv.y*5.0f+r))+0.1f;
			}
		}
		_terraindata.size = new Vector3(512, 10.0f*r+500.0f, 512);
		_terraindata.heightmapResolution = 512;
		_terraindata.baseMapResolution = 1024;
		_terraindata.SetDetailResolution(1024, 16);
		_terraindata.SetHeights(0,0,table);
		Terrain terrain_component = terrain.GetComponent<Terrain>(); 
		Material surface = new Material(Shader.Find("Legacy Shaders/Diffuse"));
		surface.color=Color.gray;
		terrain_component.materialType = Terrain.MaterialType.Custom;
		terrain_component.materialTemplate = surface;
	}

	void Start()
	{
		transform.position = new Vector3(4820.0f,3720.0f,9850.0f);
		transform.Rotate(new Vector3(40,180,0));
		GetComponent< Camera >().farClipPlane = 10000.0f;
	}

	void OnGUI() 
	{
		if (GUI.Button(new Rect(10, 10, 200, 50), "Generate new terrain")) 
		{
			Destroy(GameObject.Find("New Game Object"));
			Destroy(GameObject.Find("Terrain"));
			generate_terrain();
		}
	}
}