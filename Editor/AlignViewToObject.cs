using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

public class AlignViewToObject : EditorWindow
{
	[MenuItem("Assets/Align View To Object")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow (typeof(AlignViewToObject));
	}

	Bounds GenerateBounds (GameObject parent)
	{
		Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
		bool hasBounds = false;
		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			if (hasBounds) 
			{
				bounds.Encapsulate(renderers[i].bounds);
			} 
			else 
			{
				bounds = renderers[i].bounds;
				hasBounds = true;
			}
		}
		return bounds;
	}

	float GetPerspectiveCameraDistance (float objectSize, float fov)
	{
		return objectSize / Mathf.Sin(fov * 0.5f * Mathf.Deg2Rad);
	}

	void FocusOnGameView (Camera camera, GameObject target)
	{
		Bounds bounds = GenerateBounds(target);
		Vector3 dimensions = bounds.max - bounds.min;
		float size = Mathf.Max(dimensions.x, dimensions.y, dimensions.z);
		float distance = GetPerspectiveCameraDistance(size, camera.fieldOfView) + camera.nearClipPlane;	
		GameObject gameObject = new GameObject();
		Transform transform = gameObject.transform;	
		transform.rotation = target.transform.rotation;
		transform.position = bounds.center;
		camera.transform.position = transform.TransformPoint(distance * new Vector3(0f, 0f, -1f));
		camera.transform.rotation = transform.rotation;
		DestroyImmediate(gameObject);
	}

	void FocusOnSceneView (GameObject target)
	{
		Bounds bounds = GenerateBounds(target);
		Vector3 dimensions = bounds.max - bounds.min;
		float size = Mathf.Max(dimensions.x, dimensions.y, dimensions.z);
		GameObject gameObject = new GameObject();
		Transform transform = gameObject.transform;	
		transform.rotation = target.transform.rotation;
		transform.position = bounds.center;	
		transform.position = transform.TransformPoint(new Vector3(0f, 0f, -1f) * size);
		SceneView.lastActiveSceneView.AlignViewToObject(transform);
		DestroyImmediate(gameObject);
	}

	void OnGUI()
	{
		if (GUILayout.Button( "Align View To Object - GameView" ))
		{
			if (Selection.activeGameObject == null) return;
			FocusOnGameView (Camera.main, Selection.activeGameObject);
		}
		if (GUILayout.Button( "Align View To Object - SceneView" ))
		{
			if (Selection.activeGameObject == null) return;
			FocusOnSceneView (Selection.activeGameObject);
		}
	}
}