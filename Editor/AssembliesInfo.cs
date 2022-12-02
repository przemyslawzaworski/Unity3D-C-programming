using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Compilation;

public class AssembliesInfo : EditorWindow
{
	[MenuItem("Assets/Assemblies Info")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(AssembliesInfo));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Show" ) ) ShowAssembliesInfo();
	}

	void ShowAssembliesInfo()
	{
		string filePath = Path.Combine(Path.GetTempPath(), "report.html");
		StreamWriter writer = new StreamWriter(filePath);
		string header = "<html><head><style>table{border-collapse: collapse;} td{border: 2px solid #000000;text-align: center;}</style></head><body>";
		string footer = "</body></html>";
		writer.WriteLine(header);
		writer.WriteLine("<table>");
		string[] scripts = AssetDatabase.FindAssets("t:script");
		List<string> assemblies = new List<string>();
		for (int i = 0; i < scripts.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(scripts[i]);
			string assembly = CompilationPipeline.GetAssemblyNameFromScriptPath(path);
			string extension = Path.GetExtension(path);
			if (extension == ".dll" || assembly == null)
			{
				assembly = Path.GetFileName(path);
			}
			writer.WriteLine("<tr>");
			writer.WriteLine("<td>" + (i + 1).ToString() + "</td>");
			writer.WriteLine("<td>" + path + "</td>");
			writer.WriteLine("<td>" + assembly + "</td>");
			writer.WriteLine("</tr>");
			assemblies.Add(assembly);
		}
		writer.WriteLine("</table>");
		writer.WriteLine(footer);
		writer.Close();
		List<string> distinct = assemblies.Distinct().ToList();
		distinct.Sort();
		for (int i = 0; i < distinct.Count; i++)
		{
			Debug.Log(distinct[i]);
		}
		System.Diagnostics.Process.Start(filePath);
	}
}