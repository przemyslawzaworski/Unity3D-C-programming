//Assign script to Sphere and disable Sphere Collider.

using UnityEngine;

public class Verlet : MonoBehaviour 
{
	public GameObject Box;
	public float Bounce = 0.95f;
	public float Gravity = -0.01f;
	public float Friction = 0.998f;	
	Vector3 BoundsMin;
	Vector3 BoundsMax;
	Collider BoxCollider;
	float Radius;
	Vector3 CurrentPosition;
	Vector3 PreviousPosition;
	Vector3 Velocity;
		
	void Start () 
	{
		BoxCollider = Box.GetComponent<Collider>();
		Radius = transform.localScale.x * 0.5f;
		CurrentPosition = transform.position;
		PreviousPosition = transform.position - new Vector3(4.0f,3.0f,3.0f);
	}
	
	void Solver ()
	{
		Velocity.x = (CurrentPosition.x - PreviousPosition.x) * Friction;
		Velocity.y = (CurrentPosition.y - PreviousPosition.y) * Friction;
		Velocity.z = (CurrentPosition.z - PreviousPosition.z) * Friction;
		PreviousPosition.x = CurrentPosition.x;
		PreviousPosition.y = CurrentPosition.y;
		PreviousPosition.z = CurrentPosition.z;
		CurrentPosition.x += Velocity.x;
		CurrentPosition.z += Velocity.z;
		CurrentPosition.y += Velocity.y;
		CurrentPosition.y += Gravity;
		if(CurrentPosition.x > BoundsMax.x-Radius) 
		{
			CurrentPosition.x = BoundsMax.x-Radius;
			PreviousPosition.x = CurrentPosition.x + Velocity.x * Bounce;
		}
		if(CurrentPosition.x < BoundsMin.x+Radius) 
		{
			CurrentPosition.x = BoundsMin.x+Radius;
			PreviousPosition.x = CurrentPosition.x + Velocity.x * Bounce;
		}
		if(CurrentPosition.y > BoundsMax.y-Radius) 
		{
			CurrentPosition.y = BoundsMax.y-Radius;
			PreviousPosition.y = CurrentPosition.y + Velocity.y * Bounce;
		}
		if(CurrentPosition.y < BoundsMin.y+Radius) 
		{
			CurrentPosition.y = BoundsMin.y+Radius;
			PreviousPosition.y = CurrentPosition.y + Velocity.y * Bounce;
		}
		if(CurrentPosition.z > BoundsMax.z-Radius) 
		{
			CurrentPosition.z = BoundsMax.z-Radius;
			PreviousPosition.z = CurrentPosition.z + Velocity.z * Bounce;
		}
		if(CurrentPosition.z < BoundsMin.z+Radius) 
		{
			CurrentPosition.z = BoundsMin.z+Radius;
			PreviousPosition.z = CurrentPosition.z + Velocity.z * Bounce;
		}
		transform.position = CurrentPosition;
	}
	
	void Update () 
	{
		BoundsMin = BoxCollider.bounds.min;
		BoundsMax = BoxCollider.bounds.max;
		if (Input.GetKey(KeyCode.I))
		{
			CurrentPosition.x = transform.position.x;
			PreviousPosition.x = transform.position.x - 0.3f;
		}
		if (Input.GetKey(KeyCode.O))
		{
			CurrentPosition.y = transform.position.y;
			PreviousPosition.y = transform.position.y - 0.3f;
		}
		if (Input.GetKey(KeyCode.P))
		{
			CurrentPosition.z = transform.position.z;
			PreviousPosition.z = transform.position.z - 0.3f;
		}
		Solver();
	}
}
