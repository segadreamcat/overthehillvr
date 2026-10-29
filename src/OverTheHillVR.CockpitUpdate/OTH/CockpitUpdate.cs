using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppFusion;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppVehiclePhysics;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace OTH;

public class CockpitUpdate : MelonMod
{
	internal static CockpitUpdate Self;

	internal const string Layout = "OTHVRWheelPad";

	internal const uint GearUpBit = 65536u;

	internal const uint GearDownBit = 131072u;

	internal static Type DriverType;

	internal static FieldInfo InstanceField;

	internal static FieldInfo PadField;

	internal static Func<int, bool> WheelHeld;

	private static int neutralDepth;

	private static bool paddleUpHeld;

	private static bool paddleDownHeld;

	internal static string AssetDir;

	internal static MelonPreferences_Entry<bool> QuestEnabled;

	internal static MelonPreferences_Entry<float> QuestLock;

	internal static MelonPreferences_Entry<float> QuestSmoothing;

	internal static MelonPreferences_Entry<bool> GearDisplayEnabled;

	internal static MelonPreferences_Entry<int> PaddleUp;

	internal static MelonPreferences_Entry<int> PaddleDown;

	internal static MelonPreferences_Entry<bool> SelfTest;

	private float nextDisplay;

	private int errors;

	private string lastMap;

	private readonly QuestControls quest = new QuestControls();

	private readonly DashboardCluster cluster = new DashboardCluster();

	private readonly DashboardTilt tilt = new DashboardTilt();

	private readonly ConsoleGps gps = new ConsoleGps();

	private readonly WindshieldRain windshield = new WindshieldRain();

	private readonly CompassPlacement compass = new CompassPlacement();

	private readonly ShifterAnimation shifters = new ShifterAnimation();

	private readonly RearviewMirror rearview = new RearviewMirror();

	private readonly MenuBranding branding = new MenuBranding();

	private readonly ControllerFocus controllerFocus = new ControllerFocus();

	internal readonly SceneryStability scenery = new SceneryStability();

	internal static MelonPreferences_Entry<float> NightBrightness;

	internal static MelonPreferences_Entry<float> DayBrightness;

	internal static MelonPreferences_Entry<bool> SpeedometerMph;

	internal static object Driver => InstanceField?.GetValue(null);

