using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRDebug
{
	private IVRDebug FnTable;

	internal CVRDebug(IntPtr pInterface)
	{
		FnTable = (IVRDebug)Marshal.PtrToStructure(pInterface, typeof(IVRDebug));
	}

	public EVRDebugError EmitVrProfilerEvent(string pchMessage)
	{
		IntPtr intPtr = Utils.ToUtf8(pchMessage);
		EVRDebugError result = FnTable.EmitVrProfilerEvent(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRDebugError BeginVrProfilerEvent(ref ulong pHandleOut)
	{
		pHandleOut = 0uL;
		return FnTable.BeginVrProfilerEvent(ref pHandleOut);
	}

	public EVRDebugError FinishVrProfilerEvent(ulong hHandle, string pchMessage)
	{
		IntPtr intPtr = Utils.ToUtf8(pchMessage);
		EVRDebugError result = FnTable.FinishVrProfilerEvent(hHandle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint DriverDebugRequest(uint unDeviceIndex, string pchRequest, StringBuilder pchResponseBuffer, uint unResponseBufferSize)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRequest);
		uint result = FnTable.DriverDebugRequest(unDeviceIndex, intPtr, pchResponseBuffer, unResponseBufferSize);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}
}
