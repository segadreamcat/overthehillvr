using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVRPaths
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ETrackedPropertyError _ReadPathBatch(ulong ulRootHandle, ref PathRead_t pBatch, uint unBatchEntryCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ETrackedPropertyError _WritePathBatch(ulong ulRootHandle, ref PathWrite_t pBatch, uint unBatchEntryCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ETrackedPropertyError _StringToHandle(ref ulong pHandle, IntPtr pchPath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ETrackedPropertyError _HandleToString(ulong pHandle, string pchBuffer, uint unBufferSize, ref uint punBufferSizeUsed);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReadPathBatch ReadPathBatch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _WritePathBatch WritePathBatch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _StringToHandle StringToHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _HandleToString HandleToString;
}
