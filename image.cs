using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
 
public class image : MonoBehaviour
{
    public int Resolution = 256;
    public GameObject Target;
 
    JobHandle HandleJob;  
    NativeArray<Color32> PixelArray;
    Texture2D Map;
   
    struct CalculateJob : IJob
    {
        public NativeArray<Color32> Pixels;
        public float Width;
        public float Height;
        public int Dimension;
		public float Timer;
       
        public void Execute()
        {
			for (int i = 0; i < Pixels.Length; i++)
			{
				Vector2 uv = new Vector2(Width/Dimension,Height/Dimension);
				Pixels[i] = new Color32 ((byte)(uv.x*255),(byte)(uv.y*255),(byte)(Timer*255),255) ;
				Width++;
				if (Width>=Dimension) Width=0.0f;
				if ((i+1)%Dimension==0) Height++;
			}
        }
    }
 
    void Start()
    {
		if (QualitySettings.activeColorSpace==ColorSpace.Gamma)
			Map = new Texture2D(Resolution,Resolution, TextureFormat.RGBA32, false,false);
		else
			Map = new Texture2D(Resolution,Resolution, TextureFormat.RGBA32, false,true);
		Map.wrapMode = TextureWrapMode.Clamp;
		Target.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Texture"));
		Target.GetComponent<Renderer>().material.mainTexture = Map;
    }
   
    void Update()
    {

        PixelArray = Map.GetRawTextureData<Color32>();
        int InitHeight = 0;
        int InitWidth = 0;
		float SetTime = Mathf.Sin(Time.time)*0.5f+0.5f;
        CalculateJob calculate_job = new CalculateJob()
        {
            Pixels = PixelArray,
            Width = InitWidth,
            Height = InitHeight,
            Dimension = Resolution,
			Timer = SetTime
        };
        HandleJob = calculate_job.Schedule();
        Map.Apply(false);
    }
   
    public void LateUpdate()
    {
        HandleJob.Complete();
    }
}