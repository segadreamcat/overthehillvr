using System.Runtime.InteropServices;

namespace Valve.VR;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct VRControllerState_t_Packed(VRControllerState_t unpacked)
{
	public uint unPacketNum = unpacked.unPacketNum;

	public ulong ulButtonPressed = unpacked.ulButtonPressed;

	public ulong ulButtonTouched = unpacked.ulButtonTouched;

	public VRControllerAxis_t rAxis0 = unpacked.rAxis0;

	public VRControllerAxis_t rAxis1 = unpacked.rAxis1;

	public VRControllerAxis_t rAxis2 = unpacked.rAxis2;

	public VRControllerAxis_t rAxis3 = unpacked.rAxis3;

	public VRControllerAxis_t rAxis4 = unpacked.rAxis4;

	public void Unpack(ref VRControllerState_t unpacked)
	{
		unpacked.unPacketNum = unPacketNum;
		unpacked.ulButtonPressed = ulButtonPressed;
		unpacked.ulButtonTouched = ulButtonTouched;
		unpacked.rAxis0 = rAxis0;
		unpacked.rAxis1 = rAxis1;
		unpacked.rAxis2 = rAxis2;
		unpacked.rAxis3 = rAxis3;
		unpacked.rAxis4 = rAxis4;
	}
}
