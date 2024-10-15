/*
Script created by Przemyslaw Zaworski, October 2024.
Example of mounting of an additional disk in Unity Engine.
Inspired by game "The USB Stick Found in the Grass".
Create build with this script. 
If you haven't got any vhd file:
- Run build as administrator.
- Press P to create virtual disk file (for example D:\virtualdisk.vhd, see _FileName and _FileSize). Exit build.
- Right click on vhd file, then Install. Then go to Disk Management (keys Win + R, then diskmgmt.msc).
- Format, configure virtual disk, then detach. 
If you have got vhd file:
- Run build as administrator, press O. It will open and attach disk.
- When you close build, disk is automatically detached.
*/
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using UnityEngine;

public class VirtualDisk : MonoBehaviour
{
	[DllImport("virtdisk.dll", CharSet = CharSet.Unicode)]
	public static extern Int32 AttachVirtualDisk(VirtualDiskSafeHandle handle, IntPtr descriptor, ATTACH_VIRTUAL_DISK_FLAG flags, Int32 specificFlags, ref ATTACH_VIRTUAL_DISK_PARAMETERS parameters, IntPtr overlapped);

	[DllImport("virtdisk.dll", CharSet = CharSet.Unicode)]
	public static extern Int32 CreateVirtualDisk(ref VIRTUAL_STORAGE_TYPE storageType, String path, VIRTUAL_DISK_ACCESS_MASK accessMask, IntPtr descriptor, CREATE_VIRTUAL_DISK_FLAG flags, Int32 specificFlags, ref CREATE_VIRTUAL_DISK_PARAMETERS parameters, IntPtr overlapped, ref VirtualDiskSafeHandle handle);

