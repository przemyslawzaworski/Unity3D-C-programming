using UnityEngine;

public class set_all_materials : MonoBehaviour 
{
	public GameObject target;
	public Material material;

	void SetAllMaterials(GameObject game_object, Material new_material)
	{
		Renderer[] children;
		children = game_object.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in children)
		{
			Material[] materials = new Material[renderer.materials.Length];
			for (int i = 0; i < renderer.materials.Length; i++) materials[i] = new_material;
			renderer.materials = materials;
		}
	}

	void Start () 
	{
		SetAllMaterials(target,material);
	}

}
