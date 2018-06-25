using UnityEngine;

public class terrain_chunk_collider : MonoBehaviour 
{
	public Terrain BaseTerrain;
	public GameObject Player;
	public int ColliderResolution = 64;
	public bool Debug = false;
	TerrainData NewTerrainData;
	GameObject NewTerrain;
	TerrainData td;
	Vector2 scale;
		
	Vector2 world_to_heightmap(Terrain terrain,GameObject game_object)
	{
		TerrainData t = terrain.GetComponent<Terrain>().terrainData;
		float w = t.heightmapWidth;
		float h = t.heightmapHeight;
		float terrain_width = t.size.x;
		float terrain_length = t.size.z;
		float unitX = (w/terrain_width); 
		float unitY = (h/terrain_length);	
		float x = (unitX * (game_object.transform.position.x - terrain.transform.position.x));
		float y = (unitY * (game_object.transform.position.z - terrain.transform.position.z));		
		return new Vector2(x,y);	
	}

	void InitializeCollider (int res)   //set new terrain collider
	{
		td = BaseTerrain.GetComponent<Terrain>().terrainData;
		NewTerrainData = new TerrainData();
		NewTerrain = Terrain.CreateTerrainGameObject(NewTerrainData);
		NewTerrainData.heightmapResolution = res;	
		scale = new Vector2 (td.size.x/td.heightmapWidth,td.size.z/td.heightmapHeight);
		NewTerrainData.size = new Vector3(res*scale.x, td.size.y, res*scale.y);
		NewTerrainData.baseMapResolution = 16;
		NewTerrainData.SetDetailResolution(16, 16);
		NewTerrainData.alphamapResolution = 16;
	}
	
	void Start () 
	{
		InitializeCollider(ColliderResolution);
	}

	void Update() 
	{
		if (Debug)
			NewTerrain.GetComponent<Terrain>().enabled = true;
		else
			NewTerrain.GetComponent<Terrain>().enabled = false;		
		Vector2 h = world_to_heightmap(BaseTerrain,Player);
		int d = Mathf.RoundToInt(ColliderResolution*0.5f);
		float[,] geometry = td.GetHeights(Mathf.RoundToInt(h.x)-d,Mathf.RoundToInt(h.y)-d, ColliderResolution,ColliderResolution);
		NewTerrainData.SetHeights(0,0,geometry);
		Vector3 p = Player.transform.position;
		NewTerrain.transform.position = new Vector3(p.x-(d*scale.x),BaseTerrain.transform.position.y,p.z-(d*scale.y));
	}

}
