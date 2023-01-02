using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

public class OpenCL : MonoBehaviour
{
	public int Resolution = 1024;

	[DllImport("OpenCL.dll")]
	static extern int clGetPlatformIDs(int num_entries, IntPtr[] platforms, out int num_platforms);

	[DllImport("OpenCL.dll")]
	static extern int clGetDeviceIDs(IntPtr platform, DeviceType device_type, int num_entries, IntPtr[] devices, out int num_devices);

	[DllImport("OpenCL.dll")]
	static extern IntPtr clCreateContext(IntPtr[] properties, int num_devices, IntPtr[] devices, NotifyContext pfn, IntPtr userData, out int errorCode);

	[DllImport("OpenCL.dll")]
	static extern int clReleaseContext(IntPtr context);

	[DllImport("OpenCL.dll")]
	static extern IntPtr clCreateCommandQueue(IntPtr context, IntPtr device, long properties, out int errorCode);

	[DllImport("OpenCL.dll")]
	static extern int clReleaseCommandQueue(IntPtr commandQueue);

	[DllImport("OpenCL.dll")]
	static extern IntPtr clCreateBuffer(IntPtr context, MemoryFlags memoryFlags, int sizeInBytes, IntPtr hostPtr, out int errorCode);

	[DllImport("OpenCL.dll")]
	static extern int clReleaseMemObject(IntPtr memoryObject);

	[DllImport("OpenCL.dll")]
	static extern int clEnqueueReadBuffer(IntPtr queue, IntPtr buffer, bool isBlocking, int offset, int cb, IntPtr ptr, int num_events, IntPtr list, IntPtr objects);

	[DllImport("OpenCL.dll")]
	static extern IntPtr clCreateProgramWithSource(IntPtr context, int count, string[] programSources, int[] sourceLengths, out int errorCode);

	[DllImport("OpenCL.dll")]
	static extern int clBuildProgram(IntPtr program, int deviceCount, IntPtr[] deviceList, string options, NotifyProgram notify, IntPtr userData);

	[DllImport("OpenCL.dll")]
	static extern int clReleaseProgram(IntPtr program);

	[DllImport("OpenCL.dll")]
	static extern IntPtr clCreateKernel(IntPtr kernel, string functionName, out int errorCode);

	[DllImport("OpenCL.dll")]
	static extern int clReleaseKernel(IntPtr kernel);

	[DllImport("OpenCL.dll")]
	static extern int clSetKernelArg(IntPtr kernel, int argumentIndex, int size, ref IntPtr value);

	[DllImport("OpenCL.dll")]
	static extern int clSetKernelArg(IntPtr kernel, int argumentIndex, int size, IntPtr value);

	[DllImport("OpenCL.dll")]
	static extern int clEnqueueNDRangeKernel(IntPtr queue, IntPtr kernel, int dim, ref Vector globalWorkOffset, ref Vector globalWorkSize, ref Vector localWorkSize, int count, IntPtr[] list, IntPtr objects);

	[DllImport("OpenCL.dll")]
	static extern int clGetProgramBuildInfo(IntPtr program, IntPtr device, ProgramBuildInfoString info, int size, StringBuilder p, out int ret);	

	delegate void NotifyContext(string errorInfo, IntPtr privateInfoSize, int cb, IntPtr userData);
	delegate void NotifyProgram(IntPtr program, IntPtr userData);

	byte[] _Bytes;
	Kernel _Kernel;
	CommandQueue _CommandQueue;
	Buffer _Buffer;
	Texture2D _Texture;
	Vector _GlobalWorkSize;
	Vector _LocalWorkSize;

	static string ComputeKernel = 
	@"
		__kernel void mainImage(__global uchar4 *fragColor, float iTime, int res)
		{
			unsigned int id = get_global_id(0);
			int2 iResolution = (int2)(res, res);
			int2 fragCoord = (int2)(id % iResolution.x, id / iResolution.x);
			float2 uv = (float2)(fragCoord.x / (float)iResolution.x * 8.0f, fragCoord.y / (float)iResolution.y * 8.0f);
			for(int j=1; j<4; j++)
			{
				uv.x += sin(iTime + uv.y * (float)j);
				uv.y += cos(iTime + uv.x * (float)j);
			}
			float3 q = (float3)(0.5+0.5*cos(4.0+uv.x), 0.5+0.5*cos(4.0+uv.y+2.0), 0.5+0.5*cos(4.0+uv.x+4.0));
			fragColor[id] = (uchar4)(q.x * 255, q.y * 255, q.z * 255, 255);
		};
	";

	class Context
	{
		public IntPtr Pointer { get; private set; }

		public Context(params IntPtr[] devices)
		{
			Pointer = clCreateContext(null, devices.Length, devices, null, IntPtr.Zero, out int error);
		}

		~Context()
		{
			clReleaseContext(Pointer);
		}
	}

	class CommandQueue
	{
		public IntPtr Pointer { get; private set; }

		public CommandQueue(Context context, IntPtr device)
		{
			Pointer = clCreateCommandQueue(context.Pointer, device, 0, out int error);
		}

		~CommandQueue()
		{
			clReleaseCommandQueue(Pointer);
		}

		public void ReadBuffer<T>(Buffer buffer, T[] systemBuffer) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(systemBuffer, GCHandleType.Pinned);
			int size = Math.Min(buffer.SizeInBytes, Marshal.SizeOf(typeof(T)) * systemBuffer.Length);
			clEnqueueReadBuffer(Pointer, buffer.Pointer, true, 0, size, handle.AddrOfPinnedObject(), 0, IntPtr.Zero, IntPtr.Zero);
			handle.Free();
		}

