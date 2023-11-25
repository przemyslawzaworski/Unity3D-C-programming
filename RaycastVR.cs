using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVR : MonoBehaviour
{
	public Transform Bullet; // sphere gameobject with rigidbody
	public Transform Cylinder; // scaled cylinder gameobject with no mesh collider
	public Transform Controller; // right controller mesh with no mesh collider
	public Transform TrackingSpace;  // OVRPlayerController/OVRCameraRig/TrackingSpace

	void Update()
	{
		Vector3 position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
		Quaternion rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
		Matrix4x4 matrix = TrackingSpace.localToWorldMatrix;
		Vector3 origin = matrix.MultiplyPoint(position);
		Vector3 direction = matrix.MultiplyVector(rotation * Vector3.forward);
		Controller.position = origin;
		Controller.rotation = rotation;
		Cylinder.transform.position = new Vector3(1e5f, 1e5f, 1e5f);
		if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
		{
			Ray ray = new Ray(origin, direction);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Vector3 scale = Cylinder.transform.localScale;
				Cylinder.transform.position = origin + (hit.point - origin) * 0.5f;
				Cylinder.transform.rotation = Quaternion.LookRotation(hit.point - ray.origin) * Quaternion.Euler(90, 0, 0);
				Cylinder.transform.localScale = new Vector3(scale.x, hit.distance * 0.5f, scale.z);
				if (OVRInput.GetDown(OVRInput.Button.One))
				{
					GameObject instance = Instantiate(Bullet.gameObject);
					instance.transform.position = ray.origin;
					Rigidbody rigidbody = instance.GetComponent<Rigidbody>();
					rigidbody.isKinematic = false;
					rigidbody.AddForce(ray.direction * 5000f);
				}
			}
		}
	}
}