using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public struct PathWrite_t
{
	public ulong ulPath;

	public EPropertyWriteType writeType;

	public ETrackedPropertyError eSetError;

	public IntPtr pvBuffer;

	public uint unBufferSize;

	public uint unTag;

	public ETrackedPropertyError eError;

	public IntPtr pszPath;

	[MarshalAs(UnmanagedType.I1)]
	public bool bPostEvents;

	[MarshalAs(UnmanagedType.I1)]
	public bool bValueChanged;
}
