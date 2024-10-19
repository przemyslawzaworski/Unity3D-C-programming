using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

// https://en.wikipedia.org/wiki/List_of_CIL_instructions
public class DynamicCompilation : MonoBehaviour
{
	void CreateDynamicMethodA()
	{
		DynamicMethod dynamicMethod = new DynamicMethod("SortList", null, new Type[] {typeof(List<string>)}, typeof(DynamicCompilation).Module);
		ILGenerator il = dynamicMethod.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		MethodInfo methodInfo = typeof(List<string>).GetMethod("Sort", Type.EmptyTypes);
		il.Emit(OpCodes.Callvirt, methodInfo);
		il.Emit(OpCodes.Ret);
		Action<List<string>> action = (Action<List<string>>)dynamicMethod.CreateDelegate(typeof(Action<List<string>>));
		List<string> list = new List<string> {"banana", "apple", "date", "cherry"};
		Debug.LogError("Before sorting: " + string.Join(", ", list));
		action(list);
		Debug.LogError("After sorting: " + string.Join(", ", list));
	}

	void CreateDynamicMethodB()
	{
		AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
		AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
		TypeBuilder typeBuilder = moduleBuilder.DefineType("DynamicClass", TypeAttributes.Public);
		MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.Static;
		MethodBuilder methodBuilder = typeBuilder.DefineMethod("Add", attributes, typeof(int), new Type[] {typeof(int), typeof(int)});
		ILGenerator il = methodBuilder.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Add);
		il.Emit(OpCodes.Ret);
		Type type = typeBuilder.CreateType();
		MethodInfo methodInfo = type.GetMethod("Add");
		object result = methodInfo.Invoke(null, new object[] { 10, 20 });
		Debug.LogError("Result of dynamically generated Add method: " + result); // Should print: 30
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			CreateDynamicMethodA();
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			CreateDynamicMethodB();
		}
	}
}