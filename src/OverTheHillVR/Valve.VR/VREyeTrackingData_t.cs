using System.Runtime.InteropServices;

namespace Valve.VR;

public struct VREyeTrackingData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	[MarshalAs(UnmanagedType.I1)]
	public bool bValid;

	[MarshalAs(UnmanagedType.I1)]
	public bool bTracked;

	public HmdVector3_t vGazeOrigin;

	public HmdVector3_t vGazeTarget;
}
