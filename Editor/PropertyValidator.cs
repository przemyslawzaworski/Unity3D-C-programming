using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

// Detect missing references in components
public class PropertyValidator : EditorWindow
{
	[MenuItem("Assets/Property Validator")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(PropertyValidator));
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Validation" ) ) 
		{
			if (Selection.activeGameObject == null) return;
			BuildReport(Selection.activeGameObject);
		}
	}

	struct ValidatorInfo
	{
		public string HierarchyPath;
		public string ComponentName;
		public string PropertyName;
	};

	ValidatorInfo[] Validation(GameObject source)
	{
		List<ValidatorInfo> validatorInfos = new List<ValidatorInfo>();
		MonoBehaviour[] components = source.GetComponentsInChildren<MonoBehaviour>(true);
		for (int i = 0; i < components.Length; i++)
		{
			Component component = components[i];
			SerializedObject serializedObject = new SerializedObject(component);
			SerializedProperty serializedProperty = serializedObject.GetIterator();
			while (serializedProperty.NextVisible(true))
			{
				PropertyInfo info = serializedProperty.GetType().GetProperty("objectReferenceStringValue", BindingFlags.NonPublic | BindingFlags.Instance);
				string result = (string) info.GetValue(serializedProperty, null);
				if (result.Contains("None") || result.Contains("Missing"))
				{
					GameObject target = component.gameObject;
					string hierarchyPath = target.name;
					while (target.transform.parent != null) 
					{
						target = target.transform.parent.gameObject;
						hierarchyPath = target.name + "/" + hierarchyPath;
					}
					ValidatorInfo validatorInfo = new ValidatorInfo();
					validatorInfo.HierarchyPath = hierarchyPath;
					validatorInfo.ComponentName = component.GetType().ToString();
					validatorInfo.PropertyName = serializedProperty.name;
					validatorInfos.Add(validatorInfo);
				}
			}
		}
		return validatorInfos.ToArray();
	}

	void BuildReport(GameObject source)
	{
		ValidatorInfo[] validatorInfos = Validation(source);
		string filePath = Path.Combine(Path.GetTempPath(), "report.html");
		StreamWriter writer = new StreamWriter(filePath);
		string header = "<html><head><style>table{border-collapse: collapse;} td{border: 2px solid #000000;}</style></head><body>";
		string footer = "</body></html>";
		writer.WriteLine(header);
		writer.WriteLine("<table>");
		writer.WriteLine("<tr style=\"background-color:#FFCCCC\"><td>Hierarchy path</td><td>Component</td><td>Property</td></tr>");	
		for (int i = 0; i < validatorInfos.Length; i++)
		{
			writer.WriteLine("<tr>");
			writer.WriteLine("<td>" + validatorInfos[i].HierarchyPath + "</td>");
			writer.WriteLine("<td>" + validatorInfos[i].ComponentName + "</td>");
			writer.WriteLine("<td>" + validatorInfos[i].PropertyName + "</td>"); 
			writer.WriteLine("</tr>");
		}
		writer.WriteLine("</table>");
		writer.WriteLine(footer);
		writer.Close();
		System.Diagnostics.Process.Start(filePath);
	}
}