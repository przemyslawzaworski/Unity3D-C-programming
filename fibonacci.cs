using UnityEngine;
using System.Collections;

public class fibonacci : MonoBehaviour 
{
    public static int Fibonacci(int n)
    {
		int a = 0;
		int b = 1;
		int c = 0;
		for (int i = 0; i < n; i++)
		{
			c = a;
			a = b;
			b = c + b;
		}
		return a;
	}

	void Start () 
	{
		int total = 0;
		for (int k=0;k<32;k++)
		{
			total=total+Fibonacci(k);
		}
		Debug.Log("Sum of the first 32 elements of Fibonacci sequence= "+total);
	}
	
}
