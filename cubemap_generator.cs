//In project window create new cubemap from command "Create\Legacy\Cubemap" and set it to "Readable".
//Run script. Then create material with shader "Skybox/Cubemap" and set cubemap.
//Set new skybox: Window/Lighting/Settings ->Environment/Skybox Material
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class cubemap_generator : ScriptableWizard
{
	public Transform origin;
	public Cubemap cubemap;

	void OnWizardCreate()
	{
		GameObject game_object = new GameObject("Camera");
		game_object.AddComponent<Camera>();
		game_object.transform.position = origin.position;
		game_object.transform.rotation = Quaternion.identity;      
		game_object.GetComponent<Camera>().RenderToCubemap(cubemap);
		DestroyImmediate(game_object);
		SaveCubemap();
	}

	[MenuItem("GameObject/Render to cubemap")]
	static void RenderCubemap()
	{
		ScriptableWizard.DisplayWizard<cubemap_generator>("Render to cubemap", "Generate");
	}

	void SaveCubemap()
	{
		Texture2D image = new Texture2D (cubemap.width, cubemap.height, TextureFormat.RGB24, false);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.PositiveX));        
		byte[] bytes = image.EncodeToPNG();      
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name +"_PositiveX.png", bytes);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.NegativeX));
		bytes = image.EncodeToPNG();     
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name +"_NegativeX.png", bytes);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.PositiveY));
		bytes = image.EncodeToPNG();     
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name +"_PositiveY.png", bytes);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.NegativeY));
		bytes = image.EncodeToPNG();     
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name +"_NegativeY.png", bytes);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.PositiveZ));
		bytes = image.EncodeToPNG();     
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name +"_PositiveZ.png", bytes);       
		image.SetPixels(cubemap.GetPixels(CubemapFace.NegativeZ));
		bytes = image.EncodeToPNG();     
		File.WriteAllBytes(Application.dataPath + "/" + cubemap.name   +"_NegativeZ.png", bytes);       
		DestroyImmediate(image);
	}	
}