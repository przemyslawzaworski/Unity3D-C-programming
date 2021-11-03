using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

public class GenerateScripts : EditorWindow
{
	[MenuItem("Assets/Generate Scripts")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow ( typeof(GenerateScripts) );
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Execute" ) ) Execute(Selection.activeGameObject);
	}

	string Substring (string src, string start, string end)
	{
		if (!src.Contains(start) || !src.Contains(end)) return null;
		int x = src.IndexOf(start) + start.Length;
		int y = src.LastIndexOf(end);
		return src.Substring(x, y - x);
	}

	string ExtractGenericListType (string src)
	{
		string[] dst = src.Split('`');
		return dst[0] + "<" + Substring(dst[1],"[","]") + ">";
	}

	void Execute (GameObject target)
	{
		MonoBehaviour[] components = target.GetComponentsInChildren<MonoBehaviour>(true);
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != null)
			{
				string name = components[i].GetType().ToString();
				string fullName = components[i].GetType().Namespace;
				if (name.Contains(".")) 
				{
					string[] split = name.Split('.');
					name = split[split.Length - 1];
				}
				if (System.String.IsNullOrEmpty(fullName) == false)
				{
					StreamWriter writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, name + ".cs"));
					writer.WriteLine("using UnityEngine;");
					writer.WriteLine("using System.Collections.Generic;");
					writer.WriteLine("");
					writer.WriteLine("namespace " + fullName);
					writer.WriteLine("{");
					writer.WriteLine("\tpublic class " + name + " : MonoBehaviour");
					writer.WriteLine("\t{");
					FieldInfo[] fieldInfos = components[i].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
					for (int j = 0; j < fieldInfos.Length; j++)
					{
						try
						{
							string fieldType = fieldInfos[j].FieldType.ToString();
							if (fieldType.Contains("`")) fieldType = ExtractGenericListType (fieldType);
							writer.WriteLine("\t\tpublic " + fieldType + " " + fieldInfos[j].Name.ToString() + ";");
						}
						catch (System.Exception e)
						{
							UnityEngine.Debug.LogError(e);
						}
					}
					writer.WriteLine("\t}");
					writer.WriteLine("}");
					writer.Close();					
				}
				else
				{
					StreamWriter writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, name + ".cs"));
					writer.WriteLine("using UnityEngine;");
					writer.WriteLine("using System.Collections.Generic;");
					writer.WriteLine("");
					writer.WriteLine("public class " + name + " : MonoBehaviour");
					writer.WriteLine("{");
					FieldInfo[] fieldInfos = components[i].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
					for (int j = 0; j < fieldInfos.Length; j++)
					{
						try
						{
							string fieldType = fieldInfos[j].FieldType.ToString();
							if (fieldType.Contains("`")) fieldType = ExtractGenericListType (fieldType);
							writer.WriteLine("\tpublic " + fieldType + " " + fieldInfos[j].Name.ToString() + ";");
						}
						catch (System.Exception e)
						{
							UnityEngine.Debug.LogError(e);
						}
					}
					writer.WriteLine("}");
					writer.Close();					
				}
			}
		}
	}
}