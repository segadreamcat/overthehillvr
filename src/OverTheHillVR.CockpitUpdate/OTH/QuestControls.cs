using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Il2Cpp;
using Il2CppFusion;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using Valve.VR;

namespace OTH;

internal sealed class QuestControls
{
	private CVRInput input;

	private VRActiveActionSet_t[] sets;

	private readonly Dictionary<string, ulong> actions = new Dictionary<string, ulong>();

	private readonly SteeringWheelSolver solver = new SteeringWheelSolver();

	private InputDevice pad;

	private IntPtr buffer;

	private float nextInit;

	private bool wasVr;

	private bool hadControls;

	private uint lastButtons;

	private float lastSteer;

	private float lastGas;

	private float lastBrake;

	private readonly QuestButtonRouter stickClicks = new QuestButtonRouter();

	private int shiftDirection;

	private float pauseUntil;

	private int testPhase;

	private float testAt;

	public float Steering { get; private set; }

	public bool DrivingActive { get; private set; }

	public bool IsGripping => solver.Gripping;

	public bool ControllersAvailable { get; private set; }

	private bool Init()
	{
		if (input != null)
		{
			return true;
		}
		if (Time.unscaledTime < nextInit)
		{
			return false;
		}
		nextInit = Time.unscaledTime + 5f;
		input = OpenVR.Input;
		if (input == null)
		{
			return false;
		}
		try
		{
			EVRInputError eVRInputError = input.SetActionManifestPath(Path.GetFullPath(Path.Combine(CockpitUpdate.AssetDir, "actions.json")));
			if (eVRInputError != EVRInputError.None)
			{
				CockpitUpdate.Self.Warn("Quest action manifest: " + eVRInputError);
				input = null;
				return false;
			}
			ulong pHandle = 0uL;
			Check(input.GetActionSetHandle("/actions/drive", ref pHandle), "action set");
			sets = new VRActiveActionSet_t[1]
			{
				new VRActiveActionSet_t
				{
					ulActionSet = pHandle
				}
			};
			string[] array = new string[15]
			{
				"LeftGrip", "RightGrip", "Gas", "Brake", "LeftStick", "RightStick", "A", "B", "X", "Y",
				"Menu", "LeftClick", "RightClick", "LeftPose", "RightPose"
			};
			foreach (string text in array)
			{
				ulong pHandle2 = 0uL;
				Check(input.GetActionHandle("/actions/drive/in/" + text, ref pHandle2), text);
				actions[text] = pHandle2;
			}
			CockpitUpdate.Self.Log("Quest Touch action manifest ready. Grip to steer; RT gas, LT brake; right stick left/right shifts, up/down changes view.");
			return true;
		}
		catch
		{
			input = null;
			sets = null;
			actions.Clear();
			throw;
		}
	}

