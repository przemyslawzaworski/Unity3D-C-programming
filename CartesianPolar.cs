using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartesianPolar : MonoBehaviour 
{
	public GameObject Sphere;
	
	Vector2 CartesianToPolar (Vector2 p)
	{
		double radius = System.Math.Sqrt((p.x*p.x) + (p.y*p.y));
		double angle = System.Math.Atan2(p.y, p.x);
		return new Vector2((float)radius, (float)angle);
	}
	
	Vector2 PolarToCartesian (Vector2 p)  //p.x = radius, p.y = theta
	{
		double x = p.x * System.Math.Cos(p.y);
		double y = p.x * System.Math.Sin(p.y);	
		return new Vector2((float)x,(float)y);
	}
	
	bool IsInsideCircle (Vector2 center, Vector2 point,float radius)
	{
		double D = System.Math.Sqrt(System.Math.Pow(center.x - point.x, 2) + System.Math.Pow(center.y - point.y, 2));
		return D <= radius;
	}
	
	void Start () 
	{
		Vector2 center = new Vector2(Sphere.transform.position.x,Sphere.transform.position.z);
		float scale = Mathf.Max(Sphere.transform.localScale.z,Mathf.Max(Sphere.transform.localScale.x,Sphere.transform.localScale.y));
		for (int i=0;i<500;i++)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(Random.Range(center.x-20.0f,center.x+20.0f),0.0f,Random.Range(center.y-20.0f,center.y+20.0f));
			Vector2 point = new Vector2(cube.transform.position.x,cube.transform.position.z);
			while (IsInsideCircle(center,point,scale*0.5f))
			{
				cube.GetComponent<Renderer>().material.SetColor("_Color",new Color(1.0f,0.0f,0.0f,1.0f));
				Vector2 p = CartesianToPolar(new Vector2(point.x-center.x,point.y-center.y));
				p.x += 6.0f;
				Vector2 c = PolarToCartesian(p);
				cube.transform.position = new Vector3(c.x+center.x,0.0f,c.y+center.y);	
				point = new Vector2(cube.transform.position.x,cube.transform.position.z);
			}
		}
	}

}
