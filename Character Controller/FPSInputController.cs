using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.RequireComponent(typeof(CharacterMotor))]
[UnityEngine.AddComponentMenu("Character/FPS Input Controller")]
public partial class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;
	
	public virtual void Awake()
	{
		this.motor = (CharacterMotor) this.GetComponent(typeof(CharacterMotor));
	}

	public virtual void Update()
	{
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (directionVector != Vector3.zero)
		{
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			directionLength = Mathf.Min(1, directionLength);
			directionLength = directionLength * directionLength;
			directionVector = directionVector * directionLength;
		}
		this.motor.inputMoveDirection = this.transform.rotation * directionVector;
		this.motor.inputJump = Input.GetButton("Jump");
	}

}