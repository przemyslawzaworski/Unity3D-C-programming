using UnityEngine;

public class LightMapManager : MonoBehaviour
{
	[Header("Lightmap-0_comp_light.exr")]
	[SerializeField] public Texture2D LightMap;
	
	public float ScaleX = 1.0f;
	public float ScaleY = 1.0f;
	public float OffsetX = 1.0f;
	public float OffsetY = 1.0f;
		
	public static LightMapManager Instance;
	
	void Awake()
	{
		Instance = this;
	}
 
	public void SetLightMap() 
	{
		LightmapData[] lightmaparray = LightmapSettings.lightmaps;
		LightmapData mapdata = new LightmapData();
		for (int i = 0; i < lightmaparray.Length; i++) 
		{
			mapdata.lightmapColor = LightMap;
			lightmaparray[i] = mapdata;
		}
		LightmapSettings.lightmaps = lightmaparray;		
		GameObject[] table = UnityEngine.Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i<table.Length; i++)
		{
			if (table[i].GetComponent<Renderer>() != null)
			{
				table[i].GetComponent<Renderer>().lightmapIndex = 0;
				table[i].GetComponent<Renderer>().lightmapScaleOffset = new Vector4(ScaleX,ScaleY,OffsetX,OffsetY);
			}
		}		
	}
	
	/*
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.I)) SetLightMap();
	}
	*/
}
