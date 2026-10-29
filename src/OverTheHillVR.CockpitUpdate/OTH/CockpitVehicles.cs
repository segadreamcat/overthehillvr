using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace OTH;

internal static class CockpitVehicles
{
	[ThreadStatic]
	private static bool bonnetProbe;

	internal static void Initialize(HarmonyLib.Harmony harmony)
	{
		if (CockpitUpdate.DriverType.GetMethod("UsesProtectorBonnetPoint", BindingFlags.Instance | BindingFlags.NonPublic) != null)
		{
			CockpitUpdate.Self.Log("Universal cockpit is built into core 0.3.2: every vehicle uses the shared cabin and driver view.");
			return;
		}
		CockpitUpdate.Self.Warn("Older core detected. Install the matching OverTheHillVR.dll for permanent all-vehicle cockpit support.");
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("CockpitAllowed", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CockpitVehicles).GetMethod("Allowed", BindingFlags.Static | BindingFlags.NonPublic)));
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("BonnetPoint", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CockpitVehicles).GetMethod("BonnetPrefix", BindingFlags.Static | BindingFlags.NonPublic)), null, null, new HarmonyMethod(typeof(CockpitVehicles).GetMethod("BonnetFinalizer", BindingFlags.Static | BindingFlags.NonPublic)), null);
		CockpitUpdate.Self.Log("Shared cockpit and dashboard enabled for every vehicle and paint job.");
	}

	private static bool Allowed(ref bool __result)
	{
		if (bonnetProbe)
		{
			return true;
		}
		__result = (Object)(object)CockpitUpdate.Read<Transform>("body") != (Object)null;
		return false;
	}

	private static void BonnetPrefix(out bool __state)
	{
		__state = bonnetProbe;
		bonnetProbe = true;
	}

	private static Exception BonnetFinalizer(Exception __exception, bool __state)
	{
		bonnetProbe = __state;
		return __exception;
	}
}
