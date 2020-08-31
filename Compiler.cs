// Example of C# dynamic code compilation
// Player Settings -> API Compatibility Level -> .Net 4.x
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine; 
using System.Linq;

public class Compiler : MonoBehaviour
{
	string _CreateSphereSourceCode = @"
		using UnityEngine;
		public class CreateSphere : MonoBehaviour
		{
			public static CreateSphere AddComponent(GameObject host)
			{
				return host.AddComponent<CreateSphere>();
			}
			void Start()
			{
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = Vector3.zero;
			}
		}
	";

	string _ShowComponentsSourceCode = @"
		using UnityEngine;
		using System.Linq;
		public class ShowComponents : MonoBehaviour
		{
			public static ShowComponents AddComponent(GameObject host)
			{
				return host.AddComponent<ShowComponents>();
			}
			void Start()
			{
				MonoBehaviour[] scripts = this.transform.gameObject.GetComponentsInChildren<MonoBehaviour>();
				string[] types = new string[scripts.Length];
				for (int i = 0; i < scripts.Length; i++) types[i] = scripts[i].GetType().ToString();
				var table = types.Distinct();
				for (int i = 0; i < table.Count(); i++) Debug.Log(table.ElementAt(i));
			}
		}
	";

	void BuildAndRun(string className, string source)
	{
		CSharpCodeProvider provider = new CSharpCodeProvider();
		CompilerParameters options = new CompilerParameters();
		options.GenerateExecutable = false;
		options.GenerateInMemory = true;
		options.ReferencedAssemblies.Add("C:\\Program Files\\Unity\\Editor\\Data\\Managed\\UnityEngine.dll");
		options.ReferencedAssemblies.Add("C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.1\\System.Core.dll");
		CompilerResults result = provider.CompileAssemblyFromSource(options, source);
		if (result.Errors.Count > 0) 
		{
			StringBuilder msg = new StringBuilder();
			foreach (CompilerError error in result.Errors)
			{
				msg.AppendFormat("Error ({0}): {1}\n", error.ErrorNumber, error.ErrorText);
			}
			throw new Exception(msg.ToString());
		}
		Type type = result.CompiledAssembly.GetType(className);
		MethodInfo method = type.GetMethod("AddComponent");
		var function = (Func<GameObject, MonoBehaviour>)Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), method);
		MonoBehaviour component = function.Invoke(gameObject);	
	}

	void Start() 
	{
		BuildAndRun("CreateSphere", _CreateSphereSourceCode);
		BuildAndRun("ShowComponents", _ShowComponentsSourceCode);
	}
}