using System.Runtime.InteropServices;

namespace Valve.VR;

public struct PathWriteOptions_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bPostEvents;
}
