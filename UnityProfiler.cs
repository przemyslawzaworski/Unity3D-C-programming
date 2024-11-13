using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;

// For best results, use Development Build
public class UnityProfiler : MonoBehaviour
{
	void BeginProfiler()
	{
		Profiler.logFile = System.IO.Path.Combine(Application.persistentDataPath, "log.raw");
		Profiler.enableBinaryLog = true;
		Profiler.enabled = true;
		Profiler.maxUsedMemory = 256 * 1024 * 1024;
	}

	void EndProfiler()
	{
		Profiler.enabled = false;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			BeginProfiler();
		}
		else if (Input.GetKeyDown(KeyCode.P))
		{
			EndProfiler();
		}
	}
}