using System;
using System.Reflection;
using HarmonyLib;
using Il2CppDirectInputManager;
using Il2CppFusion;
using MelonLoader;
using UnityEngine;

namespace OTH;

internal static class WheelInputFix
{
	internal static MelonPreferences_Entry<float> HardwareDegrees;

	internal static MelonPreferences_Entry<float> DrivingDegrees;

	private static FieldInfo invert;

	private static readonly bool[] pedalReady = new bool[2];

	private static float sampledAt = -100f;

	private static float physicalSteering;

	private static bool haveSteering;

	internal static bool WheelOwnsControls
	{
		get
		{
			if (Application.isFocused)
			{
				return Time.unscaledTime - sampledAt < 0.5f;
			}
			return false;
		}
	}

	internal static void Initialize(MelonPreferences_Category prefs, HarmonyLib.Harmony harmony)
	{
		HardwareDegrees = prefs.CreateEntry("WheelHardwareDegrees", 900f, "Match the G HUB operating range (total degrees)");
		DrivingDegrees = prefs.CreateEntry("WheelDrivingDegrees", 540f, "Physical steering rotation for full road-wheel lock (total degrees)");
		BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		invert = CockpitUpdate.DriverType.Assembly.GetType("OverTheHillVR.Mod").GetField("WheelPedalsInverted", bindingAttr);
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("Pedal", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(WheelInputFix).GetMethod("PedalPrefix", bindingAttr)));
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("UpdateWheelSpin", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(WheelInputFix).GetMethod("SpinPrefix", bindingAttr)));
		CockpitUpdate.Self.Log("Linear wheel response: hardware " + HardwareDegrees.Value + " degrees, driving range " + DrivingDegrees.Value + " degrees; connected wheel takes priority over Quest controls.");
	}

	private static bool PedalPrefix(int i, ushort raw, ref float __result)
	{
		if (i < 0 || i > 1)
		{
			return true;
		}
		if (!pedalReady[i] && Math.Abs(raw - 32767) < 400)
		{
			__result = 0f;
			return false;
		}
		pedalReady[i] = true;
		sampledAt = Time.unscaledTime;
		MelonPreferences_Entry<bool> melonPreferences_Entry = (MelonPreferences_Entry<bool>)invert.GetValue(null);
		__result = WheelResponse.Pedal(raw, melonPreferences_Entry.Value);
		return false;
	}

	internal static void Prepare(ref Vector2 stick, ref float gas, ref float brake)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!WheelOwnsControls)
		{
			return;
		}
		FunselektorFusionVPStandardInput nativeInput = CockpitUpdate.NativeInput;
		bool inputEnabled = nativeInput != null && NetworkBool.op_Implicit(nativeInput.inputIsEnabled);
		if (!CockpitUpdate.IsDriving || !inputEnabled)
		{
			stick = Vector2.zero;
			gas = (brake = 0f);
			return;
		}
		try
		{
			DeviceInfo val = CockpitUpdate.Read<DeviceInfo>("wheelDev");
			DIJOYSTATE2 val2 = ((val == null) ? null : DIManager.GetDeviceStateRaw(val));
			if (val2 != null && val2.lX >= 0 && val2.lX <= 65535)
			{
				physicalSteering = WheelResponse.Axis(val2.lX);
				haveSteering = true;
				stick = new Vector2(WheelResponse.Steering(val2.lX, HardwareDegrees.Value, DrivingDegrees.Value), 0f);
			}
		}
		catch (Exception ex)
		{
			CockpitUpdate.Self.Warn("Wheel axis: " + ex.Message);
		}
	}

	private static bool SpinPrefix()
	{
		if (CockpitUpdate.QuestSteeringActive && !WheelOwnsControls)
		{
			DrawWheel(CockpitUpdate.QuestSteering, CockpitUpdate.QuestLock.Value);
			return false;
		}
		if (!WheelOwnsControls || !haveSteering || !CockpitUpdate.IsDriving)
		{
			return true;
		}
		DrawWheel(physicalSteering, HardwareDegrees.Value);
		return false;
	}

	internal static void UpdateQuestWheel()
	{
		if (CockpitUpdate.QuestSteeringActive && !WheelOwnsControls)
		{
			DrawWheel(CockpitUpdate.QuestSteering, CockpitUpdate.QuestLock.Value);
		}
	}

	private static void DrawWheel(float steering, float lockDegrees)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = CockpitUpdate.Read<GameObject>("wheelGo");
		if (!((Object)(object)val == (Object)null) && CockpitUpdate.Read<bool>("wheelDriven"))
		{
			MelonPreferences_Entry<bool> melonPreferences_Entry = (MelonPreferences_Entry<bool>)CockpitUpdate.DriverType.Assembly.GetType("OverTheHillVR.Mod").GetField("WheelInvert", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
			float num = steering * Mathf.Clamp(lockDegrees, 180f, 1080f) * 0.5f * (float)((!melonPreferences_Entry.Value) ? 1 : (-1));
			val.transform.localRotation = Quaternion.AngleAxis(num, CockpitUpdate.Read<Vector3>("wheelAxisLocal")) * CockpitUpdate.Read<Quaternion>("wheelBaseRot");
		}
	}
}
