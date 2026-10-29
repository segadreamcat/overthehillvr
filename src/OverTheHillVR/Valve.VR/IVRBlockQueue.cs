using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVRBlockQueue
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _Create(ref ulong pulQueueHandle, IntPtr pchPath, uint unBlockDataSize, uint unBlockHeaderSize, uint unBlockCount, uint unFlags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _Connect(ref ulong pulQueueHandle, IntPtr pchPath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _Destroy(ulong ulQueueHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _AcquireWriteOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _ReleaseWriteOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _WaitAndAcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType, uint unTimeoutMs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _AcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _ReleaseReadOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EBlockQueueError _QueueHasReader(ulong ulQueueHandle, [MarshalAs(UnmanagedType.I1)] ref bool pbHasReaders);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Create Create;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Connect Connect;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Destroy Destroy;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _AcquireWriteOnlyBlock AcquireWriteOnlyBlock;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReleaseWriteOnlyBlock ReleaseWriteOnlyBlock;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _WaitAndAcquireReadOnlyBlock WaitAndAcquireReadOnlyBlock;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _AcquireReadOnlyBlock AcquireReadOnlyBlock;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReleaseReadOnlyBlock ReleaseReadOnlyBlock;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _QueueHasReader QueueHasReader;
}
