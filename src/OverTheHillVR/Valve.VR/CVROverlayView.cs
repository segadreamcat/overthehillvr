using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVROverlayView
{
	private IVROverlayView FnTable;

	internal CVROverlayView(IntPtr pInterface)
	{
		FnTable = (IVROverlayView)Marshal.PtrToStructure(pInterface, typeof(IVROverlayView));
	}

	public EVROverlayError AcquireOverlayView(ulong ulOverlayHandle, ref VRNativeDevice_t pNativeDevice, ref VROverlayView_t pOverlayView, uint unOverlayViewSize)
	{
		return FnTable.AcquireOverlayView(ulOverlayHandle, ref pNativeDevice, ref pOverlayView, unOverlayViewSize);
	}

	public EVROverlayError ReleaseOverlayView(ref VROverlayView_t pOverlayView)
	{
		return FnTable.ReleaseOverlayView(ref pOverlayView);
	}

	public void PostOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pvrEvent)
	{
		FnTable.PostOverlayEvent(ulOverlayHandle, ref pvrEvent);
	}

	public bool IsViewingPermitted(ulong ulOverlayHandle)
	{
		return FnTable.IsViewingPermitted(ulOverlayHandle);
	}
}
