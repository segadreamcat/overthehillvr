using System.Runtime.InteropServices;

namespace Valve.VR;

public struct VREvent_AudioMuteControl_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bMute;
}
