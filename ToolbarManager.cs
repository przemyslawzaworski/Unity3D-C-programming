using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

[InitializeOnLoad]
public static class ToolbarManager
{
	public static readonly List<Action> Actions = new List<Action>();
	private static GUIStyle _GUIStyle = null;

	static ToolbarManager()
	{
		ToolbarCallback.OnToolbarGUI = OnGUI;
		ToolbarCallback.OnToolbarGUIButton = GUIButton;
	}

#if UNITY_2019_3_OR_NEWER
	public const float Space = 8;
#else
	public const float Space = 10;
#endif
	public const float LargeSpace = 20;
	public const float ButtonWidth = 32;
	public const float DropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
	public const float PlayPauseStopWidth = 140;
#else
	public const float PlayPauseStopWidth = 100;
#endif

	static void OnGUI()
	{
		if (_GUIStyle == null) _GUIStyle = new GUIStyle("CommandLeft");
		float screenWidth = EditorGUIUtility.currentViewWidth;
		float playButtonsPosition = Mathf.RoundToInt ((screenWidth - PlayPauseStopWidth) / 2);
		Rect rect = new Rect(0, 0, screenWidth, Screen.height);
		rect.xMin = playButtonsPosition;
		rect.xMin += _GUIStyle.fixedWidth * 3;
		rect.xMax = screenWidth;
		rect.xMax -= Space;
		rect.xMax -= DropdownWidth;
		rect.xMax -= Space;
		rect.xMax -= DropdownWidth;
#if UNITY_2019_3_OR_NEWER
		rect.xMax -= Space;
#else
		rect.xMax -= LargeSpace;
#endif
		rect.xMax -= DropdownWidth;
		rect.xMax -= Space;
		rect.xMax -= ButtonWidth;
		rect.xMax -= Space;
		rect.xMax -= 78;
		rect.xMin += Space;
		rect.xMax -= Space;
#if UNITY_2019_3_OR_NEWER
		rect.y = 4;
		rect.height = 22;
#else
		rect.y = 5;
		rect.height = 24;
#endif
		if (rect.width > 0)
		{
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal();
			foreach (Action handler in Actions)
			{
				handler();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}

	public static void GUIButton() 
	{
		GUILayout.BeginHorizontal();
		foreach (Action handler in Actions)
		{
			handler();
		}
		GUILayout.EndHorizontal();
	}
}

public static class ToolbarCallback
{
	static BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#if UNITY_2020_1_OR_NEWER
	static PropertyInfo _WindowBackend = typeof(Editor).Assembly.GetType("UnityEditor.GUIView").GetProperty("windowBackend", _BindingFlags);
	static PropertyInfo _VisualTree = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend").GetProperty("visualTree", _BindingFlags);
#else
	static PropertyInfo _VisualTree = typeof(Editor).Assembly.GetType("UnityEditor.GUIView").GetProperty("visualTree", _BindingFlags);
#endif
	static FieldInfo _OnGUIHandler = typeof(IMGUIContainer).GetField("m_OnGUIHandler", _BindingFlags);
	static ScriptableObject _ScriptableObject;

	public static Action OnToolbarGUI;
	public static Action OnToolbarGUIButton;

	static ToolbarCallback()
	{
		EditorApplication.update -= OnUpdate;
		EditorApplication.update += OnUpdate;
	}

	static void OnUpdate()
	{
		if (_ScriptableObject == null)
		{
			UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(typeof(Editor).Assembly.GetType("UnityEditor.Toolbar"));
			_ScriptableObject = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
			if (_ScriptableObject != null)
			{
#if UNITY_2021_1_OR_NEWER
				FieldInfo root = _ScriptableObject.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
				VisualElement mRoot = root.GetValue(_ScriptableObject) as VisualElement;
				VisualElement toolbarZone = mRoot.Q("ToolbarZoneRightAlign");
				VisualElement parent = new VisualElement() {style = {flexGrow = 1, flexDirection = FlexDirection.Row,}};
				IMGUIContainer container = new IMGUIContainer();
				container.style.flexGrow = 1;
				container.onGUIHandler += () => { OnToolbarGUIButton?.Invoke();}; 
				parent.Add(container);
				toolbarZone.Add(parent);
				
#else
#if UNITY_2020_1_OR_NEWER
				System.Object windowBackend = _WindowBackend.GetValue(_ScriptableObject);
				VisualElement visualTree = (VisualElement) _VisualTree.GetValue(windowBackend, null);
#else
				VisualElement visualTree = (VisualElement) _VisualTree.GetValue(_ScriptableObject, null);
#endif
				IMGUIContainer container = (IMGUIContainer) visualTree[0];
				Action handler = (Action) _OnGUIHandler.GetValue(container);
				handler -= OnGUI;
				handler += OnGUI;
				_OnGUIHandler.SetValue(container, handler);
#endif
			}
		}
	}

	static void OnGUI() 
	{
		Action handler = OnToolbarGUI;
		if (handler != null) handler();
	}
}

[InitializeOnLoad]
public static class SimulateBuild
{
	static bool Enabled = false;

	static SimulateBuild()
	{
		ToolbarManager.Actions.Add(OnToolbarGUI);
	}

	static void OnToolbarGUI()
	{
		Texture texture = EditorGUIUtility.IconContent("Prefab Icon").image;
		GUI.changed = false;
		GUILayout.Toggle(Enabled, new GUIContent(null, texture, "Simulate Build"), "Command");
		if (GUI.changed)
		{
			Enabled = !Enabled;
			Debug.Log("Simulate build is: " + Enabled.ToString());
		}
	}
}