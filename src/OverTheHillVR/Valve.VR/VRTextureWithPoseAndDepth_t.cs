using System;

namespace Valve.VR;

public struct VRTextureWithPoseAndDepth_t
{
	public IntPtr handle;

	public ETextureType eType;

	public EColorSpace eColorSpace;

	public HmdMatrix34_t mDeviceToAbsoluteTracking;

	public VRTextureDepthInfo_t depth;
}
