//Assign spheres.
//It needs improvements.

using UnityEngine;

public class VerletExtended : MonoBehaviour 
{
	public GameObject[] Sphere;
	public GameObject Box;
	public float Bounce = 0.95f;
	public float Gravity = -0.01f;
	public float Friction = 0.998f;	
	Vector3 BoundsMin;
	Vector3 BoundsMax;
	Collider BoxCollider;
	float Radius;
	Vector3[] CurrentPosition;
	Vector3[] PreviousPosition;
	Vector3[] Velocity;
		
	void Start () 
	{
		CurrentPosition = new Vector3[Sphere.Length];
		PreviousPosition = new Vector3[Sphere.Length];
		Velocity = new Vector3[Sphere.Length];
		BoxCollider = Box.GetComponent<Collider>();
		Radius = Sphere[0].transform.localScale.x * 0.5f;
		for (int i=0;i<Sphere.Length;i++)
		{
			CurrentPosition[i] = Sphere[i].transform.position;
			PreviousPosition[i] = Sphere[i].transform.position - new Vector3(Random.Range(0.1f,0.9f),1.0f,Random.Range(0.1f,0.9f));
		}
	}
	
	bool NextCombination (int[] comb, int n)
	{
		int i = 1;
		comb [i]++;
		while ((i > 0) && (comb [i] >= n - 2 + 1 + i)) 
		{
			i--;
			comb [i]++;
		}
		if (comb [0] > n - 2) return  false;
		for (i++; i < 2; i++) 
		{
			comb [i] = comb [i - 1] + 1;
		}
		return  true;
	}
	
	void Solver ()
	{
		int[] comb = new int[2];
		float d1 = Vector3.Distance( Sphere[0].transform.position, Sphere[1].transform.position) - 2.0f * Radius;
		if (d1<0.0f)
		{
			CurrentPosition[0] = PreviousPosition[0];
			PreviousPosition[0] = CurrentPosition[0] + Velocity[0] * Bounce  ;
			CurrentPosition[1] = PreviousPosition[1];
			PreviousPosition[1] = CurrentPosition[1] + Velocity[1] * Bounce ;			
		}

		while (NextCombination (comb, Sphere.Length)) 
		{
			float d2 = Vector3.Distance( Sphere[comb[0]].transform.position, Sphere[comb[1]].transform.position) - 2.0f * Radius;
			if (d2<0.0f)
			{
				CurrentPosition[comb[0]] = PreviousPosition[comb[0]];
				PreviousPosition[comb[0]] = CurrentPosition[comb[0]] + Velocity[comb[0]] * Bounce ;					
				CurrentPosition[comb[1]] = PreviousPosition[comb[1]];
				PreviousPosition[comb[1]] = CurrentPosition[comb[1]] + Velocity[comb[1]] * Bounce ;					
			}			
		}
		
		for (int i=0;i<Sphere.Length;i++)
		{
			Velocity[i].x = (CurrentPosition[i].x - PreviousPosition[i].x) * Friction;
			Velocity[i].y = (CurrentPosition[i].y - PreviousPosition[i].y) * Friction;
			Velocity[i].z = (CurrentPosition[i].z - PreviousPosition[i].z) * Friction;
			PreviousPosition[i].x = CurrentPosition[i].x;
			PreviousPosition[i].y = CurrentPosition[i].y;
			PreviousPosition[i].z = CurrentPosition[i].z;
			CurrentPosition[i].x += Velocity[i].x;
			CurrentPosition[i].z += Velocity[i].z;
			CurrentPosition[i].y += Velocity[i].y;
			CurrentPosition[i].y += Gravity;
			if(CurrentPosition[i].x > BoundsMax.x-Radius) 
			{
				CurrentPosition[i].x = BoundsMax.x-Radius;
				PreviousPosition[i].x = CurrentPosition[i].x + Velocity[i].x * Bounce;
			}
			if(CurrentPosition[i].x < BoundsMin.x+Radius) 
			{
				CurrentPosition[i].x = BoundsMin.x+Radius;
				PreviousPosition[i].x = CurrentPosition[i].x + Velocity[i].x * Bounce;
			}
			if(CurrentPosition[i].y > BoundsMax.y-Radius) 
			{
				CurrentPosition[i].y = BoundsMax.y-Radius;
				PreviousPosition[i].y = CurrentPosition[i].y + Velocity[i].y * Bounce;
			}
			if(CurrentPosition[i].y < BoundsMin.y+Radius) 
			{
				CurrentPosition[i].y = BoundsMin.y+Radius;
				PreviousPosition[i].y = CurrentPosition[i].y + Velocity[i].y * Bounce;
			}
			if(CurrentPosition[i].z > BoundsMax.z-Radius) 
			{
				CurrentPosition[i].z = BoundsMax.z-Radius;
				PreviousPosition[i].z = CurrentPosition[i].z + Velocity[i].z * Bounce;
			}
			if(CurrentPosition[i].z < BoundsMin.z+Radius) 
			{
				CurrentPosition[i].z = BoundsMin.z+Radius;
				PreviousPosition[i].z = CurrentPosition[i].z + Velocity[i].z * Bounce;
			}
			Sphere[i].transform.position = CurrentPosition[i];
		}
	}
	
	void Update () 
	{
		BoundsMin = BoxCollider.bounds.min;
		BoundsMax = BoxCollider.bounds.max;
		if (Input.GetKey(KeyCode.I))
		{
			CurrentPosition[0].x = Sphere[0].transform.position.x;
			PreviousPosition[0].x = Sphere[0].transform.position.x - 0.3f;
		}
		if (Input.GetKey(KeyCode.O))
		{
			CurrentPosition[0].y = Sphere[0].transform.position.y;
			PreviousPosition[0].y = Sphere[0].transform.position.y - 0.3f;
		}
		if (Input.GetKey(KeyCode.P))
		{
			CurrentPosition[0].z = Sphere[0].transform.position.z;
			PreviousPosition[0].z = Sphere[0].transform.position.z - 0.3f;
		}
		Solver();
	}
}
