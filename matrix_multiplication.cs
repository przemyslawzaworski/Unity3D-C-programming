using UnityEngine;
using System.Collections;

public class matrix_multiplication : MonoBehaviour 
{
	float[,] mul (float[,] a, float[,] b, int p, int k)
	{
		float[,] c = new float[p,k];
		for (int y=0;y<p;y++)
		{
			for (int x=0;x<p;x++)
			{
				float total =0.0f;
				for (int n=0;n<p;n++)
				{
					c[x,y]=b[n,y]*a[x,n]+total;
					total=c[x,y];					
				}
			}
		}
		return c;
	}
	
	void Start () 
	{
		float[,] matrixA = new float [4,4] { {4.6f,5f,9f,2f},{5f,3f,-4f,3f},{2f,3f,2f,4f},{1f,4f,7f,3f}};
		float[,] matrixB = new float [4,4] { {5f,8f,6f,3f},{2f,1.7f,4f,2f},{4f,2f,5f,3f},{1f,2f,8f,3f}};
		float[,] matrixC = mul(matrixA,matrixB,4,4);
		for (int i=0;i<4;i++) Debug.Log(matrixC[i,0]+" "+matrixC[i,1]+" "+matrixC[i,2]+" "+matrixC[i,3]);	
	}
}
