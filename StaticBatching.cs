using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StaticBatching : MonoBehaviour
{
	public enum Options {StaticBatching, MeshCombiner, Instancing};	
	public Options Option = Options.StaticBatching;
		
	GameObject _MainParent;

	float Random (Vector2 p ) 
	{
		return Mathf.Abs( (Mathf.Sin( p.x * 12.9898f + p.y * 78.233f ) * 43758.5453f) % 1);
	}

	void RunStaticBatcher(GameObject mainParent, List<GameObject> exceptions)
	{
		Transform[] transforms = mainParent.transform.gameObject.GetComponentsInChildren<Transform>();  //get all transforms from prefab into array
		List<Material> temp = new List<Material>();		
		for (int i=0; i<transforms.Length; i++)
		{
			Renderer renderer = transforms[i].gameObject.GetComponent<Renderer>();
			if (renderer)
				temp.Add(renderer.sharedMaterial);  //add material
		}
		List<Material> materials = temp.Distinct(new MaterialNameComparer()).ToList();	//remove copies, by name
		List<List<GameObject>> gameobjects = new List<List<GameObject>>();
		List<GameObject> parents = new List<GameObject>();
		for (int i=0; i<materials.Count; i++)  //initialize lists
		{
			gameobjects.Add(new List<GameObject>());
			parents.Add(new GameObject());
			parents[i].name = materials[i].name;
		}
		for (int i=0; i<transforms.Length; i++)  //group gameobjects by material type
		{
			if (exceptions.Contains(transforms[i].gameObject)) continue;
			Renderer renderer = transforms[i].gameObject.GetComponent<Renderer>();
			if (renderer)
			{
				for (int j=0; j<materials.Count; j++)
				{
					if (renderer.sharedMaterial.name == materials[j].name)
					{
						gameobjects[j].Add(transforms[i].gameObject);
						transforms[i].gameObject.transform.parent = parents[j].transform;
					}
				}
			}
		}
		for (int i=0; i<materials.Count; i++)  //run static batching for every group
		{
			parents[i].transform.parent = mainParent.transform;
			StaticBatchingUtility.Combine(gameobjects[i].ToArray(), parents[i]);
		}
	}

	void RunMeshCombiner(GameObject mainParent, string[] ignoreMaterialNames)
	{
		Transform[] transforms = mainParent.GetComponentsInChildren<Transform>();
		List<Material> temp = new List<Material>();
		for (int i=0; i<transforms.Length; i++)
		{
			Renderer renderer = transforms[i].gameObject.GetComponent<Renderer>();
			if (renderer)
				temp.Add(renderer.sharedMaterial); 
		}
		List<Material> materials = temp.Distinct(new MaterialNameComparer1()).ToList();			
		List<List<CombineInstance>> instances = new List<List<CombineInstance>>();
		for (int i=0; i<materials.Count; i++)
		{
			instances.Add(new List<CombineInstance>());
		}		
		for (int i=0;i<transforms.Length;i++)
		{
			Renderer renderer = transforms[i].gameObject.GetComponent<Renderer>();
			MeshFilter filter = transforms[i].gameObject.GetComponent<MeshFilter>();
			if (renderer && filter)
			{
				if (ignoreMaterialNames.Contains(renderer.sharedMaterial.name)) continue;
				string objectName = transforms[i].gameObject.name;
				if (objectName.Contains("LOD1") || objectName.Contains("LOD2") || objectName.Contains("LOD3")) continue;
				for (int j=0; j<materials.Count; j++)
				{
					if (renderer.sharedMaterial.name == materials[j].name)
					{
						CombineInstance combine = new CombineInstance();
						combine.mesh = filter.sharedMesh;
						combine.transform = filter.transform.localToWorldMatrix;
						instances[j].Add(combine);
					}
				}
			}
		}		
		for (int i=0; i<materials.Count; i++)
		{
			GameObject target = new GameObject();
			target.name = materials[i].name;
			MeshFilter filter = target.AddComponent<MeshFilter>();
			Renderer renderer = target.AddComponent<MeshRenderer>();
			Mesh mesh = new Mesh();
			mesh.name = materials[i].name;
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			mesh.CombineMeshes(instances[i].ToArray(), true, true);
			filter.sharedMesh = mesh;
			renderer.material = materials[i];
			target.transform.parent = mainParent.transform;
		}
		for (int i=0; i<transforms.Length; i++)
		{
			if (transforms[i].gameObject.GetComponent<MeshFilter>())
			{
				Destroy(transforms[i].gameObject.GetComponent<MeshFilter>().mesh);
			}
		}
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O) && (!_MainParent))
		{
			int i = 0;
			_MainParent = new GameObject();
			_MainParent.name = "MainParent";
			Material red = new Material(Shader.Find("Standard"));
			red.name = "Red";
			red.SetColor("_Color", new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
			if (Option == Options.Instancing) red.enableInstancing = true;
			Material blue = new Material(Shader.Find("Standard (Specular setup)"));
			blue.name = "Blue";
			blue.SetColor("_Color", new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
			if (Option == Options.Instancing) blue.enableInstancing = true;
			Camera.main.transform.position = new Vector3(50.0f, 4.0f, -50.0f);
			for (int x=0; x<64; x++)
			{
				for (int y=0; y<64; y++)
				{
					GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					if (Random(new Vector2(x,y)) < 0.5f) 
						sphere.GetComponent<Renderer>().sharedMaterial = red;
					else 
						sphere.GetComponent<Renderer>().sharedMaterial = blue;
					sphere.transform.position = new Vector3(x*2.0f, 0.0f, y*2.0f);
					sphere.transform.parent = _MainParent.transform;
					i++;
				}
			}
			if (Option == Options.MeshCombiner) RunMeshCombiner(_MainParent, new string[0]);
			if (Option == Options.StaticBatching) RunStaticBatcher(_MainParent, new List<GameObject>());
		}
		else if (Input.GetKeyDown(KeyCode.P) && (_MainParent))
		{
			Destroy(_MainParent);
			Resources.UnloadUnusedAssets();  //required to destroy CombineMesh
		}
	}
}

class MaterialNameComparer : IEqualityComparer<Material>
{
	public bool Equals(Material x, Material y)
	{
		return (x.name == y.name) ? true : false;
	}
 
	public int GetHashCode(Material obj)
	{
		return obj.name.GetHashCode();
	}
}