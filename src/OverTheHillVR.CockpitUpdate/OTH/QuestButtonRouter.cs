using System;

namespace OTH;

public sealed class QuestButtonRouter
{
	public const int Recenter = 1;

	public const int ParkingBrake = 2;

	public const int Pause = 4;

	public const float PauseHoldSeconds = 1f;

	private bool previousRight;

	private bool previousBoth;

	private bool chord;

	private bool pauseFired;

	private float leftHeld;

	public int Update(bool left, bool right, float delta = 0f)
	{
		bool flag = left & right;
		if (flag)
		{
			chord = true;
		}
		int num = ((flag && !previousBoth && !pauseFired) ? 1 : 0);
		if (left && !right && !chord && !pauseFired)
		{
			if (float.IsFinite(delta) && delta > 0f)
			{
				leftHeld += Math.Min(delta, 0.1f);
			}
			if (leftHeld >= 1f)
			{
				pauseFired = true;
				num |= 4;
			}
		}
		if (!left)
		{
			leftHeld = 0f;
		}
		if (previousRight && !right && !chord && !pauseFired)
		{
			num |= 2;
		}
		previousRight = right;
		previousBoth = flag;
		if (!left && !right)
		{
			chord = false;
			pauseFired = false;
		}
		return num;
	}

	public void Reset()
	{
		previousRight = (previousBoth = (chord = (pauseFired = false)));
		leftHeld = 0f;
	}
}
