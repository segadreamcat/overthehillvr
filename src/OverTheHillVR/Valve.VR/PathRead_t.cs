using System;

namespace Valve.VR;

public struct PathRead_t
{
	public ulong ulPath;

	public IntPtr pvBuffer;

	public uint unBufferSize;

	public uint unTag;

	public uint unRequiredBufferSize;

	public ETrackedPropertyError eError;

	public IntPtr pszPath;
}
