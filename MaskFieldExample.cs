using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MaskFieldExample : MonoBehaviour
{
	[System.Flags] public enum Tree { None, Birch, Maple, Oak, Pine }
	[MaskField] public Tree Trees;

	public void Start()
	{
		List<string> values = MaskFieldAttribute.GetValues(Trees);
		for (int i = 0; i < values.Count; i++) Debug.Log(values[i]);
	}
}

public sealed class MaskFieldAttribute : PropertyAttribute
{
	public static List<string> GetValues<T>(T param) where T : IConvertible
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
		{
			if ((Convert.ToInt32(param) & (1 << i)) != 0)
			{
				list.Add(Enum.GetValues(typeof(T)).GetValue(i).ToString());
			}
		}
		return list;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MaskFieldAttribute))]
public class MaskFieldAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
	}
}
#endif