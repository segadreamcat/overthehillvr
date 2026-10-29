using System;

namespace Valve.VR;

public struct VRTextureMotionInfo_t
{
	public IntPtr handle;

	public HmdMatrix44_t mDeltaPose;
}
