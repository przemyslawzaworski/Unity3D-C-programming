// Author: Przemyslaw Zaworski

using UnityEngine;
using System;

public class footsteps : MonoBehaviour 
{
	[Tooltip("Time interval between steps when player is walking.")]
	public float WalkingDelay = 1.0f;
	[Tooltip("Time interval between steps when player is running.")]	
	public float RunningDelay = 0.5f;
	[Tooltip("Value to determine bound speed between walking and running.")]	
	public float WalkingMax = 4.0f;	
	[System.Serializable]
	public struct AudioStruct
	{
		public PhysicMaterial PhysicMaterialReference;
		public AudioClip AudioClipReference;
	}
	[Tooltip("Physic material and audio.")]	
	public AudioStruct[] AudioGroup;

	private AudioSource source;
	private Vector3 position;
	private CharacterController controller;
	private float speed;
	private bool isSound = false;	
	private bool isGrounded = false;
	private int ID = 0;
	
	float ComputeSpeed(GameObject item)
	{
		float s = ((item.transform.position - position).magnitude) / Time.deltaTime;
		position = item.transform.position;
		return s;
	}

	void FootstepsWithDelay(AudioClip sound, float delay,float volume)
	{
		if ((Time.time%delay)<=Time.fixedDeltaTime)
		{
			source.PlayOneShot(sound, volume);		
		}
	}

	void Footsteps(AudioClip sound,float volume)
	{
		source.PlayOneShot(sound, volume);		
	}	
	
	void Start () 
	{
		source = GetComponent<AudioSource>();	
		controller = GetComponent<CharacterController>();
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)  
	{
		try
		{
			isGrounded = hit.collider != null;
			ID = hit.collider.sharedMaterial.GetHashCode();
		}
		catch (Exception) { }
	}

	void FixedUpdate ()
	{
		if (isGrounded)
		{
			speed = ComputeSpeed(this.gameObject);
			for (int i=0;i<AudioGroup.Length;i++)
			{
				if (ID==AudioGroup[i].PhysicMaterialReference.GetHashCode()) 
				{
					if (controller.isGrounded && isSound ) 
					{
						Footsteps(AudioGroup[i].AudioClipReference,1.0f);
						isSound = false; 
						return;
					}
					
					if (!controller.isGrounded) 
					{
						isSound = true;
						return;
					}
					
					if  (speed>0.1f) 
					{
						if (speed<WalkingMax)
						{
							FootstepsWithDelay(AudioGroup[i].AudioClipReference,WalkingDelay,1.0f);
						}
						else 
						{
							FootstepsWithDelay(AudioGroup[i].AudioClipReference,RunningDelay,1.0f);
						}
					}
				}
			}
		}
	}
}