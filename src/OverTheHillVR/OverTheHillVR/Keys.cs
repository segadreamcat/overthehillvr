using System;
using System.Runtime.InteropServices;

namespace OverTheHillVR;

internal static class Keys
{
	public const int F3 = 114;

	public const int F4 = 115;

	public const int F5 = 116;

	public const int F12 = 123;

	public const int F11 = 122;

	public const int F10 = 121;

	public const int F9 = 120;

	public const int F6 = 117;

	public const int F7 = 118;

	public const int F8 = 119;

	public const int PageUp = 33;

	public const int PageDown = 34;

	public const int Home = 36;

	public const int End = 35;

	public const int Insert = 45;

	public const int Delete = 46;

	private static readonly int[] watched = new int[16]
	{
		114, 115, 116, 123, 122, 121, 120, 117, 118, 119,
		33, 34, 36, 35, 45, 46
	};

	private static readonly bool[] now = new bool[256];

	private static readonly bool[] prev = new bool[256];

	private static readonly uint myPid = (uint)Environment.ProcessId;

	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vk);

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pid);

	public static void Poll()
	{
		bool flag = false;
		try
		{
			GetWindowThreadProcessId(GetForegroundWindow(), out var pid);
			flag = pid == myPid;
		}
		catch
		{
		}
		int[] array = watched;
		foreach (int num in array)
		{
			prev[num] = now[num];
			now[num] = flag && (GetAsyncKeyState(num) & 0x8000) != 0;
		}
	}

	public static bool Down(int k)
	{
		if (now[k])
		{
			return !prev[k];
		}
		return false;
	}

	public static bool Held(int k)
	{
		return now[k];
	}
}
