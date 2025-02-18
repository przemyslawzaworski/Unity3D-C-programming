using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Elevator : MonoBehaviour
{
	public Rigidbody ElevatorRB;
	public float Speed = 1f;
	public bool Up = true;
	public bool Enable {get; set;} = false;

	void Start()
	{
		if (ElevatorRB == null)
		{
			ElevatorRB = GetComponent<Rigidbody>();
		}
		ElevatorRB.isKinematic = true;
	}

	void FixedUpdate()
	{
		if (Enable)
		{
			float sign = Up ? 1f : - 1f;
			ElevatorRB.velocity = new Vector3(0f, Speed * sign, 0f);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Enable = !Enable;
			ElevatorRB.isKinematic = !ElevatorRB.isKinematic;
		}
	}
}