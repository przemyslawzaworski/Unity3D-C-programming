 using UnityEngine;
 using System.Collections;
 
 public class drag_rigidbody : MonoBehaviour 
 {
	private Vector3 screen_point;
	private Vector3 offset;

	void OnMouseDown()
	{
		screen_point = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		Vector3 input = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen_point.z);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(input);
	}
 
	void OnMouseDrag()
	{ 
		Vector3 cursor_point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen_point.z);
		Vector3 final_position = Camera.main.ScreenToWorldPoint(cursor_point)+offset;
		transform.position = final_position;
	}
}