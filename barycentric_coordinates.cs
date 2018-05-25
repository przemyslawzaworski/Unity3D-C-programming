// references:
// http://wiki.unity3d.com/index.php/Barycentric
// https://en.wikipedia.org/wiki/Barycentric_coordinate_system

using UnityEngine;

public class barycentric_coordinates : MonoBehaviour 
{

	Vector3 barycentric(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p0)
	{
		Vector2 a = p2 - p3;
		Vector2 b = p1 - p3;
		Vector2 c = p0 - p3;
		float ab = a.x * b.x + a.y * b.y;
		float ac = a.x * c.x + a.y * c.y;
		float bc = b.x * c.x + b.y * c.y;
		float m = a.x * a.x + a.y * a.y;
		float n = b.x * b.x + b.y * b.y;		
		float d = m * n - ab * ab;
		float u = (m * bc - ab * ac) / d;
		float v = (n * ac - ab * bc) / d;
		float w = 1.0f - u - v;
		return new Vector3(u,v,w);
	}
	
	Vector3 barycentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p0)
	{
		Vector3 a = p2 - p3;
		Vector3 b = p1 - p3;
		Vector3 c = p0 - p3;
		float ab = a.x * b.x + a.y * b.y + a.z * b.z;
		float ac = a.x * c.x + a.y * c.y + a.z * c.z;
		float bc = b.x * c.x + b.y * c.y + b.z * c.z;
		float m = a.x * a.x + a.y * a.y + a.z * a.z;
		float n = b.x * b.x + b.y * b.y + b.z * b.z;
		float d = m * n - ab * ab;
		float u = (m * bc - ab * ac) / d;
		float v = (n * ac - ab * bc) / d;
		float w = 1.0f - u - v;
		return new Vector3(u,v,w);
	}
	
	Vector3 barycentric(Vector4 p1, Vector4 p2, Vector4 p3, Vector4 p0)
	{
		Vector4 a = p2 - p3;
		Vector4 b = p1 - p3;
		Vector4 c = p0 - p3;
		float ab = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		float ac = a.x * c.x + a.y * c.y + a.z * c.z + a.w * c.w;
		float bc = b.x * c.x + b.y * c.y + b.z * c.z + b.w * c.w;
		float m = a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w;
		float n = b.x * b.x + b.y * b.y + b.z * b.z + b.w * b.w;		
		float d = m * n - ab * ab;
		float u = (m * bc - ab * ac) / d;
		float v = (n * ac - ab * bc) / d;
		float w = 1.0f - u - v;
		return new Vector3(u,v,w);
	}
	
	bool InsideTriangle (Vector3 p)
	{
		return (p.x >= 0.0f) && (p.x <= 1.0f) && (p.y >= 0.0f) && (p.y <= 1.0f) && (p.z >= 0.0f) && (p.z <= 1.0f);
	}

	Vector2 Interpolate(Vector2 v1, Vector2 v2, Vector2 v3, Vector3 p)
	{
		return v1 * p.x + v2 * p.y + v3 * p.z;
	}
	
	Vector3 Interpolate(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 p)
	{
		return v1 * p.x + v2 * p.y + v3 * p.z;
	}
	
	Vector4 Interpolate(Vector4 v1, Vector4 v2, Vector4 v3, Vector3 p)
	{
		return v1 * p.x + v2 * p.y + v3 * p.z;
	}
	
	void Start () 
	{
		Vector3 v1 = new Vector3 (0.0f,0.0f,0.0f);
		Vector3 v2 = new Vector3 (0.0f,1.0f,0.0f);
		Vector3 v3 = new Vector3 (1.0f,0.0f,0.0f);
		Vector3 p  = new Vector3 (0.8f,0.7f,0.0f); 
		Vector3 k = barycentric(v1,v2,v3,p);   
		bool result = InsideTriangle(k);   //false
		Debug.Log(result);
	}
	
	void Update () 
	{
		
	}
}
