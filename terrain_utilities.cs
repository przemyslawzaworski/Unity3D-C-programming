using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrain_utilities : MonoBehaviour 
{
	public Terrain terrain;

	float GetTerrainMaximumHeightLocalSpace (Terrain terrain)
	{
		return terrain.terrainData.bounds.max.y;
	}

	float GetTerrainMaximumHeightWorldSpace (Terrain terrain)	
	{
		float a = terrain.terrainData.bounds.max.y;
		float b = terrain.transform.position.y;	
		return a+b;
	}
	
	float GetTerrainMaximumHeightNormalized (Terrain terrain)
	{
		float a = terrain.terrainData.bounds.max.y;
		float b = terrain.terrainData.size.y;	
		return a/b;
	}
	
}
