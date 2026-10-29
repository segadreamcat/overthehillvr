using System.Runtime.InteropServices;

namespace Valve.VR;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct VREvent_t_Packed(VREvent_t unpacked)
{
	public uint eventType = unpacked.eventType;

	public uint trackedDeviceIndex = unpacked.trackedDeviceIndex;

	public float eventAgeSeconds = unpacked.eventAgeSeconds;

	public VREvent_Data_t data = unpacked.data;

	public void Unpack(ref VREvent_t unpacked)
	{
		unpacked.eventType = eventType;
		unpacked.trackedDeviceIndex = trackedDeviceIndex;
		unpacked.eventAgeSeconds = eventAgeSeconds;
		unpacked.data = data;
	}
}
