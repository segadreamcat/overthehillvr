using System;

namespace Valve.VR;

public struct VRTextureWithDepth_t
{
	public IntPtr handle;

	public ETextureType eType;

	public EColorSpace eColorSpace;

	public VRTextureDepthInfo_t depth;
}
