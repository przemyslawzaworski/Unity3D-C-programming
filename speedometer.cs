/*
Example:
Speedometer has values from 0 to 120 km/h for full circumference (0-360 degrees), so multiply velocity(speed) by 3.0
Input float Speed has unit in m/s, so convert to km/h by multiplication by 3.6
Input child GameObject (for example speed pointer), should have pivot situated at the end (fit to center of parent circle).
In this way, gameobject can be rotated around, like hand of a clock.
*/

using UnityEngine;

public class speedometer : MonoBehaviour
{
	public GameObject Pointer;
	public float Speed;
	
	private Vector3 _StartEuler;
	
	void Start()
	{
		_StartEuler = Pointer.transform.localEulerAngles;
	}

	void ComputeAngle (float speed, GameObject obj)
	{
		float angle = speed * 3.0f;
		obj.transform.localRotation = Quaternion.Euler(_StartEuler) * Quaternion.Euler(0.0f, angle, 0.0f);
	}
	
	void Update()
	{
		ComputeAngle (Speed * 3.6f, Pointer);
	}
}
