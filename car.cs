using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class car : MonoBehaviour 
{
	[Header("Wheel Collider Members")]
	public WheelCollider[] frontWheels;
	public WheelCollider[] rearWheels;	
	
	[Header("Wheel Collider Properties")]
	[Tooltip("Brake torque expressed in Newton metres.")]
	[Range(0.0f,1000.0f)]	
	public float brakeTorque = 0.0f;
	[Tooltip("The center of the wheel, measured in the object's local space.")]	
	public Vector3 center;
	[Tooltip("Application point of the suspension and tire forces measured from the base of the resting wheel.")]		
	[Range(-1.0f,1.0f)]	
	public float forceAppPointDistance = 0.5f;
	[Tooltip("The mass of the wheel, expressed in kilograms.")]
	[Range(10.0f,500.0f)]	
	public float mass = 100.0f;
	[Tooltip("Motor torque on the wheel axle expressed in Newton metres.")]	
	[Range(10.0f,1000.0f)]	
	public float motorTorque = 400.0f;
	[Tooltip("The radius of the wheel, measured in local space.")]	
	public float radius = 0.5f;
	[Tooltip("If center of mass is zero, sprungMass = Rigidbody.mass * 0.25 .")]		
	public float sprungMass;
	[Tooltip("Steering angle in degrees, always around the local y-axis.")]	
	public float steerAngle = 30.0f;
	[Tooltip("Maximum extension distance of wheel suspension, measured in local space.")]
	[Range(0.0f,1.0f)]		
	public float suspensionDistance = 0.3f;
	[Tooltip("The damping rate of the wheel.")]	
	[Range(0.0f,20.0f)]	
	public float wheelDampingRate = 8.0f;
	
	[Header("Wheel Collider Forward Friction")]
	[Range(0.0f,10.0f)]
	public float asymptoteSlipF = 0.8f;
	[Range(0.0f,10.0f)]	
	public float asymptoteValueF = 0.5f;
	[Range(0.0f,10.0f)]	
	public float extremumSlipF = 0.4f;
	[Range(0.0f,10.0f)]	
	public float extremumValueF = 1.0f;
	[Range(0.0f,10.0f)]	
	public float stiffnessF = 1.0f;
	
	[Header("Wheel Collider Sideways Friction")]
	[Range(0.0f,10.0f)]	
	public float asymptoteSlipS = 0.8f;
	[Range(0.0f,10.0f)]	
	public float asymptoteValueS = 0.5f;
	[Range(0.0f,10.0f)]	
	public float extremumSlipS = 0.4f;
	[Range(0.0f,10.0f)]	
	public float extremumValueS = 1.0f;
	[Range(0.0f,10.0f)]	
	public float stiffnessS = 1.0f;

	[Header("Wheel Collider Suspension Spring")]
	[Range(0.0f,100000.0f)]	
	public float spring = 35000.0f;
	[Range(0.0f,10000.0f)]	
	public float damper = 4500.0f;
	[Range(0.0f,1.0f)]	
	public float targetPosition = 0.5f;

	[Header("Wheel Collider Substeps")]
	public float speedThreshold = 5.0f;
	public int stepsBelowThreshold = 22;
	public int stepsAboveThreshold = 2;
	
	[Header("Vehicle Rigidbody")]
	public GameObject Vehicle;
	public Rigidbody rigidBody;
	public Vector3 CenterOfMass;
	public float DownForce = 1.0f;
		
	public void SetGeometry(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) return;
		Transform visualWheel = collider.transform.GetChild(0); 
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	float SteerAngle (float x) //reduce current steer angle depends on car speed
	{
		float temp = x;
		float speed = Vector3.Magnitude(rigidBody.velocity) * 2.0f;
		float ratio = speed / x;
		temp *= 1.0F - ratio;
		return Mathf.Clamp(temp, 15.0F, x);
	}
	
	void Turbo (WheelCollider w)
	{
		float rot = Vehicle.transform.rotation.eulerAngles.x;
		if (rot>180) rot=rot-360.0f;
		rot = Mathf.Abs(rot);
		w.motorTorque = w.motorTorque + w.motorTorque * (Mathf.Pow(rot,1.0f+rot*0.008f) * 0.045f);		
	}
	
	public void FixedUpdate()
	{
		frontWheels[0].ConfigureVehicleSubsteps(speedThreshold,stepsBelowThreshold,stepsAboveThreshold);
		rigidBody.AddForce(-rigidBody.transform.up * DownForce * rigidBody.velocity.magnitude);
		rigidBody.centerOfMass = CenterOfMass;
		for (int i=0;i<frontWheels.Length;i++)
		{
			frontWheels[i].brakeTorque = brakeTorque;
			frontWheels[i].center = center;
			frontWheels[i].forceAppPointDistance = forceAppPointDistance;
			frontWheels[i].mass = mass;
			frontWheels[i].motorTorque = motorTorque * Input.GetAxis("Vertical");
			frontWheels[i].radius = radius;
			frontWheels[i].steerAngle = SteerAngle( steerAngle) * Input.GetAxis("Horizontal");
			frontWheels[i].suspensionDistance = suspensionDistance;
			frontWheels[i].wheelDampingRate = wheelDampingRate;
			WheelFrictionCurve FFC = new WheelFrictionCurve();
			FFC.asymptoteSlip = asymptoteSlipF;
			FFC.asymptoteValue = asymptoteValueF;
			FFC.extremumSlip = extremumSlipF;
			FFC.extremumValue = extremumValueF;
			FFC.stiffness = stiffnessF;
			frontWheels[i].forwardFriction = FFC;
			WheelFrictionCurve SFC = new WheelFrictionCurve();
			SFC.asymptoteSlip = asymptoteSlipS;
			SFC.asymptoteValue = asymptoteValueS;
			SFC.extremumSlip = extremumSlipS;
			SFC.extremumValue = extremumValueS;
			SFC.stiffness = stiffnessS;
			frontWheels[i].sidewaysFriction = SFC;
			JointSpring suspensionSpring = new JointSpring();
			suspensionSpring.spring = spring;          
			suspensionSpring.damper = damper;          
			suspensionSpring.targetPosition = targetPosition;
			frontWheels[i].suspensionSpring = suspensionSpring;	
			SetGeometry(frontWheels[i]);
			Turbo(frontWheels[i]);
		}
		for (int j=0;j<rearWheels.Length;j++)
		{
			rearWheels[j].center = center;
			rearWheels[j].forceAppPointDistance = forceAppPointDistance;
			rearWheels[j].mass = mass;
			rearWheels[j].radius = radius;
			rearWheels[j].suspensionDistance = suspensionDistance;
			rearWheels[j].wheelDampingRate = wheelDampingRate;
			WheelFrictionCurve FFC = new WheelFrictionCurve();
			FFC.asymptoteSlip = asymptoteSlipF;
			FFC.asymptoteValue = asymptoteValueF;
			FFC.extremumSlip = extremumSlipF;
			FFC.extremumValue = extremumValueF;
			FFC.stiffness = stiffnessF;
			rearWheels[j].forwardFriction = FFC;
			WheelFrictionCurve SFC = new WheelFrictionCurve();
			SFC.asymptoteSlip = asymptoteSlipS;
			SFC.asymptoteValue = asymptoteValueS;
			SFC.extremumSlip = extremumSlipS;
			SFC.extremumValue = extremumValueS;
			SFC.stiffness = stiffnessS;
			rearWheels[j].sidewaysFriction = SFC;
			JointSpring suspensionSpring = new JointSpring();
			suspensionSpring.spring = spring;          
			suspensionSpring.damper = damper;          
			suspensionSpring.targetPosition = targetPosition;
			rearWheels[j].suspensionSpring = suspensionSpring;
			SetGeometry(rearWheels[j]);
			Turbo(rearWheels[j]);
		}
	}

}