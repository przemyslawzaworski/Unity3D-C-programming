//Calculate combinations without repetition
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cwr : MonoBehaviour 
{
	public static void printc (int[] comb, int k)
	{
		string line="";
		for (int i = 0; i < k; i++) 
		{
			line=line+(comb [i] + 1);
			if (i < k-1) line=line+ (',');
		}
		Debug.Log (line);
	}

	public static bool next_comb (int[] comb, int k, int n)
	{
		int i = k - 1;
		comb [i]++;
		while ((i > 0) && (comb [i] >= n - k + 1 + i)) 
		{
			i--;
			comb [i]++;
		}
		if (comb [0] > n - k) return  false;
		for (i++; i < k; i++) 
		{
			comb [i] = comb [i - 1] + 1;
		}
		return  true;
	}
	
	void Start () 
	{
		int n = 12;   //Number of elements to choose from
		int k = 7;   //Number of elements chosen
		int z = 1;
		int[] comb = new int[k];
		for (int ii = 0; ii < k; ii++) 
		{
			comb [ii] = ii;
		}
		printc (comb, k);
		while (next_comb (comb, k, n)) 
		{
			printc (comb, k);
			z++;
		}
		string result = "N = "+n+"  K = "+k+"   "+ " Number of combinations: "+ z.ToString ();	
		Debug.Log(result);
	}
}
