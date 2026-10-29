using UnityEngine;

namespace OverTheHillVR;

internal static class WheelPad
{
	private static ulong now;

	private static ulong prev;

	private static uint pov = 65535u;

	private static uint prevPov = 65535u;

	private static float holdStart = -1f;

	private static bool holdFired;

	public static ulong Buttons => now;

	public static ulong Pressed => now & ~prev;

	public static void Feed(ulong buttons, uint p)
	{
		prev = now;
		now = buttons;
		prevPov = pov;
		pov = p;
	}

	public static void Idle()
	{
		prev = now;
		now = 0uL;
		prevPov = pov;
		pov = 65535u;
	}

	private static bool Up(uint p)
	{
		if (p != 0 && p != 4500)
		{
			return p == 31500;
		}
		return true;
	}

	public static bool AnyDown()
	{
		return (now & ~prev) != 0;
	}

	public static bool PovUpDown()
	{
		if (Up(pov))
		{
			return !Up(prevPov);
		}
		return false;
	}

	public static bool PovUpHeld()
	{
		return Up(pov);
	}

	private static ulong Bit(int oneBased)
	{
		if (oneBased < 1 || oneBased > 64)
		{
			return 0uL;
		}
		return (ulong)(1L << oneBased - 1);
	}

	public static bool Down(int oneBased)
	{
		ulong num = Bit(oneBased);
		if (num != 0L && (now & num) != 0L)
		{
			return (prev & num) == 0;
		}
		return false;
	}

	public static bool Held(int oneBased)
	{
		ulong num = Bit(oneBased);
		if (num != 0L)
		{
			return (now & num) != 0;
		}
		return false;
	}

	public static bool HeldFor(int oneBased, float secs)
	{
		if (!Held(oneBased))
		{
			if (Bit(oneBased) != 0L && (prev & Bit(oneBased)) != 0L)
			{
				holdStart = -1f;
				holdFired = false;
			}
			return false;
		}
		if (holdStart < 0f)
		{
			holdStart = Time.unscaledTime;
		}
		if (!holdFired && Time.unscaledTime - holdStart >= secs)
		{
			holdFired = true;
			return true;
		}
		return false;
	}
}