		public void EnqueueRange(Kernel kernel, Vector globalWorkSize, Vector localWorkSize)
		{
			Vector offset = new Vector();
			clEnqueueNDRangeKernel(Pointer, kernel.Pointer, globalWorkSize.Dimension, ref offset, ref globalWorkSize, ref localWorkSize, 0, null, IntPtr.Zero);
		}
	}

	class Buffer
	{
		public IntPtr Pointer { get; private set; }
		public int SizeInBytes { get; private set; }

		private Buffer() { }

		~Buffer()
		{
			clReleaseMemObject(Pointer);
		}

		public static Buffer Create(Context context, int size)
		{
			Buffer buffer = new Buffer();
			buffer.SizeInBytes = size;
			buffer.Pointer = clCreateBuffer(context.Pointer, MemoryFlags.WriteOnly, buffer.SizeInBytes, IntPtr.Zero, out int errorCode);
			return buffer;
		}
	}

	class Program
	{
		public IntPtr Pointer { get; private set; }

		public Program(Context context, params string[] sources)
		{
			Pointer = clCreateProgramWithSource(context.Pointer, sources.Length, sources, null, out int errorCode);
		}

		~Program()
		{
			clReleaseProgram(Pointer);
		}

		public void Build(params IntPtr[] devices)
		{
			int error = clBuildProgram(Pointer, devices.Length, devices, null, null, IntPtr.Zero);

			if (error != 0)
			{
				int paramValueSize = 0;
				clGetProgramBuildInfo(Pointer, devices.First(), ProgramBuildInfoString.Log, 0, null, out paramValueSize);
				System.Text.StringBuilder text = new StringBuilder(paramValueSize);
				clGetProgramBuildInfo(Pointer, devices.First(), ProgramBuildInfoString.Log, paramValueSize, text, out paramValueSize);
				throw new Exception(text.ToString());
			}
		}
	}

	class Kernel
	{
		public IntPtr Pointer { get; private set; }

		public Kernel(Program program, string functionName)
		{
			Pointer = clCreateKernel(program.Pointer, functionName, out int errorCode);
		}

		~Kernel()
		{
			clReleaseKernel(Pointer);
		}

		public void SetArgument(int argumentIndex, Buffer buffer)
		{
			IntPtr bufferPointer = buffer.Pointer;
			clSetKernelArg(Pointer, argumentIndex, Marshal.SizeOf(typeof(IntPtr)), ref bufferPointer);
		}

		public void SetArgument<T>(int argumentIndex, T value) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
			clSetKernelArg(Pointer, argumentIndex, Marshal.SizeOf(typeof(T)), handle.AddrOfPinnedObject());
			handle.Free();
		}
	}

	enum DeviceType : long
	{
		Default = (1 << 0),
		Cpu = (1 << 1),
		Gpu = (1 << 2),
		Accelerator = (1 << 3),
		All = 0xFFFFFFFF
	}

	enum MemoryFlags : long
	{
		ReadWrite = (1 << 0),
		WriteOnly = (1 << 1),
		ReadOnly = (1 << 2),
		UseHostMemory = (1 << 3),
		HostAccessible = (1 << 4),
		CopyHostMemory = (1 << 5)
	}

	struct Vector
	{
		public int X;
		public int Y;
		public int Z;
		public int Dimension;

		public Vector(int x, int y, int z, int dim) {X = x; Y = y; Z = z; Dimension = dim;}
	}

	enum ProgramBuildInfoString
	{
		Options = 0x1182,
		Log = 0x1183
	}

	void Start()
	{
		clGetPlatformIDs(0, null, out int platformCount);
		IntPtr[] platforms = new IntPtr[platformCount];
		clGetPlatformIDs(platformCount, platforms, out platformCount);
		clGetDeviceIDs(platforms[0], DeviceType.Default, 0, null, out int deviceCount);
		IntPtr[] devices = new IntPtr[deviceCount];
		clGetDeviceIDs(platforms[0], DeviceType.Default, deviceCount, devices, out deviceCount);
		Context context = new Context(devices[0]);
		_CommandQueue = new CommandQueue(context, devices[0]);
		Program program = new Program(context, ComputeKernel);
		program.Build(devices[0]);
		_Bytes = new byte[Resolution * Resolution * 4];
		_Kernel = new Kernel(program, "mainImage");
		_Buffer = Buffer.Create(context, _Bytes.Length);
		_Kernel.SetArgument(0, _Buffer);
		_Texture = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false);
		_GlobalWorkSize = new Vector(Resolution * Resolution, 0, 0, 1);
		_LocalWorkSize = new Vector(16, 0, 0, 1);
	}

	void Update()
	{
		_Kernel.SetArgument(1, Time.time);
		_Kernel.SetArgument(2, Resolution);
		_CommandQueue.EnqueueRange(_Kernel, _GlobalWorkSize, _LocalWorkSize);
		_CommandQueue.ReadBuffer(_Buffer, _Bytes);
		_Texture.LoadRawTextureData(_Bytes);
		_Texture.Apply();
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0, 0, 512, 512), _Texture, ScaleMode.ScaleToFit, true);
	}

	void OnDestroy()
	{
		Destroy(_Texture);
	}
}