	public void Tick()
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		DrivingActive = false;
		if (WheelInputFix.WheelOwnsControls || PhysicalGamepadConnected())
		{
			ReleasePad();
			ControllersAvailable = false;
			solver.Reset();
			shiftDirection = 0;
			return;
		}
		bool flag = CockpitUpdate.Read<bool>("vrOn");
		if (!CockpitUpdate.QuestEnabled.Value || !flag || !Application.isFocused)
		{
			ControllersAvailable = false;
			Neutral();
			solver.Reset();
			shiftDirection = 0;
			if (wasVr && !flag)
			{
				input = null;
				actions.Clear();
			}
			wasVr = flag;
			return;
		}
		wasVr = flag;
		if (!Init())
		{
			return;
		}
		if (input.UpdateActionState(sets, (uint)Marshal.SizeOf<VRActiveActionSet_t>()) != EVRInputError.None)
		{
			Neutral();
			solver.Reset();
			return;
		}
		InputPoseActionData_t inputPoseActionData_t = Pose("LeftPose");
		InputPoseActionData_t inputPoseActionData_t2 = Pose("RightPose");
		bool flag2 = inputPoseActionData_t.bActive && inputPoseActionData_t.pose.bPoseIsValid && inputPoseActionData_t.pose.bDeviceIsConnected;
		bool flag3 = inputPoseActionData_t2.bActive && inputPoseActionData_t2.pose.bPoseIsValid && inputPoseActionData_t2.pose.bDeviceIsConnected;
		ControllersAvailable = flag2 | flag3;
		if (!ControllersAvailable)
		{
			Neutral();
			solver.Reset();
			return;
		}
		if (!hadControls)
		{
			hadControls = true;
			CockpitUpdate.Self.Log("Quest controller poses active.");
		}
		bool num = CockpitUpdate.IsDriving && NetworkBool.op_Implicit(CockpitUpdate.NativeInput.inputIsEnabled);
		InputAnalogActionData_t inputAnalogActionData_t = Analog("LeftStick");
		InputAnalogActionData_t inputAnalogActionData_t2 = Analog("RightStick");
		uint num2 = 0u;
		if (flag3 && Digital("A"))
		{
			num2 |= 0x40;
		}
		if (flag3 && Digital("B"))
		{
			num2 |= 0x20;
		}
		if (flag2 && Digital("X"))
		{
			num2 |= 0x80;
		}
		if (flag2 && Digital("Y"))
		{
			num2 |= 0x10;
		}
		bool left = flag2 && Digital("LeftClick");
		bool right = flag3 && Digital("RightClick");
		int num3 = stickClicks.Update(left, right, Time.unscaledDeltaTime);
		if ((num3 & 4) != 0)
		{
			pauseUntil = Time.unscaledTime + 0.12f;
		}
		if (Time.unscaledTime < pauseUntil)
		{
			num2 |= 0x1000;
		}
		if ((num3 & 1) != 0)
		{
			CockpitUpdate.DriverType.GetMethod("Recenter", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(CockpitUpdate.Driver, null);
		}
		if (num && (num3 & 2) != 0)
		{
			num2 |= 8;
		}
		float steer = 0f;
		float gas = 0f;
		float brake = 0f;
		if (num)
		{
			bool leftGrip = flag2 && Digital("LeftGrip");
			bool rightGrip = flag3 && Digital("RightGrip");
			Vector3 val = CockpitUpdate.Read<Quaternion>("hmdRot") * Vector3.right;
			steer = solver.Update(leftGrip, rightGrip, Position(inputPoseActionData_t.pose), Position(inputPoseActionData_t2.pose), new Vector3(val.x, val.y, 0f - val.z), CockpitUpdate.QuestLock.Value, Time.unscaledDeltaTime);
			if ((!solver.Gripping & flag2) && Math.Abs(inputAnalogActionData_t.x) > 0.22f)
			{
				steer = Deadzone(inputAnalogActionData_t.x, 0.22f);
			}
			steer = (Steering = SteeringWheelSolver.Smooth(Steering, steer, Time.unscaledDeltaTime, CockpitUpdate.QuestSmoothing.Value));
			DrivingActive = true;
			gas = (flag3 ? Trigger(Analog("Gas").x) : 0f);
			brake = (flag2 ? Trigger(Analog("Brake").x) : 0f);
			int num5 = (flag3 ? ((inputAnalogActionData_t2.x > 0.65f) ? 1 : ((inputAnalogActionData_t2.x < -0.65f) ? (-1) : 0)) : 0);
			if (num5 != 0 && shiftDirection == 0)
			{
				CockpitUpdate.RequestGear(num5, "Quest right stick");
			}
			shiftDirection = num5;
			if (flag3 && inputAnalogActionData_t2.y > 0.65f)
			{
				num2 |= 1;
			}
			if (flag3 && inputAnalogActionData_t2.y < -0.65f)
			{
				num2 |= 2;
			}
		}
		else
		{
			Steering = 0f;
			solver.Reset();
			shiftDirection = 0;
			if (flag2 && inputAnalogActionData_t.y > 0.65f)
			{
				num2 |= 1;
			}
			if (flag2 && inputAnalogActionData_t.y < -0.65f)
			{
				num2 |= 2;
			}
			if (flag2 && inputAnalogActionData_t.x < -0.65f)
			{
				num2 |= 4;
			}
			if (flag2 && inputAnalogActionData_t.x > 0.65f)
			{
				num2 |= 8;
			}
		}
		Send(num2, steer, gas, brake);
	}

	private bool Digital(string name)
	{
		InputDigitalActionData_t pActionData = default;
		if (input.GetDigitalActionData(actions[name], ref pActionData, (uint)Marshal.SizeOf<InputDigitalActionData_t>(), 0uL) == EVRInputError.None && pActionData.bActive)
		{
			return pActionData.bState;
		}
		return false;
	}

	private InputAnalogActionData_t Analog(string name)
	{
		InputAnalogActionData_t pActionData = default;
		if (input.GetAnalogActionData(actions[name], ref pActionData, (uint)Marshal.SizeOf<InputAnalogActionData_t>(), 0uL) != EVRInputError.None || !pActionData.bActive)
		{
			return default;
		}
		return pActionData;
	}

	private InputPoseActionData_t Pose(string name)
	{
		InputPoseActionData_t pActionData = default;
		ETrackingUniverseOrigin eOrigin = OpenVR.Compositor?.GetTrackingSpace() ?? ETrackingUniverseOrigin.TrackingUniverseStanding;
		if (input.GetPoseActionDataRelativeToNow(actions[name], eOrigin, 0f, ref pActionData, (uint)Marshal.SizeOf<InputPoseActionData_t>(), 0uL) != EVRInputError.None)
		{
			return default;
		}
		return pActionData;
	}

