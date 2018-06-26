using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class generic : MonoBehaviour 
{
	T Sum<T>(params T[] p)
	{
		dynamic n = p[0];   //"dynamic" keyword has performance impact
		for (int i=1;i<p.Length;i++) {n+=p[i];} 
		return n;
	}
	
	void Start () 
	{
		Debug.Log(Sum(2,3,5));   //10
		Debug.Log(Sum(1.0f,2.0f,5.0f,-3.0f));   //5.0
		Debug.Log(Sum("abc","def"));   //abcdef
	}

}
