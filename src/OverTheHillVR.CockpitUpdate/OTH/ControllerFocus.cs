using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OTH;

internal sealed class ControllerFocus
{
	private delegate bool EnumProc(IntPtr window, IntPtr data);

	private static IntPtr unityWindow;

	private MethodInfo focus;

	private float nextCheck;

	private bool sawMenu;

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumProc callback, IntPtr data);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr window, out uint process);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int GetClassName(IntPtr window, StringBuilder text, int length);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr window);

	[DllImport("user32.dll")]
	private static extern bool IsWindow(IntPtr window);

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	internal void Initialize(HarmonyLib.Harmony harmony)
	{
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("GameWindow", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(ControllerFocus).GetMethod("WindowPrefix", BindingFlags.Static | BindingFlags.NonPublic)));
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("GameFocused", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(ControllerFocus).GetMethod("FocusedPrefix", BindingFlags.Static | BindingFlags.NonPublic)));
		focus = CockpitUpdate.DriverType.GetMethod("FocusGame", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	internal static IntPtr Window()
	{
		if (unityWindow != IntPtr.Zero && IsWindow(unityWindow))
		{
			return unityWindow;
		}
		EnumWindows((IntPtr window, IntPtr _) =>
		{
			GetWindowThreadProcessId(window, out var process);
			if (process != (uint)Environment.ProcessId || !IsWindowVisible(window))
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder(128);
			GetClassName(window, stringBuilder, stringBuilder.Capacity);
			if (stringBuilder.ToString() != "UnityWndClass")
			{
				return true;
			}
			unityWindow = window;
			CockpitUpdate.Self.Log("Controller focus target verified: UnityWndClass (game window).");
			return false;
		}, IntPtr.Zero);
		return unityWindow;
	}

	private static bool WindowPrefix(ref IntPtr __result)
	{
		__result = Window();
		return false;
	}

	private static bool FocusedPrefix(ref bool __result)
	{
		IntPtr intPtr = Window();
		__result = intPtr != IntPtr.Zero && GetForegroundWindow() == intPtr;
		return false;
	}

	internal void Update()
	{
		if (sawMenu || Time.unscaledTime < nextCheck || CockpitUpdate.Driver == null)
		{
			return;
		}
		nextCheck = Time.unscaledTime + 1f;
		if (!QuestControls.PhysicalGamepadConnected())
		{
			return;
		}
		foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<ContextUIMainMenu>()))
		{
			ContextUIMainMenu val = ((Il2CppObjectBase)(object)item).TryCast<ContextUIMainMenu>();
			if ((Object)(object)val == (Object)null || !((Behaviour)val).isActiveAndEnabled)
			{
				continue;
			}
			focus.Invoke(CockpitUpdate.Driver, new object[1] { "Xbox main menu ready" });
			EventSystem current = EventSystem.current;
			if ((Object)(object)current != (Object)null && (Object)(object)current.currentSelectedGameObject == (Object)null)
			{
				foreach (Button componentsInChild in ((Component)val).GetComponentsInChildren<Button>(false))
				{
					if (((Behaviour)componentsInChild).isActiveAndEnabled && ((Selectable)componentsInChild).interactable)
					{
						current.SetSelectedGameObject(((Component)componentsInChild).gameObject);
						break;
					}
				}
			}
			sawMenu = true;
			CockpitUpdate.Self.Log("Xbox main-menu startup focus and initial selection applied.");
			break;
		}
	}
}
