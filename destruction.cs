using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destruction : MonoBehaviour 
{
	public GameObject construction;
	public Mesh mesh;
	public int scale;
	public float resistance;
	public float TotalMass;
	
	List<GameObject> children;
	bool hit = false;

	List<GameObject> GetChildren(GameObject item)
	{
		List<GameObject> children = new List<GameObject>();
		foreach (Transform t in item.transform)
		{
			children.Add(t.gameObject);
		}
		return children;
	}
	
	void Start () 
	{
		construction.AddComponent<Rigidbody>().mass = TotalMass;
		construction.GetComponent<Rigidbody>().hideFlags = HideFlags.HideInInspector;
		int r = scale;
		GameObject[] cube = new GameObject[r*r*r];
		for (int i=0;i<r*r*r;i++)
		{
			cube[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube[i].transform.parent = construction.transform;
			cube[i].transform.localPosition = new Vector3(i%r,(i%(r*r))/r,i/(r*r));
			cube[i].transform.rotation = construction.transform.rotation;
			cube[i].hideFlags = HideFlags.HideInHierarchy;
		}
		children = GetChildren(construction);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > resistance && !hit)
		{
			for (int i=0;i<children.Count;i++)
			{
				Vector3 position = transform.TransformPoint(children[i].transform.localPosition);
				children[i].transform.parent = null;
				Rigidbody body = children[i].AddComponent<Rigidbody>();
				body.mass = construction.GetComponent<Rigidbody>().mass/(scale*scale*scale);
				children[i].transform.position = position;
				hit = true;
			}
		}
	}
}
