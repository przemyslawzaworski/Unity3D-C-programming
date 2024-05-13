using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

public class DiskSpeed : MonoBehaviour
{
	async void Start()
	{
		await Benchmark();
	}

	async Task Benchmark()
	{
		string filePath = Path.Combine(Path.GetTempPath(), "testfile.bin");
		byte[] bytes = new byte[1024 * 1024 * 1024];
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		FileStream writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
		await writer.WriteAsync(bytes, 0, bytes.Length);
		writer.Close();
		stopwatch.Stop();
		float writeSpeed = bytes.Length / (float)stopwatch.ElapsedMilliseconds * 1000 / (1024 * 1024);
		stopwatch.Reset();
		stopwatch.Start();
		FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
		byte[] readData = new byte[reader.Length];
		await reader.ReadAsync(readData, 0, readData.Length);
		reader.Close();
		stopwatch.Stop();
		float readSpeed = readData.Length / (float)stopwatch.ElapsedMilliseconds * 1000 / (1024 * 1024);
		UnityEngine.Debug.Log("Write Speed: " + writeSpeed.ToString("F2") + " MB/s");
		UnityEngine.Debug.Log("Read Speed: " + readSpeed.ToString("F2") + " MB/s");
		File.Delete(filePath);
	}
}