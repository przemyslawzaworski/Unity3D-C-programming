using UnityEngine;

public class instantiate : MonoBehaviour 
{
	public int Resolution = 10;
	public int Offset = 2;
	
	void Start () 
	{
		int x = 0;
		int y = 0;	
		for (int i = 0; i < Resolution*Resolution; i++)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(x * Offset, 0.0F, y * Offset);
			cube.name = "Cube"+i.ToString();
			x++;
			if (x>=Resolution) x=0;
			if ((i+1)%Resolution==0) y++;
		}
	}
}
