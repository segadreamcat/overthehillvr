using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVRHeadsetView
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetHeadsetViewSize(uint nWidth, uint nHeight);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _GetHeadsetViewSize(ref uint pnWidth, ref uint pnHeight);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetHeadsetViewMode(uint eHeadsetViewMode);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetHeadsetViewMode();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetHeadsetViewCropped([MarshalAs(UnmanagedType.I1)] bool bCropped);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetHeadsetViewCropped();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate float _GetHeadsetViewAspectRatio();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetHeadsetViewBlendRange(float flStartPct, float flEndPct);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _GetHeadsetViewBlendRange(ref float pStartPct, ref float pEndPct);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetHeadsetViewSize SetHeadsetViewSize;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetHeadsetViewSize GetHeadsetViewSize;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetHeadsetViewMode SetHeadsetViewMode;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetHeadsetViewMode GetHeadsetViewMode;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetHeadsetViewCropped SetHeadsetViewCropped;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetHeadsetViewCropped GetHeadsetViewCropped;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetHeadsetViewAspectRatio GetHeadsetViewAspectRatio;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetHeadsetViewBlendRange SetHeadsetViewBlendRange;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetHeadsetViewBlendRange GetHeadsetViewBlendRange;
}