	[DllImport("virtdisk.dll", CharSet = CharSet.Unicode)]
	public static extern Int32 OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE storageType, String path, VIRTUAL_DISK_ACCESS_MASK accessMask, OPEN_VIRTUAL_DISK_FLAG flags, ref OPEN_VIRTUAL_DISK_PARAMETERS parameters, ref VirtualDiskSafeHandle handle);

	[DllImportAttribute("kernel32.dll", SetLastError = true)] [return: MarshalAsAttribute(UnmanagedType.Bool)]
	public static extern Boolean CloseHandle(IntPtr hObject);

	string _FileName  = @"D:\virtualdisk.vhd";
	int _FileSize = 128 * 1024 * 1024; // 128 MB
	VirtualDiskSafeHandle _OpenAttachHandle = new VirtualDiskSafeHandle();	

	const int CREATE_VIRTUAL_DISK_PARAMETERS_DEFAULT_SECTOR_SIZE = 0x200;
	const int OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT = 1;
	const int VIRTUAL_STORAGE_TYPE_DEVICE_UNKNOWN = 0;
	const int VIRTUAL_STORAGE_TYPE_DEVICE_ISO = 1;
	const int VIRTUAL_STORAGE_TYPE_DEVICE_VHD = 2;
	const int VIRTUAL_STORAGE_TYPE_DEVICE_VHDX = 3;
	static readonly Guid VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT = new Guid("EC984AEC-A0F9-47e9-901F-71415A66345B");
	const Int32 ERROR_SUCCESS = 0;

	public enum ATTACH_VIRTUAL_DISK_FLAG : int 
	{
		ATTACH_VIRTUAL_DISK_FLAG_NONE = 0x00000000,
		ATTACH_VIRTUAL_DISK_FLAG_READ_ONLY = 0x00000001,
		ATTACH_VIRTUAL_DISK_FLAG_NO_DRIVE_LETTER = 0x00000002,
		ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME = 0x00000004,
		ATTACH_VIRTUAL_DISK_FLAG_NO_LOCAL_HOST = 0x00000008
	}

	public enum ATTACH_VIRTUAL_DISK_VERSION : int 
	{
		ATTACH_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
		ATTACH_VIRTUAL_DISK_VERSION_1 = 1
	}

	public enum CREATE_VIRTUAL_DISK_FLAG : int 
	{
		CREATE_VIRTUAL_DISK_FLAG_NONE = 0x00000000,
		CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION = 0x00000001
	}

	public enum CREATE_VIRTUAL_DISK_VERSION : int 
	{
		CREATE_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
		CREATE_VIRTUAL_DISK_VERSION_1 = 1
	}

	public enum OPEN_VIRTUAL_DISK_FLAG : int 
	{
		OPEN_VIRTUAL_DISK_FLAG_NONE = 0x00000000,
		OPEN_VIRTUAL_DISK_FLAG_NO_PARENTS = 0x00000001,
		OPEN_VIRTUAL_DISK_FLAG_BLANK_FILE = 0x00000002,
		OPEN_VIRTUAL_DISK_FLAG_BOOT_DRIVE = 0x00000004
	}

	public enum OPEN_VIRTUAL_DISK_VERSION : int 
	{
		OPEN_VIRTUAL_DISK_VERSION_UNSPECIFIED = 0,
		OPEN_VIRTUAL_DISK_VERSION_1 = 1
	}

	public enum VIRTUAL_DISK_ACCESS_MASK : int 
	{
		VIRTUAL_DISK_ACCESS_ATTACH_RO = 0x00010000,
		VIRTUAL_DISK_ACCESS_ATTACH_RW = 0x00020000,
		VIRTUAL_DISK_ACCESS_DETACH = 0x00040000,
		VIRTUAL_DISK_ACCESS_GET_INFO = 0x00080000,
		VIRTUAL_DISK_ACCESS_CREATE = 0x00100000,
		VIRTUAL_DISK_ACCESS_METAOPS = 0x00200000,
		VIRTUAL_DISK_ACCESS_READ = 0x000d0000,
		VIRTUAL_DISK_ACCESS_ALL = 0x003f0000,
		VIRTUAL_DISK_ACCESS_WRITABLE = 0x00320000
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ATTACH_VIRTUAL_DISK_PARAMETERS 
	{
		public ATTACH_VIRTUAL_DISK_VERSION Version;
		public ATTACH_VIRTUAL_DISK_PARAMETERS_Version1 Version1;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ATTACH_VIRTUAL_DISK_PARAMETERS_Version1 
	{
		public Int32 Reserved;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CREATE_VIRTUAL_DISK_PARAMETERS 
	{
		public CREATE_VIRTUAL_DISK_VERSION Version;
		public CREATE_VIRTUAL_DISK_PARAMETERS_Version1 Version1;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CREATE_VIRTUAL_DISK_PARAMETERS_Version1 
	{
		public Guid UniqueId;
		public Int64 MaximumSize;
		public Int32 BlockSizeInBytes; 
		public Int32 SectorSizeInBytes; 
		public IntPtr ParentPath; 
		public IntPtr SourcePath;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct OPEN_VIRTUAL_DISK_PARAMETERS 
	{
		public OPEN_VIRTUAL_DISK_VERSION Version;
		public OPEN_VIRTUAL_DISK_PARAMETERS_Version1 Version1;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct OPEN_VIRTUAL_DISK_PARAMETERS_Version1 
	{
		public Int32 RWDepth;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VIRTUAL_STORAGE_TYPE 
	{
		public Int32 DeviceId; 
		public Guid VendorId; 
	}

	public class VirtualDiskSafeHandle : SafeHandle 
	{
		public VirtualDiskSafeHandle() : base(IntPtr.Zero, true) { }

		public override bool IsInvalid 
		{
			get { return (this.IsClosed) || (base.handle == IntPtr.Zero); }
		}

		protected override bool ReleaseHandle() 
		{
			return CloseHandle(this.handle);
		}

		public void DestroyHandle()
		{
			ReleaseHandle();
		}
	}

	void Create(int deviceType, long size, CREATE_VIRTUAL_DISK_FLAG options, int blockSize, int sectorSize) 
	{
		VirtualDiskSafeHandle createHandle = new VirtualDiskSafeHandle();
		CREATE_VIRTUAL_DISK_PARAMETERS parameters = new CREATE_VIRTUAL_DISK_PARAMETERS();
		parameters.Version = CREATE_VIRTUAL_DISK_VERSION.CREATE_VIRTUAL_DISK_VERSION_1;
		parameters.Version1.BlockSizeInBytes = (blockSize == 0) ? 0 : blockSize;
		parameters.Version1.MaximumSize = size;
		parameters.Version1.ParentPath = IntPtr.Zero;
		parameters.Version1.SectorSizeInBytes = (sectorSize == 0) ? CREATE_VIRTUAL_DISK_PARAMETERS_DEFAULT_SECTOR_SIZE : sectorSize;
		parameters.Version1.SourcePath = IntPtr.Zero;
		parameters.Version1.UniqueId = Guid.Empty;
		VIRTUAL_STORAGE_TYPE storageType = new VIRTUAL_STORAGE_TYPE();
		storageType.DeviceId = deviceType;
		storageType.VendorId = VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;
		VIRTUAL_DISK_ACCESS_MASK accessMask = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL;
		int result = CreateVirtualDisk(ref storageType, this._FileName, accessMask, IntPtr.Zero, options, 0, ref parameters, IntPtr.Zero, ref createHandle);          
		if (result == ERROR_SUCCESS)
		{
			Debug.Log("Disk has been successfully created.");
		}
		else 
		{
			throw new Win32Exception(result);
		}
		createHandle.DestroyHandle();
	}

	void Open(VIRTUAL_DISK_ACCESS_MASK fileAccess, int deviceType) 
	{
		OPEN_VIRTUAL_DISK_PARAMETERS parameters = new OPEN_VIRTUAL_DISK_PARAMETERS();
		parameters.Version = OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1;
		parameters.Version1.RWDepth = OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;
		VIRTUAL_STORAGE_TYPE storageType = new VIRTUAL_STORAGE_TYPE();
		storageType.DeviceId = deviceType;
		if (deviceType == VIRTUAL_STORAGE_TYPE_DEVICE_ISO)
		{
			VIRTUAL_DISK_ACCESS_MASK getInfo = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_GET_INFO;
			fileAccess = ((fileAccess & getInfo) == getInfo) ? getInfo : 0;
			fileAccess |= VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ATTACH_RO;
		}
		storageType.VendorId = VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;
		OPEN_VIRTUAL_DISK_FLAG flag = OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE;
		int result = OpenVirtualDisk(ref storageType, this._FileName, fileAccess, flag, ref parameters, ref _OpenAttachHandle);
		if (result == ERROR_SUCCESS) 
		{
			Debug.Log("Disk has been successfully opened.");
		} 
		else 
		{
			_OpenAttachHandle.SetHandleAsInvalid();
			throw new Win32Exception(result);
		}
	}

	void Attach(ATTACH_VIRTUAL_DISK_FLAG options, int deviceType) 
	{
		if (deviceType == VIRTUAL_STORAGE_TYPE_DEVICE_ISO) { options |= ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_READ_ONLY; }
		ATTACH_VIRTUAL_DISK_PARAMETERS parameters = new ATTACH_VIRTUAL_DISK_PARAMETERS();
		parameters.Version = ATTACH_VIRTUAL_DISK_VERSION.ATTACH_VIRTUAL_DISK_VERSION_1;
		int result = AttachVirtualDisk(_OpenAttachHandle, IntPtr.Zero, options, 0, ref parameters, IntPtr.Zero);
		if (result == ERROR_SUCCESS) 
		{
			Debug.Log("Disk has been successfully attached.");
		}
		else 
		{
			throw new Win32Exception(result);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Create(VIRTUAL_STORAGE_TYPE_DEVICE_VHD, _FileSize, CREATE_VIRTUAL_DISK_FLAG.CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION, 0, 0);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			Open(VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL, VIRTUAL_STORAGE_TYPE_DEVICE_VHD);
			Attach(ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_NONE, VIRTUAL_STORAGE_TYPE_DEVICE_VHD);
		}
	}
}