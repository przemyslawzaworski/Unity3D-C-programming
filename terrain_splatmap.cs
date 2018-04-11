//Written by Przemyslaw Zaworski
//Script detects terrain splat map name from game object position

using UnityEngine;

public class terrain_splatmap : MonoBehaviour 
{
	public Terrain terrain;
	public GameObject player;
	string label;
	float[,,] alphamaps;
	TerrainData terrain_data;
	
	string GetTerrainTextureName (Terrain terrain, Vector3 input ) 
	{
		TerrainData td = terrain.GetComponent<Terrain>().terrainData;
		float unitX = (td.alphamapWidth/td.size.x); 
		float unitY = (td.alphamapHeight/td.size.z);	
		int x = Mathf.RoundToInt(unitX * (input.x - terrain.transform.position.x));
		int y = Mathf.RoundToInt(unitY * (input.z - terrain.transform.position.z));	
		float[,,] maps = alphamaps;
		int count = td.splatPrototypes.Length;
		float highest = 0.0f;
		string name = "";
		if (x<=0 || x>=td.alphamapWidth || y<=0 || y>=td.alphamapHeight)
		{			
			return "None";
		}
		else
		{
			for (int i=0;i<count;i++)
			{
				float fvalue = maps[y,x,i];
				if (fvalue>highest) 
				{
					highest = fvalue;
					name = td.splatPrototypes[i].texture.ToString();
				}
			}
		}
		return name;
	}
	
	void Start()
	{
		TerrainData terrain_data = terrain.GetComponent<Terrain>().terrainData;		
		alphamaps = terrain_data.GetAlphamaps(0, 0, terrain_data.alphamapWidth, terrain_data.alphamapHeight);
	}
	
	void Update () 
	{
		label = GetTerrainTextureName(terrain,player.transform.position);
	}
	
	void OnGUI()
	{
		GUIStyle guiStyle = new GUIStyle();
		guiStyle.fontSize = 20;
		GUI.Label(new Rect(10, 10, 300, 40), label);
	}
}
