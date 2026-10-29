using System;

namespace Valve.VR;

public struct PropertyRead_t
{
	public ETrackedDeviceProperty prop;

	public IntPtr pvBuffer;

	public uint unBufferSize;

	public uint unTag;

	public uint unRequiredBufferSize;

	public ETrackedPropertyError eError;
}
