using UnityEngine;
using System.Collections;
using System.IO;

public class noise_texture_generator : MonoBehaviour 
{
	float fract (float x)
	{
		return Mathf.Abs (x % 1);
	}
	void Start ()
	{
		Texture2D image = new Texture2D (256, 256, TextureFormat.RGBA32, false);
		for (int y = 0; y < 256; y++) 
		{
			for (int x = 0; x < 256; x++) 
			{
				float noise = fract(Mathf.Sin( Random.Range(0.0f,1.0f)*12.9898f+Random.Range(0.0f,1.0f)*78.233f )  * 43758.5453f);
				Color color = new Color (noise, noise, noise, 1.0f);
				image.SetPixel (x, y, color);
			}
		}
		image.Apply ();
		byte[] bytes = image.EncodeToPNG ();
		Object.Destroy (image);
		File.WriteAllBytes ("C:\\noise.png", bytes);
	}

}
