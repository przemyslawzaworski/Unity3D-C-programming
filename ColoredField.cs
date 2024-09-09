using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ColoredField : MonoBehaviour
{
    [ColorField(1f, 0f, 0f)] public int Red = 0;
    [field: SerializeField, ColorField(0f, 1f, 0f)] public float Green { get; set; } = 0.0f;
    [ColorField("#0000FF")]  public string Blue = "";
    public GameObject Default;
    [ColorField(1f, 1f, 0f)]  public Transform Yellow;
}

public class ColorFieldAttribute : PropertyAttribute
{
    public Color color;

    public ColorFieldAttribute(float r, float g, float b, float a = 1f)
    {
        color = new Color(r, g, b, a);
    }

    public ColorFieldAttribute(string hexColor)
    {
        bool success = ColorUtility.TryParseHtmlString(hexColor, out Color parsedColor);
        color = success ? parsedColor : Color.white;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ColorFieldAttribute))]
public class ColorFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ColorFieldAttribute colorField = (ColorFieldAttribute)attribute;
        Color previousColor = GUI.color;
        GUI.color = colorField.color;
        EditorGUI.PropertyField(position, property, label);
        GUI.color = previousColor;
    }
}
#endif