using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OverTheHillVR;

internal static class Pad
{
	private struct State
	{
		public uint packet;

		public ushort buttons;

		public byte lt;

		public byte rt;

		public short lx;

		public short ly;

		public short rx;

		public short ry;
	}

	public const ushort Up = 1;

	public const ushort Down = 2;

	public const ushort Left = 4;

	public const ushort Right = 8;

	public const ushort Back = 32;

	public const ushort L3 = 64;

	public const ushort R3 = 128;

	public const ushort LB = 256;

	public const ushort RB = 512;

	public const ushort A = 4096;

	public const ushort B = 8192;

	public const ushort X = 16384;

	public const ushort Y = 32768;

	private static ushort now;

	private static ushort prev;

	private static bool failed;

	private static float nextScan;

	private static int pad = -1;

	private static float heldSince = -1f;

	private static bool heldFired;

	[DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
	private static extern uint GetState(uint idx, out State s);

	public static void Poll()
	{
		prev = now;
		now = 0;
		if (failed || !Mod.ControllerCombos.Value)
		{
			return;
		}
		try
		{
			if (pad >= 0 && GetState((uint)pad, out var s) == 0)
			{
				now |= s.buttons;
				return;
			}
			pad = -1;
			if (Time.unscaledTime < nextScan)
			{
				return;
			}
			nextScan = Time.unscaledTime + 2f;
			for (uint num = 0u; num < 4; num++)
			{
				if (GetState(num, out var s2) == 0)
				{
					pad = (int)num;
					now |= s2.buttons;
					Mod.L.Msg($"Xbox controller found (slot {num}).");
					break;
				}
			}
		}
		catch (Exception ex)
		{
			failed = true;
			Mod.L.Warning("XInput unavailable: " + ex.Message);
		}
	}

	private static bool H(ushort b, ushort st)
	{
		return (st & b) == b;
	}

	public static bool HeldFor(int b, float secs)
	{
		if (!H((ushort)b, now))
		{
			heldSince = -1f;
			heldFired = false;
			return false;
		}
		if (heldSince < 0f)
		{
			heldSince = Time.unscaledTime;
		}
		if (!heldFired && Time.unscaledTime - heldSince >= secs)
		{
			heldFired = true;
			return true;
		}
		return false;
	}

	public static bool AnyDown()
	{
		if (now != 0)
		{
			return prev == 0;
		}
		return false;
	}

	public static bool DpadUpDown()
	{
		if ((now & 1) != 0 && (prev & 1) == 0)
		{
			return (now & 0x20) == 0;
		}
		return false;
	}

	public static bool DpadUpHeld()
	{
		if ((now & 1) != 0)
		{
			return (now & 0x20) == 0;
		}
		return false;
	}

	public static bool ComboHeld(ushort b)
	{
		return H((ushort)(0x20 | b), now);
	}

	public static bool ComboDown(ushort b)
	{
		if (ComboHeld(b))
		{
			return !H((ushort)(0x20 | b), prev);
		}
		return false;
	}

	public static bool RecenterDown()
	{
		if (!H(192, now) || H(192, prev))
		{
			return ComboDown(4096);
		}
		return true;
	}
}
