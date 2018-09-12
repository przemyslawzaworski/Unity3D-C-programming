using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Domain : MonoBehaviour 
{
	void Union (int[] A, int[] B)
	{
		var Table = A.Union(B);
		for (int i=0; i<Table.Count(); i++)
		{
			Debug.Log(Table.ElementAt(i));
		}
	}
	
	void Subtraction (int[] A, int[] B)
	{
		var Table = A.Except(B);
		for (int i=0;i<Table.Count();i++)
		{
			Debug.Log(Table.ElementAt(i));
		}
	}
	
	void Intersection (int[] A, int[] B)
	{
		var Table = A.Intersect(B);
		for (int i=0;i<Table.Count();i++)
		{
			Debug.Log(Table.ElementAt(i));
		}
	}
	
	void Distinct (int[] A)
	{
		var Table = A.Distinct();
		for (int i=0;i<Table.Count();i++)
		{
			Debug.Log(Table.ElementAt(i));
		}
	}	
	
	void Start () 
	{
		int[] a = new int[] {1,2,3,4};  
		int[] b = new int[] {1,5,6,7};
		int[] c = new int[] {1,1,5,5,6,7};  
		Union (a,b);
		Debug.Log("");
		Subtraction (a,b);
		Debug.Log("");		
		Intersection (a,b);
		Debug.Log("");		
		Distinct (c);
	}
	
}