	private static Vector3 Position(TrackedDevicePose_t p)
	{
		return new Vector3(p.mDeviceToAbsoluteTracking.m3, p.mDeviceToAbsoluteTracking.m7, p.mDeviceToAbsoluteTracking.m11);
	}

	private static float Trigger(float x)
	{
		if (float.IsFinite(x) && !(x < 0.06f))
		{
			return Math.Clamp((x - 0.06f) / 0.94f, 0f, 1f);
		}
		return 0f;
	}

	private static float Deadzone(float x, float d)
	{
		return (float)Math.Sign(x) * Math.Clamp((Math.Abs(x) - d) / (1f - d), 0f, 1f);
	}

	private static void Check(EVRInputError e, string name)
	{
		if (e != EVRInputError.None)
		{
			throw new Exception("Quest " + name + ": " + e);
		}
	}

	internal static bool PhysicalGamepadConnected()
	{
		ReadOnlyArray<InputDevice> devices = InputSystem.devices;
		for (int i = 0; i < devices.Count; i++)
		{
			InputDevice val = devices[i];
			if (val != null && val.added && val.enabled && ((InputControl)val).layout != "OTHVRWheelPad" && ((Il2CppObjectBase)(object)val).TryCast<Gamepad>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public void Neutral()
	{
		DrivingActive = false;
		Steering = 0f;
		pauseUntil = 0f;
		stickClicks.Reset();
		if (pad != null)
		{
			Send(0u, 0f, 0f, 0f);
		}
	}

	private void ReleasePad()
	{
		if (pad != null && pad.added)
		{
			InputSystem.RemoveDevice(pad);
		}
		pad = null;
		lastButtons = 0u;
		lastSteer = (lastGas = (lastBrake = 0f));
		stickClicks.Reset();
		pauseUntil = 0f;
		Steering = 0f;
		DrivingActive = false;
	}

	private unsafe void Send(uint buttons, float steer, float gas, float brake)
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (buttons != lastButtons || !(Math.Abs(steer - lastSteer) < 0.002f) || !(Math.Abs(gas - lastGas) < 0.002f) || !(Math.Abs(brake - lastBrake) < 0.002f))
		{
			if (pad == null || !pad.added)
			{
				pad = InputSystem.AddDevice("OTHVRWheelPad", "OTHVR Quest Touch", (string)null);
				CockpitUpdate.Self.Log("Quest virtual gamepad ready (device " + pad.deviceId + ").");
			}
			if (buffer == IntPtr.Zero)
			{
				buffer = Marshal.AllocHGlobal(64);
			}
			byte* ptr = (byte*)(void*)buffer;
			for (int i = 0; i < 64; i++)
			{
				ptr[i] = 0;
			}
			*(int*)ptr = 1398030676;
			((short*)ptr)[2] = 52;
			((short*)ptr)[3] = (short)(ushort)pad.deviceId;
			((double*)ptr)[1] = InputState.currentTime;
			((int*)ptr)[5] = 1196441924;
			((int*)ptr)[6] = (int)buttons;
			float* ptr2 = (float*)ptr + 7;
			*ptr2 = Math.Clamp(steer, -1f, 1f);
			ptr2[4] = Math.Clamp(brake, 0f, 1f);
			ptr2[5] = Math.Clamp(gas, 0f, 1f);
			InputSystem.QueueEvent(new InputEventPtr((InputEvent*)ptr));
			lastButtons = buttons;
			lastSteer = steer;
			lastGas = gas;
			lastBrake = brake;
		}
	}

	internal void RunSelfTest()
	{
		if (!CockpitUpdate.SelfTest.Value || testPhase >= 2 || Time.unscaledTime < 20f || !CockpitUpdate.IsDriving)
		{
			return;
		}
		NetworkedVehicleStatus val = CockpitUpdate.Read<NetworkedVehicleStatus>("carStatus");
		if (!((Object)(object)val == (Object)null) && !(Math.Abs(val.VehicleSpeed) > 0.1f) && val.HandbrakeSet)
		{
			if (testPhase == 0)
			{
				Send(65536u, 0f, 0f, 0f);
				testAt = Time.unscaledTime;
				testPhase = 1;
				CockpitUpdate.Self.Log("SELFTEST gear up queued while parked.");
			}
			else if (Time.unscaledTime - testAt > 0.25f)
			{
				Neutral();
				testPhase = 2;
				CockpitUpdate.Self.Log("SELFTEST gear release queued.");
			}
		}
	}

	public void Dispose()
	{
		Neutral();
		ReleasePad();
		if (buffer != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(buffer);
			buffer = IntPtr.Zero;
		}
	}
}
