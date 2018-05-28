using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class intersection_tetrahedron : MonoBehaviour 
{

	bool InsideTetrahedron (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 p)
	{
		Matrix4x4 D0 = new Matrix4x4();
		D0.SetRow (0,new Vector4 (v1.x,v1.y,v1.z,1.0f));
		D0.SetRow (1,new Vector4 (v2.x,v2.y,v2.z,1.0f));
		D0.SetRow (2,new Vector4 (v3.x,v3.y,v3.z,1.0f));
		D0.SetRow (3,new Vector4 (v4.x,v4.y,v4.z,1.0f));
		
		Matrix4x4 D1 = new Matrix4x4();
		D1.SetRow (0,new Vector4 (p.x,p.y,p.z,1.0f));
		D1.SetRow (1,new Vector4 (v2.x,v2.y,v2.z,1.0f));
		D1.SetRow (2,new Vector4 (v3.x,v3.y,v3.z,1.0f));
		D1.SetRow (3,new Vector4 (v4.x,v4.y,v4.z,1.0f));

		Matrix4x4 D2 = new Matrix4x4();
		D2.SetRow (0,new Vector4 (v1.x,v1.y,v1.z,1.0f));		
		D2.SetRow (1,new Vector4 (p.x,p.y,p.z,1.0f));
		D2.SetRow (2,new Vector4 (v3.x,v3.y,v3.z,1.0f));
		D2.SetRow (3,new Vector4 (v4.x,v4.y,v4.z,1.0f));

		Matrix4x4 D3 = new Matrix4x4();
		D3.SetRow (0,new Vector4 (v1.x,v1.y,v1.z,1.0f));		
		D3.SetRow (1,new Vector4 (v2.x,v2.y,v2.z,1.0f));
		D3.SetRow (2,new Vector4 (p.x,p.y,p.z,1.0f));		
		D3.SetRow (3,new Vector4 (v4.x,v4.y,v4.z,1.0f));

		Matrix4x4 D4 = new Matrix4x4();
		D4.SetRow (0,new Vector4 (v1.x,v1.y,v1.z,1.0f));		
		D4.SetRow (1,new Vector4 (v2.x,v2.y,v2.z,1.0f));		
		D4.SetRow (2,new Vector4 (v3.x,v3.y,v3.z,1.0f));
		D4.SetRow (3,new Vector4 (p.x,p.y,p.z,1.0f));	

		float a = D0.determinant;
		float b = D1.determinant;
		float c = D2.determinant;
		float d = D3.determinant;
		float e = D4.determinant;
		
		return ( (Mathf.Sign(a)==Mathf.Sign(b)) &&  (Mathf.Sign(a)==Mathf.Sign(c)) && (Mathf.Sign(a)==Mathf.Sign(d)) && (Mathf.Sign(a)==Mathf.Sign(e)) );
	}
	
	void Start () 
	{
		Vector3 a = new Vector3(0.0f,0.0f,0.0f);
		Vector3 b = new Vector3(1.0f,0.0f,0.0f);
		Vector3 c = new Vector3(0.0f,1.0f,0.0f);
		Vector3 d = new Vector3(0.0f,0.0f,1.0f);	
		Vector3 p = new Vector3(0.1f,0.1f,-0.1f);
		bool test = InsideTetrahedron(a,b,c,d,p);
		Debug.Log(test);
	}

}
