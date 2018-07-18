// QuaternionRotation() is better when SteeringWheelMaxAngle not exceed range[0..180] otherwise use EulerRotation().
// Przemyslaw Zaworski 18.07.2018
using UnityEngine;

public class rotation : MonoBehaviour 
{
	public float SteeringWheelMaxAngle = 60.0f;
	public float SteeringWheelSpeed = 3.0f;

	private Vector3 _StartEuler;
	private float _TargetAngle;
	private float _CurrentAngle;

	void LoadEulerRotation()
	{
		_TargetAngle = 0.0f;
		_CurrentAngle = 0.0f;
		_StartEuler = this.transform.localEulerAngles;
	}

	void LoadQuaternionRotation()
	{
		_StartEuler = this.transform.localEulerAngles;
	}

	void EulerRotation()
	{
		_TargetAngle = Input.GetAxis("Horizontal") * SteeringWheelMaxAngle;
		_CurrentAngle = Mathf.Lerp(_CurrentAngle, _TargetAngle, Time.deltaTime * SteeringWheelSpeed);
		this.transform.localRotation = Quaternion.Euler(0.0f, _CurrentAngle, 0.0f) * Quaternion.Euler(_StartEuler);
	}

	void QuaternionRotation()
	{
		Quaternion target = Quaternion.Euler(0, Input.GetAxis("Horizontal") * SteeringWheelMaxAngle,0) * Quaternion.Euler(_StartEuler);
		this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, target, Time.deltaTime * SteeringWheelSpeed);
	}

	void Start()
	{
		LoadEulerRotation();
	}

	void Update()
	{
		EulerRotation();
	}
	
}
