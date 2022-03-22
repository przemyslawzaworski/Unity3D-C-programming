using UnityEngine;
using System.IO;

public class ShowOnClickEventsFromButtons : MonoBehaviour
{
	public GameObject Target;

	void Start()
	{
		Execute(Target);
	}

	string GetHierarchyPath (GameObject source) 
	{
		string hierarchyPath = source.name;
		while (source.transform.parent != null)
		{
			source = source.transform.parent.gameObject;
			hierarchyPath = source.name + "/" + hierarchyPath;
		}
		return hierarchyPath;
	}

	void Execute(GameObject source)
	{
		string filePath = Path.Combine(Path.GetTempPath(), "report.html");
		StreamWriter writer = new StreamWriter(filePath);
		string header = "<html><head><style>table{border-collapse: collapse; max-width: 100%;}td{border: 2px solid #000000;}</style></head><body>";
		string footer = "</body></html>";
		writer.WriteLine(header);
		writer.WriteLine("<table>");
		UnityEngine.UI.Button[] buttons = source.GetComponentsInChildren<UnityEngine.UI.Button>(true);
		for (int i = 0; i < buttons.Length; i++)
		{
			UnityEngine.UI.Button.ButtonClickedEvent onClick = buttons[i].onClick;
			int count = onClick.GetPersistentEventCount();
			for (int j = 0; j < count; j++)
			{
				writer.WriteLine("<tr>");
				writer.WriteLine("<td>" + GetHierarchyPath(buttons[i].gameObject) + "</td>");
				writer.WriteLine("<td>" + onClick.GetPersistentTarget(j).GetType().ToString() + "." + onClick.GetPersistentMethodName(j) + "</td>"); 
				writer.WriteLine("</tr>");
			}
		}
		writer.WriteLine("</table><br>");
		writer.WriteLine(footer);
		writer.Close();
		System.Diagnostics.Process.Start(filePath);
	}
}