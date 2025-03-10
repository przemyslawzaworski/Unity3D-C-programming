using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Android : MonoBehaviour
{
	Vector2 GetFreeMemory()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject activityManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "activity");
		AndroidJavaObject memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
		activityManager.Call("getMemoryInfo", memoryInfo);
		long availMem = memoryInfo.Get<long>("availMem");
		float totalMemory = memoryInfo.Get<long>("totalMem") / (1024 * 1024);
		float freeMemory = availMem / (1024 * 1024);
		return new Vector2(freeMemory, totalMemory);
	}

	void Start()
	{
		#if !UNITY_EDITOR
		Vector2 memory = GetFreeMemory();
		#endif
	}
}