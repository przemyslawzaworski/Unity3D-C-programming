using UnityEngine;
using System.Collections;

public class wireframe : MonoBehaviour 
{
	public bool enable;
	public Camera main_camera;

	void OnPreRender() 
	{
		if (enable) 
		{
			main_camera.clearFlags = CameraClearFlags.SolidColor;
			GL.wireframe = true;
		}
		else
		{
			GL.wireframe = false;
			main_camera.clearFlags = CameraClearFlags.Skybox;
		}
	}

	void OnPostRender() 
	{
		GL.wireframe = false;
	}
}