using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class GetTexturePathsFromMaterials : EditorWindow
{
	[MenuItem("Assets/GetTexturePathsFromMaterials")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(GetTexturePathsFromMaterials));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Print Unique Texture Names" ) ) Print(true);
		if ( GUILayout.Button( "Print Textures and Materials" ) ) Print(false);
	} 

	void Print(bool unique)
	{
		string result = "";
		List<string> rows = new List<string>();
		foreach (UnityEngine.Object source in Selection.objects)
		{
			Material material = source as Material;
			if (material)
			{
				Shader shader = material.shader;
				for(int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++) 
				{
					if(ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) 
					{
						Texture texture = material.GetTexture(ShaderUtil.GetPropertyName(shader, i));
						if (texture)
						{
							if (unique) 
								rows.Add(texture.name);
							else
								rows.Add(texture.name + " (" + material.name + ")");
						}
					}
				}
			}
		}
		List<string> distinct = rows.Distinct().ToList();
		distinct.Sort();
		for(int i = 0; i < distinct.Count; i++) result = result + distinct[i] + "\n";
		string path = System.IO.Path.GetTempPath() + "\\GetTexturePathsFromMaterials.txt";
		System.IO.File.WriteAllText(path, result, System.Text.Encoding.UTF8);
		System.Diagnostics.Process.Start("notepad.exe", path);
	}
}