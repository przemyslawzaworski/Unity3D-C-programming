using UnityEngine;
using System.Collections;

public class random_set : MonoBehaviour 
{
	
	public static bool ArrayContains(int[] array, int p) 
	{
		for(int i = 0; i < array.Length; i++) if(p == array[i]) return true;	
		return false;
	}

	int[] generate_set()
	{
		int points_amount = Mathf.RoundToInt (UnityEngine.Random.Range (0.0f, 10.0f));
		int[] points_set = new int[points_amount];
		int n = 0 , element;
		while (n < points_set.Length) 
		{
			element = Mathf.RoundToInt (UnityEngine.Random.Range (0.0f, 100.0f));
			if ((ArrayContains (points_set, element) == false)) 
			{
				points_set [n] = element;
				n++;
			}
		}
		return points_set;
	}
	
	void Start () 
	{
		int[] result = generate_set();
		for (int i=0;i<result.Length;i++) Debug.Log(result[i]);
	}

}
