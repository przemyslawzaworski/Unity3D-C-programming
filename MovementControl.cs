using UnityEngine;

public class MovementControl : MonoBehaviour
{
	Vector3 direction = Vector3.zero;
	Vector3 previous = Vector3.zero;
	Vector3 current = Vector3.zero;
	Vector3 forward = Vector3.zero;
 
	void Update()
	{
		current = transform.position;
		direction = (current - previous);
		if(Vector3.Dot(forward, direction) < 0)
			Debug.Log("Moving backwards");
		else if(Vector3.Dot(forward, direction) > 0)
			Debug.Log("Moving forwards"); 
		else Debug.Log("Static");
	}

	void LateUpdate()
	{
		previous = transform.position;
		forward = transform.forward;
	}
}
