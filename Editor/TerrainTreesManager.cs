using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Data;
using System.Linq;

public class TerrainTreesManager : EditorWindow
{
	[MenuItem("Assets/Terrain Trees Manager")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(TerrainTreesManager));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Sort And Combine Trees For All Terrains" ) )
		{
			Transform[] transforms = Selection.transforms;
			List<Terrain> terrains = new List<Terrain>();
			for (int i = 0; i < transforms.Length; i++)
			{
				Terrain terrain = transforms[i].gameObject.GetComponent<Terrain>();
				if (terrain != null) terrains.Add(terrain);
			}
			SortTreePrototypesForAllTerrains(terrains.ToArray());
		}		
	} 

	void SortTreePrototypesForTerrain(Terrain terrain)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("PrefabName", typeof(string));
		dataTable.Columns.Add("PrefabPath", typeof(string));
		dataTable.Columns.Add("BendFactor", typeof(float));
		dataTable.Columns.Add("NavMeshLod", typeof(int));
		dataTable.Columns.Add("OldIndex", typeof(int));
		TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
		for (int i = 0; i < treePrototypes.Length; i++)
		{
			TreePrototype tp = treePrototypes[i];
			dataTable.Rows.Add(tp.prefab.name, AssetDatabase.GetAssetPath(tp.prefab), tp.bendFactor, tp.navMeshLod, i);
		}
		DataView dataView = new DataView(dataTable);
		dataView.Sort = "PrefabName";
		dataTable = dataView.ToTable();
		dataTable.Columns.Add("NewIndex", typeof(int));
		int newIndex = 0;
		TreePrototype[] newPrototypes = new TreePrototype[treePrototypes.Length];
		foreach (DataRow row in dataTable.Rows)
		{
			row["NewIndex"] = newIndex;
			TreePrototype tp = new TreePrototype();
			tp.bendFactor = (float)row["BendFactor"];
			tp.navMeshLod = (int)row["NavMeshLod"];
			tp.prefab = (GameObject)AssetDatabase.LoadAssetAtPath((string)row["PrefabPath"], typeof(GameObject));
			newPrototypes[newIndex] = tp;
			newIndex++;
		}
		terrain.terrainData.treePrototypes = newPrototypes;
		terrain.terrainData.RefreshPrototypes();
		dataView = new DataView(dataTable);
		dataView.Sort = "OldIndex";
		dataTable = dataView.ToTable();
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		for (int i = 0; i < treeInstances.Length; i++)
		{
			TreeInstance treeInstance = treeInstances[i];
			treeInstance.prototypeIndex = (int)dataTable.Rows[treeInstance.prototypeIndex]["NewIndex"];
			treeInstances[i] = treeInstance;
		}
		terrain.terrainData.treeInstances = treeInstances;
		terrain.gameObject.SetActive(false);
		terrain.gameObject.SetActive(true);
	}

	void AddPrototypeToTerrain (Terrain terrain, TreePrototype treePrototype)
	{
		List<TreePrototype> treePrototypes = terrain.terrainData.treePrototypes.ToList();
		treePrototypes.Add(treePrototype);
		terrain.terrainData.treePrototypes = treePrototypes.ToArray();	
	}

	bool IsPrototypeInDataTable (DataTable dataTable, TreePrototype treePrototype)
	{
		foreach (DataRow row in dataTable.Rows)
		{
			if ((string)row["PrefabName"] == treePrototype.prefab.name)
			{
				return true;
			}
		}
		return false;
	}

	void SortTreePrototypesForAllTerrains (Terrain[] terrains)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("PrefabName", typeof(string));
		dataTable.Columns.Add("PrefabPath", typeof(string));
		dataTable.Columns.Add("BendFactor", typeof(float));
		dataTable.Columns.Add("NavMeshLod", typeof(int));
		for (int i = 0; i < terrains.Length; i++)
		{
			TreePrototype[] treePrototypes = terrains[i].terrainData.treePrototypes;
			for (int j = 0; j < treePrototypes.Length; j++)
			{
				TreePrototype tp = treePrototypes[j];
				if (IsPrototypeInDataTable (dataTable, tp) == false)
					dataTable.Rows.Add(tp.prefab.name, AssetDatabase.GetAssetPath(tp.prefab), tp.bendFactor, tp.navMeshLod);
			}
		}
		for (int i = 0; i < terrains.Length; i++)
		{
			TreePrototype[] treePrototypes = terrains[i].terrainData.treePrototypes;
			for (int j = 0; j < dataTable.Rows.Count; j++)
			{
				DataRow row = dataTable.Rows[j];
				bool result = false;
				for (int k = 0; k < treePrototypes.Length; k++)
				{
					if ((string)row["PrefabName"] == treePrototypes[k].prefab.name) result = true;
				}
				if (result == false)
				{
					TreePrototype tp = new TreePrototype();
					tp.bendFactor = (float)row["BendFactor"];
					tp.navMeshLod = (int)row["NavMeshLod"];
					tp.prefab = (GameObject)AssetDatabase.LoadAssetAtPath((string)row["PrefabPath"], typeof(GameObject));
					AddPrototypeToTerrain (terrains[i], tp);
				}
			}
			SortTreePrototypesForTerrain(terrains[i]);
		}
	}
}