	internal static bool IsDriving
	{
		get
		{
			try
			{
				GameInput instance = GameInput.instance;
				NetworkedVehicleStatus val = Read<NetworkedVehicleStatus>("carStatus");
				if ((Object)(object)instance == (Object)null || instance.playerInputActions == null || (Object)(object)val == (Object)null || !OwnsVehicle)
				{
					return false;
				}
				if (GameInput.IsMapActive((Map)0))
				{
					if (instance.playerInputActions != null && !instance.playerInputActions.Driving.enabled)
					{
						try { instance.playerInputActions.Driving.Enable(); } catch {}
					}
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}

	internal static FunselektorFusionVPStandardInput NativeInput
	{
		get
		{
			VehicleBase val = Read<VehicleBase>("carVb");
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			return ((Component)val).GetComponent<FunselektorFusionVPStandardInput>() ?? ((Component)val).GetComponentInChildren<FunselektorFusionVPStandardInput>() ?? ((Component)val).GetComponentInParent<FunselektorFusionVPStandardInput>();
		}
	}

	internal static bool OwnsVehicle
	{
		get
		{
			FunselektorFusionVPStandardInput nativeInput = NativeInput;
			if ((Object)(object)nativeInput != (Object)null)
			{
				if ((!((Object)(object)nativeInput.fusionPlayer != (Object)null) || !((NetworkBehaviour)nativeInput.fusionPlayer).HasInputAuthority) && (!((Object)(object)nativeInput.vehicleInput != (Object)null) || !((NetworkBehaviour)nativeInput.vehicleInput).HasInputAuthority))
				{
					return ((NetworkBehaviour)Read<NetworkedVehicleStatus>("carStatus")).HasInputAuthority;
				}
				return true;
			}
			return false;
		}
	}

	internal static bool QuestSteeringActive => Self?.quest.DrivingActive ?? false;

	internal static float QuestSteering => Self?.quest.Steering ?? 0f;

	internal static T Read<T>(string field)
	{
		object driver = Driver;
		if (driver == null)
		{
			return default;
		}
		return (T)DriverType.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(driver);
	}

	public override void OnInitializeMelon()
	{
		Self = this;
		AssetDir = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "OverTheHillVR", "controls");
		MelonPreferences_Category melonPreferences_Category = MelonPreferences.CreateCategory("OTHCockpitUpdate");
		QuestEnabled = melonPreferences_Category.CreateEntry("QuestControllers", default_value: true, "Quest Touch virtual steering and gamepad controls");
		QuestLock = melonPreferences_Category.CreateEntry("QuestWheelLockDegrees", 360f, "Virtual wheel rotation from full left to full right");
		QuestSmoothing = melonPreferences_Category.CreateEntry("QuestSteeringSmoothingSeconds", 0.025f, "Short tracking-jitter filter; 0 disables smoothing");
		GearDisplayEnabled = melonPreferences_Category.CreateEntry("DashboardGearDisplay", default_value: true, "Show the actual gear and engine/brake state on the dash");
		NightBrightness = melonPreferences_Category.CreateEntry("DashboardNightBrightness", 0.085f, "Legacy setting; DashboardDayBrightness now controls the fixed backlight");
		DashboardTilt.Enabled = melonPreferences_Category.CreateEntry("DashboardTiltIndicator", default_value: true, "Mount the game's pitch/roll indicators on the center dashboard");
		ConsoleGps.Enabled = melonPreferences_Category.CreateEntry("ConsoleGPS", default_value: true, "Live north-up minimap on the center console");
		ConsoleGps.Range = melonPreferences_Category.CreateEntry("ConsoleGPSRangeMeters", 900f, "Horizontal map coverage in meters, 150 to 4000");
		WindshieldRain.Enabled = melonPreferences_Category.CreateEntry("WindshieldRain", default_value: true, "Rain droplets and automatic windshield wipers in the shared cockpit");
		DayBrightness = melonPreferences_Category.CreateEntry("DashboardDayBrightness", 0.8f, "Fixed instrument backlight at all times, 0 to 1");
		SceneryStability.Enabled = melonPreferences_Category.CreateEntry("StableSceneryCulling", default_value: true, "Disable depth occlusion for VR views to reduce scenery popping/flicker");
		ShifterAnimation.Enabled = melonPreferences_Category.CreateEntry("AnimatedShifters", default_value: true, "Move the gear and 4H/4L levers with transmission changes");
		RearviewMirror.Enabled = melonPreferences_Category.CreateEntry("RearviewMirror", default_value: true, "Live interior rearview mirror");
		MenuBranding.Enabled = melonPreferences_Category.CreateEntry("MenuVRTitle", default_value: true, "Lowercase vr beneath the main title, in the original font");
		SpeedometerMph = melonPreferences_Category.CreateEntry("SpeedometerMPH", default_value: true, "MPH dial and main reading; secondary speed remains in km/h");
		PaddleUp = melonPreferences_Category.CreateEntry("PaddleUpButton", 5, "Right paddle button number (1-based)");
		PaddleDown = melonPreferences_Category.CreateEntry("PaddleDownButton", 6, "Left paddle button number (1-based)");
		SelfTest = melonPreferences_Category.CreateEntry("RunInputSelfTest", default_value: false, "Developer test: sends a shift-up press and release once while parked");
		Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly x) => x.GetName().Name == "OverTheHillVR");
		DriverType = assembly?.GetType("OverTheHillVR.VrDriver");
		if (DriverType == null)
		{
			LoggerInstance.Error("Requires the matching OverTheHillVR.dll core from this release.");
			return;
		}
		InstanceField = DriverType.GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		PadField = DriverType.GetField("vpad", BindingFlags.Instance | BindingFlags.NonPublic);
		WheelHeld = (Func<int, bool>)Delegate.CreateDelegate(typeof(Func<int, bool>), assembly.GetType("OverTheHillVR.WheelPad").GetMethod("Held"));
		InputSystem.RegisterLayout("{\"name\":\"OTHVRWheelPad\",\"extend\":\"Gamepad\",\"displayName\":\"OTHVR Wheel / Touch\",\"controls\":[{\"name\":\"leftStick\",\"processors\":\"stickDeadzone(min=0.0001,max=1)\"},{\"name\":\"gearUpButton\",\"layout\":\"Button\",\"offset\":0,\"bit\":16,\"format\":\"BIT\"},{\"name\":\"gearDownButton\",\"layout\":\"Button\",\"offset\":0,\"bit\":17,\"format\":\"BIT\"}]}", "OTHVRWheelPad", new Nullable<InputDeviceMatcher>());
		WheelInputFix.Initialize(melonPreferences_Category, HarmonyInstance);
		CockpitVehicles.Initialize(HarmonyInstance);
		controllerFocus.Initialize(HarmonyInstance);
		scenery.Initialize(HarmonyInstance);
		HarmonyInstance.Patch(DriverType.GetMethod("SendPad", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CockpitUpdate).GetMethod("WheelPrefix", BindingFlags.Static | BindingFlags.NonPublic)));
		HarmonyInstance.Patch(DriverType.GetMethod("SendNeutral", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CockpitUpdate).GetMethod("NeutralPrefix", BindingFlags.Static | BindingFlags.NonPublic)), null, null, new HarmonyMethod(typeof(CockpitUpdate).GetMethod("NeutralFinalizer", BindingFlags.Static | BindingFlags.NonPublic)), null);
		HarmonyInstance.Patch(DriverType.GetMethod("SyncMute", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CockpitUpdate).GetMethod("MutePrefix", BindingFlags.Static | BindingFlags.NonPublic)));
		LoggerInstance.Msg("Native paddle gear requests and driver instrument cluster enabled.");
	}

	private static void WheelPrefix(object __instance, ref uint btn, ref Vector2 ls, ref float rt, ref float lt)
	{
		try
		{
			if (neutralDepth == 0)
			{
				WheelInputFix.Prepare(ref ls, ref rt, ref lt);
				bool flag = WheelHeld(PaddleUp.Value);
				bool flag2 = WheelHeld(PaddleDown.Value);
				if (flag && !paddleUpHeld && !flag2)
				{
					RequestGear(1, "right paddle");
				}
				if (flag2 && !paddleDownHeld && !flag)
				{
					RequestGear(-1, "left paddle");
				}
				paddleUpHeld = flag;
				paddleDownHeld = flag2;
			}
			else
			{
				paddleUpHeld = false;
				paddleDownHeld = false;
			}
			object? value = PadField.GetValue(__instance);
			InputDevice val = (InputDevice)((value is InputDevice) ? value : null);
			if (val == null || !val.added || ((InputControl)val).layout != "OTHVRWheelPad")
			{
				if (val != null && val.added)
				{
					InputSystem.RemoveDevice(val);
				}
				val = InputSystem.AddDevice("OTHVRWheelPad", "OTHVR Wheel", (string)null);
				PadField.SetValue(__instance, val);
				Self.LoggerInstance.Msg($"Wheel gear controls ready (device {val.deviceId}).");
			}
		}
		catch (Exception ex)
		{
			Self.Warn("Paddle mapping: " + ex.Message);
		}
	}

	private static void NeutralPrefix()
	{
		neutralDepth++;
	}

	private static Exception NeutralFinalizer(Exception __exception)
	{
		neutralDepth = Math.Max(0, neutralDepth - 1);
		return __exception;
	}

	internal static bool RequestGear(int direction, string source)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isFocused || !IsDriving)
		{
			Self.Log("Gear request ignored: " + source + " / " + DrivingState());
			return false;
		}
		FunselektorFusionVPStandardInput nativeInput = NativeInput;
		if ((Object)(object)nativeInput == (Object)null)
		{
			Self.Warn("No native transmission input on the player's car.");
			return false;
		}
		if (!NetworkBool.op_Implicit(nativeInput.inputIsEnabled))
		{
			Self.Log("Gear request ignored while the game's vehicle input is disabled: " + source);
			return false;
		}
		NetworkRunner val = (((Object)(object)nativeInput.fusionPlayer != (Object)null) ? ((SimulationBehaviour)nativeInput.fusionPlayer).Runner : null);
		if ((Object)(object)val != (Object)null && (int)val.GameMode != 1)
		{
			if (!NetworkGearInput.Queue(nativeInput.vehicleInput, direction))
			{
				Self.Warn("No owned vehicle input available for network gear request.");
				return false;
			}
			Self.Log($"Network gear request {source}: {((direction > 0) ? "up" : "down")} queued through native VehicleInput ({val.GameMode}).");
			return true;
		}
		int selectedManualGear = nativeInput.SelectedManualGear;
		if (direction > 0)
		{
			nativeInput.GearUp();
		}
		else
		{
			nativeInput.GearDown();
		}
		Self.Log($"Gear request {source}: selected {selectedManualGear} -> {nativeInput.SelectedManualGear}, actual {Read<NetworkedVehicleStatus>("carStatus").GearboxGear}.");
		return true;
	}

	private static bool MutePrefix(object __instance)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
		Camera val = Read<Camera>("src");
		object value = DriverType.Assembly.GetType("OverTheHillVR.Mod").GetField("MirrorToDesktop", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
		bool flag = (bool)value.GetType().GetProperty("Value").GetValue(value);
		MethodInfo method = DriverType.GetMethod("Unmute", bindingAttr);
		if (flag || (Object)(object)val == (Object)null)
		{
			method.Invoke(__instance, null);
			return false;
		}
		Camera val2 = Read<Camera>("muted");
		if ((Object)(object)val2 == (Object)null || ((Il2CppObjectBase)(object)val2).Pointer != ((Il2CppObjectBase)(object)val).Pointer)
		{
			method.Invoke(__instance, null);
			DriverType.GetField("muted", bindingAttr).SetValue(__instance, val);
			DriverType.GetField("mutedRect", bindingAttr).SetValue(__instance, val.rect);
		}
		float num = Mathf.Clamp(Mathf.Max(128f / (float)Mathf.Max(1, Screen.width), 128f / (float)Mathf.Max(1, Screen.height)), 0.002f, 1f);
		val.rect = new Rect(0f, 0f, num, num);
		return false;
	}

	internal void Warn(string s)
	{
		if (errors++ < 8)
		{
			LoggerInstance.Warning(s);
		}
	}

	internal void Log(string s)
	{
		LoggerInstance.Msg(s);
	}

	private static string DrivingState()
	{
		GameInput instance = GameInput.instance;
		if (((instance != null) ? instance.playerInputActions : null) == null)
		{
			return "input not ready";
		}
		NetworkedVehicleStatus val = Read<NetworkedVehicleStatus>("carStatus");
		FunselektorFusionVPStandardInput nativeInput = NativeInput;
		return $"nativeDriving={GameInput.IsMapActive((Map)0)}, actions={GameInput.instance.playerInputActions.Driving.enabled}, statusAuthority={(Object)(object)val != (Object)null && ((NetworkBehaviour)val).HasInputAuthority}, playerAuthority={(Object)(object)((nativeInput != null) ? nativeInput.fusionPlayer : null) != (Object)null && ((NetworkBehaviour)nativeInput.fusionPlayer).HasInputAuthority}, inputAuthority={(Object)(object)((nativeInput != null) ? nativeInput.vehicleInput : null) != (Object)null && ((NetworkBehaviour)nativeInput.vehicleInput).HasInputAuthority}, focused={Application.isFocused}";
	}

	public override void OnUpdate()
	{
		try
		{
			branding.Update();
		}
		catch (Exception ex)
		{
			Warn("Menu title: " + ex.Message);
		}
		if (DriverType == null || Driver == null)
		{
			return;
		}
		try
		{
			controllerFocus.Update();
			quest.Tick();
			if (!((Object)(object)GameInput.instance == (Object)null))
			{
				string text = DrivingState();
				if (text != lastMap)
				{
					lastMap = text;
					LoggerInstance.Msg("Input map: " + text);
				}
			}
		}
		catch (Exception ex2)
		{
			Warn("Input update: " + ex2.Message);
			quest.Neutral();
		}
	}

	public override void OnLateUpdate()
	{
		if (DriverType != null && Driver != null)
		{
			WheelInputFix.UpdateQuestWheel();
		}
		if (DriverType != null && Driver != null)
		{
			quest.RunSelfTest();
		}
		if (DriverType != null && Driver != null)
		{
			try
			{
				tilt.Update();
			}
			catch (Exception ex)
			{
				Warn("Dashboard tilt: " + ex.Message);
				tilt.Restore();
			}
			try
			{
				gps.Update();
			}
			catch (Exception ex2)
			{
				Warn("Console GPS: " + ex2.Message);
				gps.Dispose();
			}
			try
			{
				windshield.Update();
			}
			catch (Exception ex3)
			{
				Warn("Windshield rain: " + ex3.Message);
				windshield.Dispose();
			}
			try
			{
				compass.Update();
			}
			catch (Exception ex4)
			{
				Warn("Compass placement: " + ex4.Message);
				compass.Restore();
			}
			try
			{
				shifters.Update();
			}
			catch (Exception ex5)
			{
				Warn("Shifter animation: " + ex5.Message);
			}
			try
			{
				rearview.Update();
			}
			catch (Exception ex6)
			{
				Warn("Rearview mirror: " + ex6.Message);
				rearview.Dispose();
			}
		}
		if (DriverType == null || Driver == null || Time.unscaledTime < nextDisplay)
		{
			return;
		}
		nextDisplay = Time.unscaledTime + 0.1f;
		try
		{
			cluster.Update();
		}
		catch (Exception ex7)
		{
			Warn("Dashboard: " + ex7.Message);
			nextDisplay += 2f;
		}
	}

	public override void OnApplicationQuit()
	{
		quest.Dispose();
		rearview.Dispose();
		scenery.Restore();
		cluster.Dispose();
		tilt.Restore();
		gps.Dispose();
		windshield.Dispose();
		compass.Restore();
	}
}
