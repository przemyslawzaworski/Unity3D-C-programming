using UnityEngine;

// Physics equations taken from: "Physics for Game Programmers" by Grant Palmer
public class ArmorPenetration : MonoBehaviour
{
	[Tooltip("Projectile mass [kg]")]
	public float Mass = 0.0082f;
	[Tooltip("Projectile (muzzle) velocity [m/s]")]
	public float Velocity = 440.0f;
	[Tooltip("Steel armor thickness [meters]")]
	public float Thickness = 0.01f;
	[Tooltip("Projectile diameter [meters]")]
	public float Diameter = 0.009f;
	[Tooltip("Angle of impact [degrees], angle is zero for projectile strike perpendicular to the steel plate")]
	public float Angle = 0.0f;

	// m - projectile mass [kg]
	// v - projectile (muzzle) velocity [m/s]
	// t - steel armor thickness [meters]
	// d - projectile diameter [meters]
	// phi - angle of impact [degrees], angle is zero for projectile strike perpendicular to the steel plate
	bool IsArmorCanBePenetrated (float m, float v, float t, float d, float phi)
	{
		float f = 1.8288f * (t / d - 0.45f) * (phi * phi + 2000f) + 12192f;  //armor resistance coefficient
		float k = 0.5f * m * v * v;  //kinetic energy of projectile
		float s = Mathf.Cos (phi * Mathf.Deg2Rad);
		float n = 8.025f * (t * d * d * f * f) / (s * s);  //minimum kinetic energy required to penetrate armor
		return (k > n);
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 50), "Calculate"))
		{
			bool result = IsArmorCanBePenetrated (Mass, Velocity, Thickness, Diameter, Angle);
			Debug.Log(result);
		}
	}
}