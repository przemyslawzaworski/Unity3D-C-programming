using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Serialization : MonoBehaviour 
{
	[System.Serializable]
	struct Map
	{
		public float[,] heightmap;
		public float[,] alphamap;
	};
	
	void PrintArray (float[,] table)
	{
		for (int x=0;x<table.GetLength(0);x++)
		{
			for (int y=0;y<table.GetLength(1);y++)
			{
				Debug.Log(table[x,y]);
			}
		}
	}
	
	void Serialize (string path, Map source)
	{
		try
		{
			BinaryFormatter bin = new BinaryFormatter();
			FileStream writer = new FileStream(path,FileMode.Create);
			bin.Serialize(writer, (object)source);
			writer.Close();
		}
		catch (IOException) {}
	}
	
	Map Deserialize (string path)
	{
		FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read);
		BinaryFormatter bin = new BinaryFormatter();
		Map target = (Map) bin.Deserialize(reader);	
		reader.Close();
		return target;
	}
	
	void Start()
	{
		string Path = Application.dataPath + "/StreamingAssets/" + "map.data";
		Map source;
		source.heightmap = new float[2,2] {{1.0f,2.0f},{3.0f,4.0f}};
		source.alphamap  = new float[2,2] {{5.0f,6.0f},{7.0f,8.0f}};
		Serialize (Path,source);
		Map destination = Deserialize (Path);
		PrintArray(destination.heightmap);
		PrintArray(destination.alphamap);
	}
}