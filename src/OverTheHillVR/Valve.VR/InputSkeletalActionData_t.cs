using System.Runtime.InteropServices;

namespace Valve.VR;

public struct InputSkeletalActionData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	public ulong activeOrigin;
}
