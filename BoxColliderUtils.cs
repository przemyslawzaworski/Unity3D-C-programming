using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxColliderUtils : MonoBehaviour
{
	public BoxCollider Box;

	Vector4 GetScreenCoordinates (BoxCollider boxCollider, Camera camera)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> screenCoords = new List<Vector2>();
		for (int i = 0; i < 8; i++)
		{
			Vector3 extents = boxCollider.size * 0.5f;
			extents.Scale(new Vector3((i & 1) == 0 ? 1 : -1, (i & 2) == 0 ? 1 : -1, (i & 4) == 0 ? 1 : -1));
			Vector3 localPos = boxCollider.center + extents;
			vertices.Add(boxCollider.transform.TransformPoint(localPos));
		}
		Vector3[] array = vertices.OrderBy(v => v.x).ThenBy(v => v.y).ToArray();
		for (int i = 0; i < array.Length; i = i + 2)
		{
			Vector3 result = camera.WorldToScreenPoint(array[i]);
			screenCoords.Add(new Vector2(result.x, result.y));
		}
		return new Vector4(screenCoords.Min(v => v.x), screenCoords.Min(v => v.y), screenCoords.Max(v => v.x), screenCoords.Max(v => v.y));	
	}

	bool IsInsideRectangle (float x1, float y1, float x2, float y2, float px, float py)
	{
		return px >= x1 && px <= x2 && py >= y1 && py <= y2; 
	}	

	void Update()
	{
		Vector4 rect = GetScreenCoordinates (Box, Camera.main); 
		if (IsInsideRectangle(rect.x, rect.y, rect.z, rect.w, Input.mousePosition.x, Input.mousePosition.y))
		{
			Debug.Log("Selected box collider");
		}
	}
}