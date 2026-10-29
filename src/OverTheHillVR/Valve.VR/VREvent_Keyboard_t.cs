namespace Valve.VR;

public struct VREvent_Keyboard_t
{
	public byte cNewInput0;

	public byte cNewInput1;

	public byte cNewInput2;

	public byte cNewInput3;

	public byte cNewInput4;

	public byte cNewInput5;

	public byte cNewInput6;

	public byte cNewInput7;

	public ulong uUserValue;

	public ulong overlayHandle;

	public string cNewInput => new string(new char[8]
	{
		(char)cNewInput0,
		(char)cNewInput1,
		(char)cNewInput2,
		(char)cNewInput3,
		(char)cNewInput4,
		(char)cNewInput5,
		(char)cNewInput6,
		(char)cNewInput7
	}).TrimEnd('\0');
}
