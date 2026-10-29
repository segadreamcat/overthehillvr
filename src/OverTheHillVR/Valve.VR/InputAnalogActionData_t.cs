using System.Runtime.InteropServices;

namespace Valve.VR;

public struct InputAnalogActionData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	public ulong activeOrigin;

	public float x;

	public float y;

	public float z;

	public float deltaX;

	public float deltaY;

	public float deltaZ;

	public float fUpdateTime;
}
