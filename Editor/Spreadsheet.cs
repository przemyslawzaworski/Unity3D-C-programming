using UnityEngine;
using UnityEditor;

public class Spreadsheet : EditorWindow
{
    private static string[] table;
    private static GUISkin customGUISkin;
    private static GUIStyle cellStyle;
    private static int selectedAlignment = 1; // Default to center alignment
    private static readonly string[] alignmentOptions = { "Left", "Center", "Right" };
    private static int selectedColor = 0; // Default to black color
    private static readonly string[] colorOptions = { "Black", "Red", "Green", "Blue" };
    private static readonly Color[] colors = { Color.black, Color.red, Color.green, Color.blue };
    private static float cellWidth = 100; // Default cell width

    [MenuItem("Assets/Spreadsheet")]
    static void ShowWindow()
    {
        table = new string[1024];
        for (int i = 0; i < table.Length; i++) table[i] = "";
        EditorWindow.GetWindow(typeof(Spreadsheet));
    }

    void OnGUI()
    {
        if (customGUISkin == null)
        {
            CreateCustomGUISkin();
        }

        // Add the alignment dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Text Alignment:", GUILayout.Width(100));
        selectedAlignment = EditorGUILayout.Popup(selectedAlignment, alignmentOptions, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Add the color dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Color:", GUILayout.Width(100));
        selectedColor = EditorGUILayout.Popup(selectedColor, colorOptions, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Add the cell width slider
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Cell Width:", GUILayout.Width(100));
        cellWidth = EditorGUILayout.Slider(cellWidth, 50, 200, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        // Update cellStyle based on selected alignment and color
        if (cellStyle == null)
        {
            cellStyle = new GUIStyle(customGUISkin.textField);
            cellStyle.normal.background = MakeTex(2, 2, Color.white);
        }

        cellStyle.normal.textColor = colors[selectedColor];

        switch (selectedAlignment)
        {
            case 0:
                cellStyle.alignment = TextAnchor.MiddleLeft;
                break;
            case 1:
                cellStyle.alignment = TextAnchor.MiddleCenter;
                break;
            case 2:
                cellStyle.alignment = TextAnchor.MiddleRight;
                break;
        }

        int numRows = 32;
        int numCols = 32;
        int cellHeight = 20;

        for (int row = 0; row < numRows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < numCols; col++)
            {
                int index = row * numCols + col;
                table[index] = GUILayout.TextField(table[index], cellStyle, GUILayout.Width(cellWidth), GUILayout.Height(cellHeight));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void CreateCustomGUISkin()
    {
        customGUISkin = ScriptableObject.CreateInstance<GUISkin>();
        customGUISkin.textField = new GUIStyle(GUI.skin.textField);
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
