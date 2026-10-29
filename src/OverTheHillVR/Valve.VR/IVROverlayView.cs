using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVROverlayView
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _AcquireOverlayView(ulong ulOverlayHandle, ref VRNativeDevice_t pNativeDevice, ref VROverlayView_t pOverlayView, uint unOverlayViewSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ReleaseOverlayView(ref VROverlayView_t pOverlayView);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _PostOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pvrEvent);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsViewingPermitted(ulong ulOverlayHandle);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _AcquireOverlayView AcquireOverlayView;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReleaseOverlayView ReleaseOverlayView;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _PostOverlayEvent PostOverlayEvent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsViewingPermitted IsViewingPermitted;
}
