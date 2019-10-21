using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimeFactorization : MonoBehaviour
{
	private List<int> FindFactors(int n)
	{
		List<int> result = new List<int>();
		int root = (int)System.Math.Sqrt(n);
		int k = 2;
		while(n > 1 && k <= root)
		{
			while(n % k == 0)
			{
				result.Add(k);
				n/=k;
			}
			++k;
		}
		if (n>1) result.Add(n);
		return result;
	}

	void Start()
	{
		for (int k=0; k<1000; k++)
		{
			List<int> factors = FindFactors(k);
			string caption = "";
			for (int i=0; i<factors.Count; i++) caption = caption + factors[i].ToString() + " ";
			Debug.Log(k.ToString()+": "+caption);
		}
	}
}
