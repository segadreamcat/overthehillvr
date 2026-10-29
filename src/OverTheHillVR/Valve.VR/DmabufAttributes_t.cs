using System;

namespace Valve.VR;

public struct DmabufAttributes_t
{
	public IntPtr pNext;

	public uint unWidth;

	public uint unHeight;

	public uint unDepth;

	public uint unMipLevels;

	public uint unArrayLayers;

	public uint unSampleCount;

	public uint unFormat;

	public ulong ulModifier;

	public uint unPlaneCount;

	public DmabufPlane_t plane0;

	public DmabufPlane_t plane1;

	public DmabufPlane_t plane2;

	public DmabufPlane_t plane3;
}
