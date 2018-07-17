// A quaternion is a four-element vector that can be used to encode any rotation in a 3D coordinate system.
// Technically, a quaternion is composed of one real element and three complex elements.

using UnityEngine;

public class quaternion : MonoBehaviour 
{
	Vector4 EulerToQuaternion(Vector3 p)
	{
		p.x *= Mathf.Deg2Rad; 
		p.y *= Mathf.Deg2Rad;
		p.z *= Mathf.Deg2Rad;
		Vector4 q;
		float cy = Mathf.Cos(p.z * 0.5f);
		float sy = Mathf.Sin(p.z * 0.5f);
		float cr = Mathf.Cos(p.y * 0.5f);
		float sr = Mathf.Sin(p.y * 0.5f);
		float cp = Mathf.Cos(p.x * 0.5f);
		float sp = Mathf.Sin(p.x * 0.5f);
		q.w = cy * cr * cp + sy * sr * sp;
		q.x = cy * cr * sp + sy * sr * cp;	
		q.y = cy * sr * cp - sy * cr * sp;
		q.z = sy * cr * cp - cy * sr * sp;
		return q;
	}

	Vector3 QuaternionToEuler (Vector4 p)
	{
		Vector3 v;
		Vector4 q = new Vector4 (p.w, p.z, p.x, p.y);
		v.y = Mathf.Atan2 (2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));
		v.x = Mathf.Asin (2f * (q.x * q.z - q.w * q.y)); 
		v.z = Mathf.Atan2 (2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));
		v *= Mathf.Rad2Deg;
		v.x = v.x>360 ? v.x-360 : v.x;
		v.x = v.x<0 ? v.x+360 : v.x; 
		v.y = v.y>360 ? v.y-360 : v.y;
		v.y = v.y<0 ? v.y+360 : v.y; 
		v.z = v.z>360 ? v.z-360 : v.z;
		v.z = v.z<0 ? v.z+360 : v.z; 
		return v;
	}

	void Start() 
	{
		Vector3 euler = new Vector3(Random.Range(-360f,360f),Random.Range(-360f,360f),Random.Range(-360f,360f));
		Vector4 quaternion = new Vector4 (this.transform.rotation.x,this.transform.rotation.y,this.transform.rotation.z,this.transform.rotation.w);
		Debug.Log(EulerToQuaternion(euler).ToString("F8"));	
		Debug.Log(Quaternion.Euler(euler).ToString("F8"));
		Debug.Log(QuaternionToEuler(quaternion).ToString("F8"));
		Debug.Log(this.transform.rotation.eulerAngles.ToString("F8"));
	}
}
