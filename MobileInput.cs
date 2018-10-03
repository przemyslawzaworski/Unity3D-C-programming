using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour 
{
	public List<GameObject> list = new List<GameObject>();
	public Camera MainCamera;
	private Ray ray;
	private RaycastHit hit;
	private float factor = 2.0f;
	
	void Update () 
	{
		for (int i=0;i<list.Count;i++)
		{
			if(Input.touches.Length==1)
			{
				Touch touch = Input.touches[0];
				switch(touch.phase)
				{
					case TouchPhase.Began: 
						ray = MainCamera.ScreenPointToRay(touch.position);
						if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
						{
							list[i] = hit.collider.gameObject;
						}
						break;
					case TouchPhase.Moved:
						if(list[i])
						{
							float A = MainCamera.transform.forward.x;
							float B = MainCamera.transform.forward.z;
							float C = MainCamera.transform.right.x;
							float D = MainCamera.transform.right.z;
							if (touch.deltaPosition.y > 0.0f)
							{
								Vector3 P = new Vector3(A*Time.deltaTime*factor, 0.0f, B*Time.deltaTime*factor);
								list[i].gameObject.transform.localPosition += P;
							}
							if (touch.deltaPosition.y < 0.0f)
							{
								Vector3 P = new Vector3(A*Time.deltaTime*factor, 0.0f, B*Time.deltaTime*factor);
								list[i].gameObject.transform.localPosition -= P;
							}
							if (touch.deltaPosition.x > 0.0f)
							{
								Vector3 P = new Vector3(C*Time.deltaTime*factor, 0.0f, D*Time.deltaTime*factor);
								list[i].gameObject.transform.localPosition += P;
							}
							if (touch.deltaPosition.x < 0.0f)
							{
								Vector3 P = new Vector3(C*Time.deltaTime*factor, 0.0f, D*Time.deltaTime*factor);
								list[i].gameObject.transform.localPosition -= P;
							}
						}
						break;
					case TouchPhase.Ended:
						break;
					default:
						break;
				}
			}
			
			if(Input.touches.Length == 2)
			{
				Touch touch = Input.touches[0];
				switch(touch.phase)
				{
					case TouchPhase.Began: 
						ray = Camera.main.ScreenPointToRay(touch.position);
						if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
						{
							list[i] = hit.collider.gameObject;
						}
						break;
					case TouchPhase.Moved:
						if(list[i])
						{
							list[i].gameObject.transform.Rotate(new Vector3(0, touch.deltaPosition.x*2.0f, 0),Space.Self);
						}
						break;
					case TouchPhase.Ended:
						break;
					default:
						break;
				}
			}				
		}
	}
}
