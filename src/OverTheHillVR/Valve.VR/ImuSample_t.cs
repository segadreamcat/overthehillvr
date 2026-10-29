namespace Valve.VR;

public struct ImuSample_t
{
	public double fSampleTime;

	public HmdVector3d_t vAccel;

	public HmdVector3d_t vGyro;

	public uint unOffScaleFlags;
}
