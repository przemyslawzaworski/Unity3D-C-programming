/*
https://en.wikipedia.org/wiki/Projectile_motion
https://amesweb.info/Physics/Projectile-Motion-Calculator.aspx
https://www.weber.edu/wsuimages/mollysmith/3500Presentations/Kinematics%20of%20Projectile%20Motion.pptx
https://en.wikipedia.org/wiki/Terminal_velocity
http://www.calctool.org/CALC/eng/aerospace/terminal
https://demonstrations.wolfram.com/ProjectileWithAirDrag/#more

To Do:
- Further optimization ? (reduce trigonometry ?);
- Trajectory of a projectile with Newton drag (also read about Reynolds numbers), it is more difficult to be solved analitycally, 
  but may brings even more realistic behaviour.
*/

using System;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
	public bool AirResistance = true;
	public float Velocity = 300.0f;
	public float Mass = 0.5f;
	public float DragCoefficient = 0.3f;
	public float Duration = 5.0f;

	GameObject _Bullet;
	Vector3 _StartPosition, _PreviousPosition;	
	float _Area, _Distance, _HorizontalAngle, _StartTime, _TerminalVelocity, _VerticalAngle;
	MaterialPropertyBlock _PropertyBlock;	
	bool _UpdateProjectile = false;

	float GetVerticalAngle(Transform t)
	{
		return (-1.0f) * t.eulerAngles.x;
	}

	float GetHorizontalAngle(Transform t)
	{
		float angle = 360.0f - t.eulerAngles.y + 90.0f;
		if (angle > 360.0f) angle = angle - 360.0f;
		return angle;
	}

	// Projectile motion in a uniform gravitational 3D field without air resistance:
	// startPos = position of bullet, when bullet was shot;
	// startTime = time, when bullet was shot;
	// velocity = initial velocity (initial speed of bullet, when bullet was shot, for example 800 m/s);
	// theta = horizontal angle of initial velocity in radians, computed from GetHorizontalAngle(projectileToShoot.transform) * Mathf.Deg2Rad;
	// phi = vertical angle of initial velocity in radians, computed from GetVerticalAngle(projectileToShoot.transform) * Mathf.Deg2Rad;
	Vector3 ProjectileDisplacement (Vector3 startPos, float startTime, float velocity, float theta, float phi)
	{
		float time = Time.time - startTime;
		float x = velocity * time * Mathf.Cos(theta) * Mathf.Cos(phi);
		float y = velocity * time * Mathf.Sin(phi) - 0.5f * 9.8f * time * time;
		float z = velocity * time * Mathf.Sin(theta) * Mathf.Cos(phi);
		return startPos + new Vector3(x, y, z);
	}

	// Mass = kg
	// Drag coefficient (for bullets = 0.3)
	// The density of air at sea level = 1.5
	// Projected area of object (object with size 2 cm x 5 cm = 0.001 m2 etc.)
	float TerminalVelocity(float mass, float drag, float density, float area)
	{
		float gravity = 9.8066f;
		return (float)System.Math.Sqrt((2.0f * mass * gravity) / (density * area * drag));
	}

	// Projectile motion in a uniform gravitational 3D field with air resistance (Stoke's equation):
	// startPos = position of bullet, when bullet was shot;
	// startTime = time, when bullet was shot;
	// v0 = initial velocity (initial speed of bullet, when bullet was shot, for example 800 m/s);
	// vt = terminal velocity, computed from TerminalVelocity function;
	// theta = horizontal angle of initial velocity in radians, computed from GetHorizontalAngle(projectileToShoot.transform) * Mathf.Deg2Rad;
	// phi = vertical angle of initial velocity in radians, computed from GetVerticalAngle(projectileToShoot.transform) * Mathf.Deg2Rad; 
	Vector3 ProjectileDisplacementWithAirDrag(Vector3 startPos, float startTime, float v0, float vt, float theta, float phi)
	{
		float t = Time.time - startTime;
		float g = 9.8066f;
		double e = Math.E;  // 2.7182818284590451
		float x = (float)((v0 * vt / g) * Math.Cos(theta) * Math.Cos(phi) * (1.0f - Math.Pow(e, -g * t / vt)));
		float y = (float)((vt / g) * (v0 * Math.Sin(phi) + vt) * (1.0f - Math.Pow(e, -g * t / vt)) - (vt * t));
		float z = (float)((v0 * vt / g) * Math.Sin(theta) * Math.Cos(phi) * (1.0f - Math.Pow(e, -g * t / vt)));
		return startPos + new Vector3(x, y, z);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !_UpdateProjectile)  // initialize
		{
			_Bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_Bullet.transform.parent = Camera.main.transform;
			_Bullet.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			_Bullet.transform.localPosition = new Vector3(0.0f, 0.0f, 2.0f);
			_Bullet.transform.localEulerAngles = Vector3.zero;
			float radius = _Bullet.GetComponent<SphereCollider>().radius * _Bullet.transform.localScale.x;
			_Area = Mathf.PI * radius * radius;
			_StartPosition = _Bullet.transform.position;
			_StartTime = Time.time;
			_TerminalVelocity = TerminalVelocity(Mass, DragCoefficient, 1.5f, _Area);
			_HorizontalAngle = GetHorizontalAngle(_Bullet.transform) * Mathf.Deg2Rad;
			_VerticalAngle = GetVerticalAngle(_Bullet.transform) * Mathf.Deg2Rad;
			_PropertyBlock = new MaterialPropertyBlock();
			_Bullet.GetComponent<Renderer>().GetPropertyBlock(_PropertyBlock);
			if (AirResistance)
				_PropertyBlock.SetColor("_Color", Color.red);
			else
				_PropertyBlock.SetColor("_Color", Color.blue);
			_Bullet.GetComponent<Renderer>().SetPropertyBlock(_PropertyBlock);
			_Bullet.transform.parent = null;
			_UpdateProjectile = true;
		}
		
		if (((Time.time - _StartTime) > Duration) && _UpdateProjectile) 
		{
			_UpdateProjectile = false;
			Destroy(_Bullet);
		}
		
		if (_UpdateProjectile)
		{
			_PreviousPosition = _Bullet.transform.position;
			if (AirResistance)
			{
				_Bullet.transform.position = ProjectileDisplacementWithAirDrag(_StartPosition, _StartTime, Velocity, _TerminalVelocity, _HorizontalAngle, _VerticalAngle);
			}
			else
			{
				_Bullet.transform.position = ProjectileDisplacement (_StartPosition, _StartTime, Velocity, _HorizontalAngle, _VerticalAngle);
			}
			_Distance = Vector3.Distance(_PreviousPosition, _Bullet.transform.position);
		}
	}

	void FixedUpdate()
	{
		if (_UpdateProjectile)
		{
			RaycastHit hit;
			if (Physics.Raycast(_PreviousPosition, _Bullet.transform.position - _PreviousPosition, out hit, _Distance))
			{
				Debug.Log("Hit");
			}
		}
	}
}