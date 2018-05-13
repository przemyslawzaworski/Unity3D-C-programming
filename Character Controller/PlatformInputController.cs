using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(CharacterMotor))]
[UnityEngine.AddComponentMenu("Character/Platform Input Controller")]
public partial class PlatformInputController : MonoBehaviour
{
	public bool autoRotate;
	public float maxRotationSpeed;
	private CharacterMotor motor;
	public virtual void Awake()
	{
		this.motor = (CharacterMotor) this.GetComponent(typeof(CharacterMotor));
	}

	public virtual void Update()
	{
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		if (directionVector != Vector3.zero)
		{
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			directionLength = Mathf.Min(1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		directionVector = Camera.main.transform.rotation * directionVector;
		Quaternion camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, this.transform.up);
		directionVector = camToCharacterSpace * directionVector;
		this.motor.inputMoveDirection = directionVector;
		this.motor.inputJump = Input.GetButton("Jump");
		if (this.autoRotate && (directionVector.sqrMagnitude > 0.01f))
		{
			Vector3 newForward = this.ConstantSlerp(this.transform.forward, directionVector, this.maxRotationSpeed * Time.deltaTime);
			newForward = this.ProjectOntoPlane(newForward, this.transform.up);
			this.transform.rotation = Quaternion.LookRotation(newForward, this.transform.up);
		}
	}

	public virtual Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
	{
		return v - Vector3.Project(v, normal);
	}

	public virtual Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
	{
		float value = Mathf.Min(1, angle / Vector3.Angle(from, to));
		return Vector3.Slerp(from, to, value);
	}

	public PlatformInputController()
	{
		this.autoRotate = true;
		this.maxRotationSpeed = 360;
	}

}