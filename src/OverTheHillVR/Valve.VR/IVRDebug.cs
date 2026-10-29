using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRDebug
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRDebugError _EmitVrProfilerEvent(IntPtr pchMessage);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRDebugError _BeginVrProfilerEvent(ref ulong pHandleOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRDebugError _FinishVrProfilerEvent(ulong hHandle, IntPtr pchMessage);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _DriverDebugRequest(uint unDeviceIndex, IntPtr pchRequest, StringBuilder pchResponseBuffer, uint unResponseBufferSize);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _EmitVrProfilerEvent EmitVrProfilerEvent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _BeginVrProfilerEvent BeginVrProfilerEvent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _FinishVrProfilerEvent FinishVrProfilerEvent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _DriverDebugRequest DriverDebugRequest;
}
