using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Il2Cpp;
using Il2CppDirectInputManager;
using Il2CppFusion;
using Il2CppGPUInstancerPro;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Reflection;
using Il2CppVehiclePhysics;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Valve.VR;

namespace OverTheHillVR;

internal class VrDriver
{
	private struct Style
	{
		public string Name;

		public Color Leather;

		public Color Dark;

		public Color Wood;

		public Color Headliner;

		public Color Carpet;

		public Color Fabric;

		public Color WheelTint;

		public Color ShifterTint;
	}

	private delegate bool EnumProc(IntPtr h, IntPtr l);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private delegate void RenderEventFn(int id);

	private class HudRec
	{
		public Canvas c;

		public RenderMode mode;

		public Camera cam;

		public float plane;
	}

	private static readonly Vector3 WheelCenter = new Vector3(-0.3289f, 1.16f, 0.11f);

	private static readonly Quaternion WheelRimRot = Quaternion.Euler(60f, 180f, 180f);

	private const float WheelRadius = 0.212f;

	private static readonly Vector3 GearBase = new Vector3(0.005f, 0.7885f, -0.05f);

	private const float GearHeight = 0.34f;

	private static readonly Quaternion GearRot = Quaternion.Euler(352f, 0f, 0f);

	private static readonly Vector3 GaugeNormal = new Vector3(0f, 0.342f, -0.94f);

	private static readonly Vector3[] GaugeCenters = new Vector3[2]
	{
		new Vector3(-0.3989f, 1.1644f, 0.2792f),
		new Vector3(-0.2589f, 1.1644f, 0.2792f)
	};

	private const float GaugeRadius = 0.0425f;

	private IntPtr dressedBody;

	private readonly List<GameObject> dressAdded = new List<GameObject>();

	private GameObject wheelGo;

	private GameObject roofGo;

	private readonly Transform[] needles = new Transform[2];

	private Quaternion wheelBaseRot;

	private Vector3 wheelAxisLocal;

	private bool wheelDriven;

	private bool steerWarned;

	private float cockpitStableSince = -1f;

	private static Texture2D dialTex;

	private float gaugeSpeed;

	private float gaugeRpm;

	private float upSince = -1f;

	private bool upFired;

	private GameObject interiorGo;

	private GameObject shifterGo;

	private string carKey = "";

	private Color carPaint = new Color(0.22f, 0.47f, 0.78f);

	private readonly Dictionary<IntPtr, Color> origCol = new Dictionary<IntPtr, Color>();

	private static readonly Style[] Styles = new Style[6]
	{
		new Style
		{
			Name = "Classic",
			Leather = C(0.62f, 0.31f, 0.15f),
			Dark = C(0.077f, 0.071f, 0.071f),
			Wood = C(0.42f, 0.23f, 0.09f),
			Headliner = C(0.7f, 0.66f, 0.58f),
			Carpet = C(0.08f, 0.08f, 0.085f),
			Fabric = C(0.3f, 0.3f, 0.31f),
			WheelTint = C(1f, 1f, 1f),
			ShifterTint = C(1f, 1f, 1f)
		},
		new Style
		{
			Name = "Safari",
			Leather = C(0.72f, 0.58f, 0.4f),
			Dark = C(0.13f, 0.11f, 0.08f),
			Wood = C(0.62f, 0.47f, 0.29f),
			Headliner = C(0.78f, 0.74f, 0.64f),
			Carpet = C(0.17f, 0.14f, 0.1f),
			Fabric = C(0.48f, 0.42f, 0.32f),
			WheelTint = C(1.2f, 1.1f, 0.95f),
			ShifterTint = C(1.15f, 1.05f, 0.9f)
		},
		new Style
		{
			Name = "Rally",
			Leather = C(0.55f, 0.08f, 0.07f),
			Dark = C(0.045f, 0.045f, 0.05f),
			Wood = C(0.13f, 0.13f, 0.14f),
			Headliner = C(0.22f, 0.22f, 0.23f),
			Carpet = C(0.05f, 0.05f, 0.055f),
			Fabric = C(0.16f, 0.16f, 0.17f),
			WheelTint = C(0.33f, 0.33f, 0.35f),
			ShifterTint = C(0.4f, 0.4f, 0.42f)
		},
		new Style
		{
			Name = "Ranch",
			Leather = C(0.38f, 0.1f, 0.08f),
			Dark = C(0.09f, 0.06f, 0.05f),
			Wood = C(0.3f, 0.16f, 0.07f),
			Headliner = C(0.76f, 0.7f, 0.58f),
			Carpet = C(0.11f, 0.08f, 0.06f),
			Fabric = C(0.32f, 0.26f, 0.22f),
			WheelTint = C(0.7f, 0.58f, 0.5f),
			ShifterTint = C(0.75f, 0.5f, 0.45f)
		},
		new Style
		{
			Name = "Alpine",
			Leather = C(0.4f, 0.46f, 0.54f),
			Dark = C(0.08f, 0.09f, 0.11f),
			Wood = C(0.68f, 0.57f, 0.42f),
			Headliner = C(0.75f, 0.76f, 0.77f),
			Carpet = C(0.09f, 0.1f, 0.12f),
			Fabric = C(0.42f, 0.45f, 0.5f),
			WheelTint = C(1.15f, 1.1f, 1.05f),
			ShifterTint = C(0.85f, 0.9f, 1f)
		},
		new Style
		{
			Name = "Expedition",
			Leather = C(0.34f, 0.37f, 0.21f),
			Dark = C(0.08f, 0.08f, 0.06f),
			Wood = C(0.45f, 0.3f, 0.15f),
			Headliner = C(0.62f, 0.58f, 0.46f),
			Carpet = C(0.1f, 0.1f, 0.07f),
			Fabric = C(0.36f, 0.37f, 0.28f),
			WheelTint = C(0.85f, 0.8f, 0.7f),
			ShifterTint = C(0.8f, 0.85f, 0.65f)
		}
	};

	private static readonly int[] AutoStyles = new int[4] { 0, 1, 4, 5 };

	private readonly List<(Material m, Color c)> lifted = new List<(Material, Color)>();

	private float dayLevel = 1f;

	private float nextDayCheck;

	private float nextDayLog;

	private static readonly string[] SkipNames = new string[17]
	{
		"wheel", "tire", "tyre", "glass", "window", "light", "lamp", "interior", "shadow", "cockpit",
		"seat", "driver", "rim", "brake", "exhaust", "mud", "dirt"
	};

	private static readonly string[] BodyHints = new string[7] { "body", "livery", "paint", "chassis", "hull", "shell", "car" };

	private int forceMode;

	private bool lastFlag;

	private float cockpitSince = -1f;

	private bool seatWarned;

	private float chaseYaw;

	private float chaseDist;

	private float chaseH;

	private bool chaseInit;

	private Vector3 bonnetLocal;

	private bool bonnetValid;

	private float bonnetSince = -1f;

	private int viewLevel;

	private NetworkedVehicleStatus carStatus;

	private VehicleBase carVb;

	private ExtraCameraControls zoomCtl;

	private float nextZoomCtlSearch;

	private float prevZoomTarget = float.NaN;

	private int zoomInSign;

	private float nextCarLog;

	private static readonly Vector3 SeatEye = new Vector3(-0.3289f, 1.55f, -0.44f);

	private const int TheaterLayer = 0;

	private static readonly Vector3 TheaterOrigin = VMath.V(0f, -6000f, 0f);

	private GameObject theaterRoot;

	private Material theaterScreenMat;

	private GameObject theaterScreen;

	private readonly Camera[] tEyes = new Camera[2];

	private bool theaterActive;

	private static IntPtr gameWnd;

	private float nextFocusTry;

	private int focusTries;

	private static readonly string[] VrBadEffects = new string[5] { "MotionBlur", "DepthOfField", "LensDistortion", "ChromaticAberration", "PaniniProjection" };

	private readonly List<VolumeComponent> disabledFx = new List<VolumeComponent>();

	private float nextFxScan;

	private float savedShadowDist = -1f;

	internal static VrDriver Instance;

	private CVRSystem sys;

	private bool vrOn;

	private uint eyeW;

	private uint eyeH;

	private readonly float[][] raw = new float[2][]
	{
		new float[4],
		new float[4]
	};

	private readonly Vector3[] eyeOffset = new Vector3[2];

	private readonly Quaternion[] eyeRotOffset = new Quaternion[2];

	private static CVRCompositor sComp;

	private static CVRSystem sSys;

	private static CVROverlay sOverlay;

	private static readonly object sCallLock = new object();

	private static readonly object sDataLock = new object();

	private static readonly TrackedDevicePose_t[] rtPoses = new TrackedDevicePose_t[64];

	private static readonly TrackedDevicePose_t[] rtNoGame = new TrackedDevicePose_t[0];

	private static readonly TrackedDevicePose_t[] sPoses = new TrackedDevicePose_t[64];

	private static bool sWaited;

	private static readonly HmdMatrix34_t[] sSlotPose = new HmdMatrix34_t[4];

	private static readonly bool[] sSlotValid = new bool[4];

	private static EColorSpace sColor = EColorSpace.Gamma;

	private static readonly IntPtr[] sTex = new IntPtr[2];

	private static VRTextureBounds_t sBounds;

	private static volatile int sSubmitError;

	private static int sOk;

	private static int sErr;

	private static ulong sOvHandle;

	private static IntPtr sScreenTex;

	private static bool sScreenFlip;

	private static volatile bool sPlaceScreen;

	private static float sScreenWidth = 3.2f;

	private static float sScreenDist = 2.6f;

	private static readonly RenderEventFn sRenderDel = OnRenderEvent;

	private static readonly IntPtr sRenderFn = Marshal.GetFunctionPointerForDelegate(sRenderDel);

	private const int EvSubmit = 10;

	private const int EvWaitOnly = 2;

	private const int EvScreen = 3;

	private const int EvHideScreen = 4;

	private const int EvHud = 5;

	private const int EvHideHud = 6;

	private static ulong sHudHandle;

	private static IntPtr sHudTex;

	private static volatile bool sPlaceHud;

	private static float sHudWidth = 1.4f;

	private static float sHudDist = 1.1f;

	private static float sHudDown = 0.25f;

	private static readonly (bool copy, EColorSpace cs, string name)[] Modes = new (bool, EColorSpace, string)[6]
	{
		(true, EColorSpace.Gamma, "copy/gamma"),
		(true, EColorSpace.Auto, "copy/auto"),
		(false, EColorSpace.Gamma, "direct/gamma"),
		(false, EColorSpace.Linear, "direct/linear"),
		(false, EColorSpace.Auto, "direct/auto"),
		(true, EColorSpace.Linear, "copy/linear")
	};

	private int mode;

	private float nextModeCheck;

	private bool modeConfirmed;

	private int loggedSubmitError;

	private Camera src;

	private IntPtr srcPtr;

	private readonly Camera[] eyes = new Camera[2];

	private readonly RenderTexture[] eyeRT = new RenderTexture[2];

	private readonly Texture2D[] eyeCopy = new Texture2D[2];

	private Camera muted;

	private bool frameReady;

	private int renderSlot;

	private int lastFrame = -1;

	private readonly TrackedDevicePose_t[] renderPoses = new TrackedDevicePose_t[64];

	private bool hmdValid;

	private Vector3 hmdPos;

	private Quaternion hmdRot = VMath.Identity;

	private Vector3 origin;

	private float originYaw;

	private bool needRecenter = true;

	private GameObject cockpitGo;

	private Transform body;

	private Rigidbody bodyRb;

	private Vector3 seatLocal;

	private bool seatValid;

	private float nextCockpitSearch;

	private bool inCockpit;

	private bool stereoView;

	private RenderTexture screenRT;

	private Texture2D screenTex;

	private bool screenShown;

	private Action<ScriptableRenderContext, List<Camera>> beginRender;

	private bool frameHooked;

	private bool autoStarted;

	private int lastCoroutineFrame = -1;

	private float coroutineCheck = 10f;

	private bool fallbackEndHook;

	private Action<ScriptableRenderContext, List<Camera>> endRender;

	private float seatSaveAt;

	private float nextStatus;

	private int dressFailed;

	private bool shotPending;

	private float autoShotAt;

	private bool autoShotDone;

	private string shotName;

	private int lastEndFrame = -1;

	private Rect mutedRect;

	private readonly List<HudRec> hudCanvases = new List<HudRec>();

	private readonly HashSet<IntPtr> hudSeen = new HashSet<IntPtr>();

	private Camera hudCam;

	private RenderTexture hudRT;

	private bool hudShown;

	private bool hudOn = true;

	private float nextHudScan;

	private DeviceInfo ffbDev;

	private bool ffbTried;

	private bool ffbReady;

	private float ffbNextTry;

	private float ffbNextLog;

	private int ffbLastConst = 99999;

	private int ffbLastDamper = -1;

	private int ffbLastSpring = -1;

	private float ffbConst;

	private float ffbHit;

	private float ffbHitT;

	private Vector3 ffbPrevVel;

	private float ffbPrevRollRate;

	private bool ffbHavePrev;

	private float ffbNextFocus;

	private int ffbFails;

	private int ffbErrors;

	private int ffbDir;

	private float ffbTestT = -1f;

	private float ffbTestStart;

	private float ffbTestNext;

	private bool isDirectDrive;

	private float ffbLastTick;

	private Vector3 prevRawPos;

	private float prevRawYaw;

	private bool prevRawValid;

	private DeviceInfo wheelDev;

	private bool wheelTried;

	private float wheelNextTry;

	private float wheelLastUse = -100f;

	private float wheelNextFocus;

	private InputDevice vpad;

	private IntPtr evBuf;

	private uint sentButtons = uint.MaxValue;

	private Vector2 sentLS;

	private float sentLT = -1f;

	private float sentRT = -1f;

	private float wheelNextAxisLog;

	private readonly int[] lastAx = new int[3];

	private int lastPov = -1;

	private int wheelErrors;

	private int wheelReattach;

	private float wheelAttachedAt;

	private bool wheelGotData;

	private const int GPAD = 1196441924;

	private const int STAT = 1398030676;

	private const int EvSize = 52;

	private float nextDevScan;

	private readonly HashSet<int> seenDevs = new HashSet<int>();

	private int povMode;

	private uint povBase = uint.MaxValue;

	private bool povLogged;

	private readonly bool[] pedalSeen = new bool[2];

	private readonly float[] pedalRest = new float[2];

	private readonly float[] pedalSeenAt = new float[2];

	private static MelonLogger.Instance L => Mod.L;

	private void UpdateDress(Camera cam)
	{
		bool flag = inCockpit && (Object)(object)body != (Object)null;
		if ((Object)(object)body != (Object)null && ((Il2CppObjectBase)(object)body).Pointer != dressedBody && dressedBody != IntPtr.Zero)
		{
			ClearDress();
		}
		if (flag && dressedBody == IntPtr.Zero)
		{
			dressedBody = ((Il2CppObjectBase)(object)body).Pointer;
			try
			{
				BuildCockpit();
			}
			catch (Exception ex)
			{
				L.Warning("Cockpit build failed: " + ex);
			}
		}
		foreach (GameObject item in dressAdded)
		{
			if ((Object)(object)item != (Object)null && item.activeSelf != flag)
			{
				item.SetActive(flag);
			}
		}
		if ((Object)(object)roofGo != (Object)null)
		{
			bool flag2 = flag && Mod.RoofOn.Value;
			if (roofGo.activeSelf != flag2)
			{
				roofGo.SetActive(flag2);
			}
		}
		RoofToggleTick();
		LiftTick();
		if (flag)
		{
			UpdateWheelSpin();
			UpdateGauges();
		}
	}

	private void ClearDress()
	{
		foreach (GameObject item in dressAdded)
		{
			if ((Object)(object)item != (Object)null)
			{
				Object.Destroy((Object)(object)item);
			}
		}
		if ((Object)(object)roofGo != (Object)null)
		{
			Object.Destroy((Object)(object)roofGo);
		}
		dressAdded.Clear();
		roofGo = null;
		wheelGo = null;
		interiorGo = null;
		shifterGo = null;
		needles[0] = (needles[1] = null);
		wheelDriven = false;
		dressedBody = IntPtr.Zero;
		lifted.Clear();
	}

	private void DestroyMirrors()
	{
	}

	private static void NoCull(GameObject go)
	{
		foreach (MeshRenderer componentsInChild in go.GetComponentsInChildren<MeshRenderer>(true))
		{
			Material sharedMaterial = ((Renderer)componentsInChild).sharedMaterial;
			if ((Object)(object)sharedMaterial != (Object)null)
			{
				sharedMaterial.SetFloat("_Cull", 0f);
			}
		}
	}

	private void BuildCockpit()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Othm.Spawn("interior.othm", "OTHVR Interior", body);
		if ((Object)(object)val != (Object)null)
		{
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = Vector3.one;
			NoCull(val);
			dressAdded.Add(val);
		}
		interiorGo = val;
		roofGo = Othm.Spawn("roof.othm", "OTHVR Roof", body);
		if ((Object)(object)roofGo != (Object)null)
		{
			roofGo.transform.localPosition = Vector3.zero;
			roofGo.transform.localRotation = Quaternion.identity;
			NoCull(roofGo);
		}
		if (Mod.ModelWheel.Value)
		{
			GameObject val2 = Othm.Spawn("wheel.othm", "OTHVR Wheel", body);
			if ((Object)(object)val2 != (Object)null)
			{
				Vector3 val3 = Vector3.up;
				if (Vector3.Dot(WheelRimRot * val3, SeatEye - WheelCenter) < 0f)
				{
					val3 = Vector3.down;
				}
				Quaternion val4 = WheelRimRot * Quaternion.FromToRotation(Vector3.back, val3);
				val2.transform.localPosition = WheelCenter;
				val2.transform.localRotation = val4;
				val2.transform.localScale = Vector3.one * 0.212f * Mod.WheelScale.Value;
				wheelGo = val2;
				wheelBaseRot = val4;
				Vector3 val5 = val4 * Vector3.forward;
				wheelAxisLocal = val5.normalized;
				wheelDriven = Mod.WheelFromCar.Value;
				dressAdded.Add(val2);
			}
		}
		if (Mod.ModelShifter.Value)
		{
			GameObject val6 = Othm.Spawn("shifter.othm", "OTHVR Shifter", body);
			if ((Object)(object)val6 != (Object)null)
			{
				float value = Mod.ShifterStraighten.Value;
				val6.transform.localPosition = GearBase;
				val6.transform.localRotation = GearRot * Quaternion.Euler(8.75f * value, 0f, -14.5f * value);
				val6.transform.localScale = Vector3.one * 0.34f * Mod.ShifterScale.Value;
				shifterGo = val6;
				dressAdded.Add(val6);
			}
		}
		BuildGauges();
		try
		{
			ApplyInteriorLook(null);
		}
		catch (Exception ex)
		{
			L.Warning("Interior colors failed: " + ex.Message);
		}
		L.Msg("Driver seat cockpit built.");
	}

	private static Texture2D DialTexture()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected Obj, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)dialTex != (Object)null)
		{
			return dialTex;
		}
		Color32[] array = new Color32[65536];
		Color32 val = new Color32((byte)12, (byte)12, (byte)13, byte.MaxValue);
		Color32 val2 = new Color32((byte)225, (byte)222, (byte)210, byte.MaxValue);
		Color32 val3 = new Color32((byte)200, (byte)40, (byte)30, byte.MaxValue);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				float num = ((float)j + 0.5f) / 256f * 2f - 1f;
				float num2 = ((float)i + 0.5f) / 256f * 2f - 1f;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				Color32 val4;
				if (num3 > 1f)
				{
					val4 = new Color32((byte)0, (byte)0, (byte)0, byte.MaxValue);
				}
				else
				{
					val4 = (Color32)((num3 > 0.94f) ? new Color32((byte)150, (byte)150, (byte)150, byte.MaxValue) : val);
				}
				if (num3 <= 0.94f && num3 > 0.62f)
				{
					float num4 = Mathf.Atan2(num, num2) * 57.29578f;
					if (num4 >= -135f && num4 <= 135f)
					{
						float num5 = (num4 + 135f) / 270f * 10f;
						bool flag = Mathf.Abs(num5 - Mathf.Round(num5)) * 27f < 1.1f && num3 > 0.66f;
						float num6 = num5 * 2f;
						bool flag2 = Mathf.Abs(num6 - Mathf.Round(num6)) * 13.5f < 0.7f && num3 > 0.8f;
						if (flag | flag2)
						{
							val4 = ((num5 > 8.5f) ? val3 : val2);
						}
					}
				}
				if (num3 < 0.07f)
				{
					val4 = new Color32((byte)90, (byte)90, (byte)92, byte.MaxValue);
				}
				array[i * 256 + j] = val4;
			}
		}
		dialTex = new Texture2D(256, 256, (TextureFormat)4, true);
		dialTex.SetPixels32((Il2CppStructArray<Color32>?)array);
		dialTex.Apply(true);
		((Object)dialTex).hideFlags = (HideFlags)32;
		return dialTex;
	}

	private static Mesh QuadMesh(float w, float h, float cx, float cy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		val.vertices = new Vector3[4]
		{
			VMath.V(cx - w / 2f, cy - h / 2f, 0f),
			VMath.V(cx + w / 2f, cy - h / 2f, 0f),
			VMath.V(cx + w / 2f, cy + h / 2f, 0f),
			VMath.V(cx - w / 2f, cy + h / 2f, 0f)
		};
		val.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
		val.triangles = new int[12]
		{
			0, 2, 1, 0, 3, 2, 0, 1, 2, 0,
			2, 3
		};
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}

	private GameObject MeshGo(string name, Mesh mesh, Material mat, Transform parent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		GameObject val = new GameObject(name);
		val.transform.SetParent(parent, false);
		val.AddComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).sharedMaterial = mat;
		((Renderer)val2).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)val2).receiveShadows = false;
		return val;
	}

	private void BuildGauges()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected Obj, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		Material val = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
		val.SetTexture("_BaseMap", (Texture)(object)DialTexture());
		val.mainTexture = (Texture)(object)DialTexture();
		val.SetFloat("_Cull", 0f);
		val.SetColor("_BaseColor", new Color(0.8f, 0.8f, 0.8f, 1f));
		Material val2 = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
		val2.SetColor("_BaseColor", new Color(0.95f, 0.45f, 0.1f, 1f));
		val2.SetFloat("_Cull", 0f);
		Mesh mesh = QuadMesh(0.085f, 0.085f, 0f, 0f);
		Mesh mesh2 = QuadMesh(0.0028f, 0.036125f, 0f, 0.01615f);
		Quaternion localRotation = Quaternion.LookRotation(-GaugeNormal, Vector3.up);
		for (int i = 0; i < 2; i++)
		{
			GameObject val3 = MeshGo("OTHVR Gauge " + i, mesh, val, body);
			val3.transform.localPosition = GaugeCenters[i] + GaugeNormal * 0.003f;
			val3.transform.localRotation = localRotation;
			GameObject val4 = MeshGo("Needle", mesh2, val2, val3.transform);
			val4.transform.localPosition = VMath.V(0f, 0f, -0.0015f);
			needles[i] = val4.transform;
			dressAdded.Add(val3);
		}
	}

	private void UpdateGauges()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)needles[0] == (Object)null)
		{
			return;
		}
		float num;
		if (!((Object)(object)bodyRb != (Object)null))
		{
			num = 0f;
		}
		else
		{
			Vector3 velocity = bodyRb.velocity;
			num = velocity.magnitude * 3.6f;
		}
		float num2 = num;
		float num3 = -1f;
		try
		{
			if ((Object)(object)carStatus != (Object)null)
			{
				num3 = carStatus.EngineRpm;
			}
		}
		catch
		{
		}
		if (num3 < 0f)
		{
			num3 = 800f + num2 * 45f;
		}
		float num4 = 1f - Mathf.Exp(-8f * Time.deltaTime);
		gaugeSpeed += (num2 - gaugeSpeed) * num4;
		gaugeRpm += (num3 - gaugeRpm) * num4;
		SetNeedle(needles[0], gaugeSpeed / 160f);
		SetNeedle(needles[1], gaugeRpm / 6000f);
	}

	private static void SetNeedle(Transform t, float frac)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		frac = Mathf.Clamp01(frac);
		t.localRotation = Quaternion.AngleAxis(135f - 270f * frac, Vector3.forward);
	}

	private float CarSteer()
	{
		try
		{
			if ((Object)(object)carVb == (Object)null)
			{
				return 0f;
			}
			return (float)carVb.data.Get(Channel.Input, InputData.Steer) / 10000f;
		}
		catch (Exception ex)
		{
			if (!steerWarned)
			{
				steerWarned = true;
				L.Warning("steer read: " + ex.Message);
			}
			return 0f;
		}
	}

	private void UpdateWheelSpin()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (wheelDriven && !((Object)(object)wheelGo == (Object)null))
		{
			float num = Mathf.Clamp(CarSteer(), -1f, 1f) * Mod.WheelLock.Value * 0.5f * (Mod.WheelInvert.Value ? (-1f) : 1f);
			wheelGo.transform.localRotation = Quaternion.AngleAxis(num, wheelAxisLocal) * wheelBaseRot;
		}
	}

	private void RoofToggleTick()
	{
		if (!inCockpit)
		{
			cockpitStableSince = -1f;
			upSince = -1f;
		}
		else if (cockpitStableSince < 0f)
		{
			cockpitStableSince = Time.unscaledTime;
		}
		else
		{
			if (Time.unscaledTime - cockpitStableSince < 0.8f)
			{
				return;
			}
			if (ZoomInHeld() || Pad.DpadUpHeld() || WheelPad.PovUpHeld())
			{
				if (upSince < 0f)
				{
					upSince = Time.unscaledTime;
				}
				else if (!upFired && Time.unscaledTime - upSince >= 1f)
				{
					upFired = true;
					NextInteriorStyle();
				}
			}
			else if (upSince >= 0f)
			{
				if (!upFired)
				{
					ToggleRoof();
				}
				upSince = -1f;
				upFired = false;
			}
			if (Keys.Down(115))
			{
				ToggleRoof();
			}
			if (Keys.Down(114))
			{
				NextInteriorStyle();
			}
		}
	}

	private void ToggleRoof()
	{
		Mod.RoofOn.Value = !Mod.RoofOn.Value;
		MelonPreferences.Save();
		L.Msg("Roof " + (Mod.RoofOn.Value ? "on (hardtop)." : "off (open top)."));
	}

	private bool ZoomInHeld()
	{
		try
		{
			GameInput instance = GameInput.instance;
			object obj;
			if (instance == null)
			{
				obj = null;
			}
			else
			{
				PlayerInputActions playerInputActions = instance.playerInputActions;
				obj = ((playerInputActions != null) ? playerInputActions.m_CameraOrbitAndZoom_CameraZoomIn : null);
			}
			InputAction val = (InputAction)obj;
			return val != null && val.IsPressed();
		}
		catch
		{
			return false;
		}
	}

	private Bounds CarLocalBounds(Renderer r)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = r.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 val = VMath.V(1E+09f, 1E+09f, 1E+09f);
		Vector3 val2 = VMath.V(-1E+09f, -1E+09f, -1E+09f);
		for (int i = 0; i < 8; i++)
		{
			Vector3 val3 = body.InverseTransformPoint(VMath.V(((i & 1) != 0) ? max.x : min.x, ((i & 2) != 0) ? max.y : min.y, ((i & 4) != 0) ? max.z : min.z));
			val = Vector3.Min(val, val3);
			val2 = Vector3.Max(val2, val3);
		}
		Bounds result = default;
		result.SetMinMax(val, val2);
		return result;
	}

	private static Color C(float r, float g, float b)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new Color(r, g, b, 1f);
	}

	private int CurrentStyle()
	{
		int value = Mod.InteriorStyle.Value;
		if (value >= 0 && value < Styles.Length)
		{
			return value;
		}
		uint num = 2166136261u;
		string text = carKey;
		foreach (char c in text)
		{
			num ^= c;
			num *= 16777619;
		}
		return AutoStyles[num % (uint)AutoStyles.Length];
	}

	private void NextInteriorStyle()
	{
		int num = CurrentStyle();
		Mod.InteriorStyle.Value = (num + 1) % Styles.Length;
		MelonPreferences.Save();
		L.Msg("Interior style: " + Styles[Mod.InteriorStyle.Value].Name + " (set InteriorStyle = -1 for automatic per car).");
		ApplyColors();
	}

	private void ApplyInteriorLook(Transform fppRoot)
	{
		DetectCarPaint(fppRoot);
		ApplyColors();
	}

	private void Lift(Material m, Color c)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Texture texture = m.GetTexture("_BaseMap");
		m.EnableKeyword("_EMISSION");
		m.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)0;
		if ((Object)(object)texture != (Object)null)
		{
			m.SetTexture("_EmissionMap", texture);
		}
		lifted.RemoveAll(((Material m, Color c) x) => (Object)(object)x.m == (Object)null || ((Il2CppObjectBase)(object)x.m).Pointer == ((Il2CppObjectBase)(object)m).Pointer);
		lifted.Add((m, c));
		SetLift(m, c);
	}

	private void SetLift(Material m, Color c)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		float num = Mod.InteriorBrightness.Value * dayLevel * dayLevel;
		m.SetColor("_EmissionColor", new Color(c.r * num, c.g * num, c.b * num, 1f));
		float num2 = 1f - Mod.InteriorNightDim.Value * (1f - dayLevel);
		m.SetColor("_BaseColor", new Color(c.r * num2, c.g * num2, c.b * num2, c.a));
	}

	private void LiftTick()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (lifted.Count == 0 || Time.unscaledTime < nextDayCheck)
		{
			return;
		}
		nextDayCheck = Time.unscaledTime + 1f;
		float num = DaylightLevel();
		if (Mathf.Abs(num - dayLevel) < 0.03f)
		{
			return;
		}
		dayLevel = Mathf.MoveTowards(dayLevel, num, 0.1f);
		foreach (var (val, c) in lifted)
		{
			if ((Object)(object)val != (Object)null)
			{
				SetLift(val, c);
			}
		}
	}

	private float DaylightLevel()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Invalid comparison between Unknown and I4
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		try
		{
			Light val = RenderSettings.sun;
			if ((Object)(object)val == (Object)null)
			{
				foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<Light>()))
				{
					Light val2 = ((Il2CppObjectBase)(object)item).TryCast<Light>();
					if ((Object)(object)val2 != (Object)null && (int)val2.type == 1 && ((Behaviour)val2).isActiveAndEnabled && ((Object)(object)val == (Object)null || val2.intensity > val.intensity))
					{
						val = val2;
					}
				}
			}
			float num = 0.5f;
			float num2 = 0f;
			float num3 = 0f;
			if ((Object)(object)val != (Object)null && ((Behaviour)val).isActiveAndEnabled)
			{
				num2 = Vector3.Dot(-((Component)val).transform.forward, Vector3.up);
				num3 = val.intensity;
				num = Mathf.Clamp01(num2 * 4f) * Mathf.Clamp01(num3);
			}
			else if ((Object)(object)val != (Object)null)
			{
				num = 0f;
			}
			Color val3 = (((int)RenderSettings.ambientMode == 1) ? RenderSettings.ambientSkyColor : RenderSettings.ambientLight);
			float num4 = (0.3f * val3.r + 0.59f * val3.g + 0.11f * val3.b) * Mathf.Max(0.01f, RenderSettings.ambientIntensity);
			float num5 = Mathf.Clamp01((num4 - 0.04f) / 0.25f);
			float num6 = Mathf.Min(num, num5);
			if (Time.unscaledTime > nextDayLog)
			{
				nextDayLog = Time.unscaledTime + 30f;
				L.Msg($"Daylight {num6:0.00} (sun '{(((Object)(object)val != (Object)null) ? ((Object)val).name : "-")}' height {num2:0.00} intensity {num3:0.00}, ambient {num4:0.00}).");
			}
			return num6;
		}
		catch
		{
			return 1f;
		}
	}

	private void ApplyColors()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Style st = Styles[CurrentStyle()];
		Paint(interiorGo);
		Paint(roofGo);
		Tint(wheelGo, st.WheelTint);
		Tint(shifterGo, st.ShifterTint);
		L.Msg($"Interior look: style {st.Name}{((Mod.InteriorStyle.Value < 0) ? " (auto)" : "")}, paint {(Mod.MatchCarColor.Value ? ("car color " + ColorUtility.ToHtmlStringRGB(carPaint)) : "default")}.");
		static Color D(Color c)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Mathf.Min(1f, c.r / 0.8f), Mathf.Min(1f, c.g / 0.8f), Mathf.Min(1f, c.b / 0.8f), 1f);
		}
		void Paint(GameObject go)
		{
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)go == (Object)null)
			{
				return;
			}
			foreach (MeshRenderer componentsInChild in go.GetComponentsInChildren<MeshRenderer>(true))
			{
				string name = ((Object)((Component)componentsInChild).gameObject).name;
				if (name.StartsWith("part_"))
				{
					Nullable val = name.Substring(5) switch
					{
						"paint" => Mod.MatchCarColor.Value ? new Nullable(D(carPaint)) : ((Nullable)null), 
						"leather" => D(st.Leather), 
						"blackleather" => D(st.Dark), 
						"wood" => D(st.Wood), 
						"headliner" => D(st.Headliner), 
						"carpet" => D(st.Carpet), 
						"fabric" => D(st.Fabric), 
						_ => null, 
					};
					Material material = ((Renderer)componentsInChild).material;
					if (!origCol.ContainsKey(((Il2CppObjectBase)(object)componentsInChild).Pointer))
					{
						origCol[((Il2CppObjectBase)(object)componentsInChild).Pointer] = material.GetColor("_BaseColor");
					}
					material.SetColor("_BaseColor", val ?? origCol[((Il2CppObjectBase)(object)componentsInChild).Pointer]);
					Lift(material, val ?? origCol[((Il2CppObjectBase)(object)componentsInChild).Pointer]);
				}
			}
		}
		void Tint(GameObject go, Color t)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)go == (Object)null)
			{
				return;
			}
			foreach (MeshRenderer componentsInChild2 in go.GetComponentsInChildren<MeshRenderer>(true))
			{
				Material material = ((Renderer)componentsInChild2).material;
				if (!origCol.ContainsKey(((Il2CppObjectBase)(object)componentsInChild2).Pointer))
				{
					origCol[((Il2CppObjectBase)(object)componentsInChild2).Pointer] = material.GetColor("_BaseColor");
				}
				Color val = origCol[((Il2CppObjectBase)(object)componentsInChild2).Pointer];
				Color val2 = new Color(Mathf.Min(1f, val.r * t.r), Mathf.Min(1f, val.g * t.g), Mathf.Min(1f, val.b * t.b), val.a);
				material.SetColor("_BaseColor", val2);
				Lift(material, val2);
			}
		}
	}

	private static bool HasAny(string s, string[] keys)
	{
		s = s.ToLowerInvariant();
		foreach (string value in keys)
		{
			if (s.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	private void DetectCarPaint(Transform fppRoot)
	{
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)body == (Object)null)
		{
			return;
		}
		Renderer val = null;
		Material val2 = null;
		Texture val3 = null;
		float num = 0f;
		List<(float, string)> list = new List<(float, string)>();
		foreach (Renderer componentsInChild in ((Component)body).GetComponentsInChildren<Renderer>(false))
		{
			try
			{
				if (((Object)componentsInChild).name.StartsWith("OTHVR") || ((Object)componentsInChild).name.StartsWith("part") || ((Object)(object)fppRoot != (Object)null && ((Component)componentsInChild).transform.IsChildOf(fppRoot)) || (!((Object)(object)((Il2CppObjectBase)(object)componentsInChild).TryCast<MeshRenderer>() != (Object)null) && !((Object)(object)((Il2CppObjectBase)(object)componentsInChild).TryCast<SkinnedMeshRenderer>() != (Object)null)))
				{
					continue;
				}
				Material sharedMaterial = componentsInChild.sharedMaterial;
				if ((Object)(object)sharedMaterial == (Object)null)
				{
					continue;
				}
				string s = ((Object)componentsInChild).name + " " + ((Object)sharedMaterial).name + " " + (((Object)(object)sharedMaterial.shader != (Object)null) ? ((Object)sharedMaterial.shader).name : "");
				if (HasAny(((Object)componentsInChild).name + " " + ((Object)sharedMaterial).name, SkipNames))
				{
					continue;
				}
				Texture val4 = PaintTexture(sharedMaterial);
				Bounds bounds = componentsInChild.bounds;
				Vector3 size = bounds.size;
				float num2 = size.x * size.y * size.z;
				if (HasAny(s, BodyHints))
				{
					num2 *= 4f;
				}
				if (!((Object)(object)val4 == (Object)null) || sharedMaterial.HasProperty("_BaseColor") || sharedMaterial.HasProperty("_Color"))
				{
					if ((Object)(object)val4 != (Object)null)
					{
						num2 *= 2f;
					}
					list.Add((num2, $"{((Object)componentsInChild).name} / {((Object)sharedMaterial).name} / {(((Object)(object)sharedMaterial.shader != (Object)null) ? ((Object)sharedMaterial.shader).name : "?")} / tex {(((Object)(object)val4 != (Object)null) ? ((Object)val4).name : "-")}"));
					if (num2 > num)
					{
						num = num2;
						val = componentsInChild;
						val2 = sharedMaterial;
						val3 = val4;
					}
				}
			}
			catch
			{
			}
		}
		list.Sort(((float, string) a, (float, string) b) => b.Item1.CompareTo(a.Item1));
		for (int num3 = 0; num3 < Math.Min(4, list.Count); num3++)
		{
			L.Msg($"  paint candidate {list[num3].Item1:0.00}: {list[num3].Item2}");
		}
		if ((Object)(object)val2 == (Object)null)
		{
			L.Msg("Car paint: no body material found, keeping default.");
			carKey = ((Object)body).name;
			return;
		}
		Color val5 = Color.white;
		bool flag = false;
		if ((Object)(object)val3 != (Object)null)
		{
			Nullable val6 = DominantColor(val3);
			if (val6.HasValue)
			{
				val5 = val6.Value;
				flag = true;
			}
		}
		Color val7 = Color.white;
		if (val2.HasProperty("_BaseColor"))
		{
			val7 = val2.GetColor("_BaseColor");
		}
		else if (val2.HasProperty("_Color"))
		{
			val7 = val2.GetColor("_Color");
		}
		if (flag)
		{
			val5 = new Color(val5.r * val7.r, val5.g * val7.g, val5.b * val7.b, 1f);
		}
		else
		{
			val5 = val7;
			flag = true;
		}
		if (Sat(val5) < 0.3f)
		{
			try
			{
				Shader shader = val2.shader;
				int propertyCount = shader.GetPropertyCount();
				Nullable val8 = null;
				float num4 = 0f;
				StringBuilder stringBuilder = new StringBuilder();
				for (int num5 = 0; num5 < propertyCount; num5++)
				{
					if ((int)shader.GetPropertyType(num5) != 0)
					{
						continue;
					}
					string propertyName = shader.GetPropertyName(num5);
					Color color = val2.GetColor(propertyName);
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 2, stringBuilder2);
					handler.AppendLiteral(" ");
					handler.AppendFormatted(propertyName);
					handler.AppendLiteral("=");
					handler.AppendFormatted(ColorUtility.ToHtmlStringRGB(color));
					stringBuilder2.Append(ref handler);
					float num6 = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
					string text = propertyName.ToLowerInvariant();
					if (!(num6 < 0.15f) && !text.Contains("emis") && !text.Contains("spec") && !text.Contains("rim") && !text.Contains("shadow") && !text.Contains("dirt") && !text.Contains("mud"))
					{
						float num7 = Sat(color) * ((text.Contains("paint") || text.Contains("body") || text.Contains("main") || text.Contains("primary") || text.Contains("base")) ? 2f : 1f);
						if (Sat(color) > 0.3f && num7 > num4)
						{
							num4 = num7;
							val8 = color;
						}
					}
				}
				L.Msg("  body colors:" + stringBuilder);
				if (val8.HasValue)
				{
					val5 = val8.Value;
				}
			}
			catch (Exception ex)
			{
				L.Msg("  body colors: " + ex.Message);
			}
		}
		float num8 = Mathf.Max(val5.r, Mathf.Max(val5.g, val5.b));
		if (num8 < 0.35f)
		{
			val5 = ((num8 < 0.01f) ? new Color(0.35f, 0.35f, 0.35f) : new Color(val5.r * 0.35f / num8, val5.g * 0.35f / num8, val5.b * 0.35f / num8));
		}
		carPaint = new Color(Mathf.Clamp(val5.r, 0.03f, 0.95f), Mathf.Clamp(val5.g, 0.03f, 0.95f), Mathf.Clamp(val5.b, 0.03f, 0.95f), 1f);
		carKey = ((Object)body).name.Replace("(Clone)", "") + "|" + (((Object)(object)val3 != (Object)null) ? ((Object)val3).name : ((Object)val2).name);
		L.Msg($"Car paint: {ColorUtility.ToHtmlStringRGB(carPaint)} from '{((Object)val).name}' / '{((Object)val2).name}' (key {carKey}).");
		static float Sat(Color x)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			float num9 = Mathf.Max(x.r, Mathf.Max(x.g, x.b));
			float num10 = Mathf.Min(x.r, Mathf.Min(x.g, x.b));
			if (!(num9 > 0f))
			{
				return 0f;
			}
			return (num9 - num10) / num9;
		}
	}

	private static Texture PaintTexture(Material m)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		Texture val = null;
		try
		{
			Shader shader = m.shader;
			int propertyCount = shader.GetPropertyCount();
			for (int i = 0; i < propertyCount; i++)
			{
				if ((int)shader.GetPropertyType(i) != 4)
				{
					continue;
				}
				string propertyName = shader.GetPropertyName(i);
				Texture texture = m.GetTexture(propertyName);
				if (!((Object)(object)texture == (Object)null) && texture.width >= 16)
				{
					string text = propertyName.ToLowerInvariant();
					if (text.Contains("livery") || text.Contains("paint"))
					{
						return texture;
					}
					if ((Object)(object)val == (Object)null && (text.Contains("basemap") || text.Contains("maintex") || text.Contains("albedo") || text.Contains("diffuse") || text.Contains("color")))
					{
						val = texture;
					}
				}
			}
		}
		catch
		{
		}
		return val ?? m.mainTexture;
	}

	private static Nullable DominantColor(Texture tex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected Obj, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture val = null;
		RenderTexture active = RenderTexture.active;
		try
		{
			val = RenderTexture.GetTemporary(32, 32, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)2);
			Graphics.Blit(tex, val);
			RenderTexture.active = val;
			Texture2D val2 = new Texture2D(32, 32, (TextureFormat)4, false);
			val2.ReadPixels(new Rect(0f, 0f, 32f, 32f), 0, 0);
			val2.Apply();
			Il2CppStructArray<Color32> pixels = val2.GetPixels32();
			Object.Destroy((Object)val2);
			float[] array = new float[512];
			Vector3[] array2 = new Vector3[512];
			for (int i = 0; i < pixels.Length; i++)
			{
				Color32 val3 = pixels[i];
				byte b = Math.Max(val3.r, Math.Max(val3.g, val3.b));
				Math.Min(val3.r, Math.Min(val3.g, val3.b));
				if (b >= 16)
				{
					int num = (val3.r >> 5) * 64 + (val3.g >> 5) * 8 + (val3.b >> 5);
					array[num]++;
					ref Vector3 reference = ref array2[num];
					reference += new Vector3((float)(int)val3.r, (float)(int)val3.g, (float)(int)val3.b);
				}
			}
			float num2 = 0f;
			for (int j = 0; j < 512; j++)
			{
				num2 += array[j];
			}
			int num3 = -1;
			int num4 = -1;
			float num5 = 0f;
			float num6 = 0f;
			for (int k = 0; k < 512; k++)
			{
				if (array[k] == 0f)
				{
					continue;
				}
				Vector3 val4 = array2[k] / array[k] / 255f;
				float num7 = Mathf.Max(val4.x, Mathf.Max(val4.y, val4.z));
				float num8 = Mathf.Min(val4.x, Mathf.Min(val4.y, val4.z));
				float num9 = ((num7 > 0f) ? ((num7 - num8) / num7) : 0f);
				if (num7 < 0.22f)
				{
					continue;
				}
				if (num9 > 0.35f && array[k] > num2 * 0.04f)
				{
					float num10 = array[k] * num9 * num7;
					if (num10 > num5)
					{
						num5 = num10;
						num3 = k;
					}
				}
				float num11 = array[k] * num7;
				if (num11 > num6)
				{
					num6 = num11;
					num4 = k;
				}
			}
			if (num3 < 0)
			{
				num3 = num4;
			}
			if (num3 < 0)
			{
				return null;
			}
			Vector3 val5 = array2[num3] / array[num3] / 255f;
			return new Color(val5.x, val5.y, val5.z, 1f);
		}
		catch (Exception ex)
		{
			Mod.L.Warning("Paint readback: " + ex.Message);
			return null;
		}
		finally
		{
			RenderTexture.active = active;
			if ((Object)(object)val != (Object)null)
			{
				RenderTexture.ReleaseTemporary(val);
			}
		}
	}

	private bool UsesProtectorBonnetPoint()
	{
		if ((Object)(object)body != (Object)null)
		{
			return ((Object)body).name.IndexOf("Protector", StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	private void FindPlayerCar()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		if (Time.unscaledTime < nextCockpitSearch)
		{
			return;
		}
		nextCockpitSearch = Time.unscaledTime + 0.5f;
		NetworkedVehicleStatus val = null;
		float num = float.MaxValue;
		Camera val2 = null;
		try
		{
			val2 = Camera.main;
		}
		catch
		{
		}
		try
		{
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<NetworkedVehicleStatus>()))
			{
				NetworkedVehicleStatus val3 = ((Il2CppObjectBase)(object)item).TryCast<NetworkedVehicleStatus>();
				if (!((Object)(object)val3 == (Object)null) && !((Object)(object)val3.vehicle == (Object)null))
				{
					float num2 = (((Object)(object)val2 != (Object)null) ? Vector3.Distance(((Component)val2).transform.position, ((Component)val3.vehicle).transform.position) : 0f);
					bool flag = false;
					try
					{
						flag = ((NetworkBehaviour)val3).HasInputAuthority;
					}
					catch
					{
					}
					if (flag)
					{
						num2 -= 100000f;
					}
					if (num2 < num)
					{
						num = num2;
						val = val3;
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (Time.unscaledTime > nextCarLog)
			{
				nextCarLog = Time.unscaledTime + 30f;
				L.Warning("car search: " + ex.Message);
			}
		}
		carStatus = val;
		carVb = (((Object)(object)val != (Object)null) ? val.vehicle : null);
		Transform val4 = (((Object)(object)carVb != (Object)null) ? ((Component)carVb).transform : null);
		Rigidbody val5 = null;
		if ((Object)(object)val4 != (Object)null)
		{
			try
			{
				val5 = ((Component)val4).GetComponentInParent<Rigidbody>();
				if ((Object)(object)val5 == (Object)null)
				{
					val5 = ((Component)val4).GetComponentInChildren<Rigidbody>();
				}
			}
			catch
			{
			}
		}
		Transform o = (((Object)(object)val5 != (Object)null) ? ((Component)val5).transform : val4);
		if (!(SafePtr((Object)(object)o) != SafePtr((Object)(object)body)))
		{
			return;
		}
		body = o;
		bodyRb = val5;
		seatValid = false;
		bonnetValid = false;
		if ((Object)(object)body != (Object)null)
		{
			if ((Object)(object)bodyRb != (Object)null && Mod.SmoothCarBody.Value && (int)bodyRb.interpolation == 0)
			{
				bodyRb.interpolation = (RigidbodyInterpolation)1;
				L.Msg("Car rigidbody interpolation turned on for smooth VR.");
			}
			L.Msg($"Car: '{Path(body)}' (driver seat {(CockpitAllowed() ? "yes" : "no, bonnet view only")}).");
			if (viewLevel == 2 && !CockpitAllowed())
			{
				viewLevel = 1;
			}
		}
		else if (Mod.DebugLog.Value)
		{
			L.Msg("Car gone.");
		}
	}

	private void ZoomTick()
	{
		if ((Object)(object)body == (Object)null)
		{
			viewLevel = 0;
			return;
		}
		if ((Object)(object)zoomCtl == (Object)null && Time.unscaledTime > nextZoomCtlSearch)
		{
			nextZoomCtlSearch = Time.unscaledTime + 2f;
			try
			{
				Object val = Object.FindObjectOfType(Il2CppType.Of<ExtraCameraControls>());
				zoomCtl = ((val != (Object)null) ? ((Il2CppObjectBase)(object)val).TryCast<ExtraCameraControls>() : null);
			}
			catch
			{
			}
		}
		bool flag = false;
		bool flag2 = false;
		try
		{
			GameInput instance = GameInput.instance;
			PlayerInputActions val2 = (((Object)(object)instance != (Object)null) ? instance.playerInputActions : null);
			if (val2 != null)
			{
				InputAction cameraOrbitAndZoom_CameraZoomIn = val2.m_CameraOrbitAndZoom_CameraZoomIn;
				InputAction cameraOrbitAndZoom_CameraZoomOut = val2.m_CameraOrbitAndZoom_CameraZoomOut;
				flag = cameraOrbitAndZoom_CameraZoomIn != null && (cameraOrbitAndZoom_CameraZoomIn.WasPressedThisFrame() || cameraOrbitAndZoom_CameraZoomIn.WasPerformedThisFrame());
				flag2 = cameraOrbitAndZoom_CameraZoomOut != null && (cameraOrbitAndZoom_CameraZoomOut.WasPressedThisFrame() || cameraOrbitAndZoom_CameraZoomOut.WasPerformedThisFrame());
			}
		}
		catch
		{
		}
		float num = float.NaN;
		float num2 = 0f;
		float num3 = 0f;
		try
		{
			if ((Object)(object)zoomCtl != (Object)null)
			{
				num = zoomCtl.targetZoomMultiplier;
				num2 = zoomCtl.minZoom;
				num3 = zoomCtl.maxZoom;
			}
		}
		catch
		{
			zoomCtl = null;
		}
		if (!float.IsNaN(prevZoomTarget) && !float.IsNaN(num) && zoomInSign == 0 && Mathf.Abs(num - prevZoomTarget) > 0.0001f && (Object)(object)zoomCtl != (Object)null)
		{
			try
			{
				if (zoomCtl.isZoomingIn)
				{
					zoomInSign = ((!(num < prevZoomTarget)) ? 1 : (-1));
				}
			}
			catch
			{
			}
		}
		bool flag3 = true;
		if (!float.IsNaN(prevZoomTarget) && (Object)(object)zoomCtl != (Object)null)
		{
			float num4 = Mathf.Max(0.01f, (num3 - num2) * 0.02f);
			flag3 = ((zoomInSign > 0) ? (prevZoomTarget >= num3 - num4) : (prevZoomTarget <= num2 + num4));
		}
		prevZoomTarget = num;
		if (flag)
		{
			if ((viewLevel == 0) & flag3)
			{
				SetView(1);
			}
			else if (viewLevel == 1 && CockpitAllowed())
			{
				SetView(2);
			}
		}
		else if (flag2 && viewLevel > 0)
		{
			SetView(viewLevel - 1);
		}
	}

	private void SetView(int lvl)
	{
		if (lvl != viewLevel)
		{
			viewLevel = lvl;
			seatValid = false;
			bonnetValid = false;
			needRecenter = true;
			L.Msg("View: " + lvl switch
			{
				1 => "bonnet", 
				0 => "chase", 
				_ => "driver seat", 
			});
		}
	}

	private void ToggleCockpit()
	{
		SetView((viewLevel != 2) ? ((!CockpitAllowed()) ? 1 : 2) : 0);
	}

	private bool DetectCockpit(Camera cam)
	{
		bool flag = viewLevel == 2 && CockpitAllowed();
		if (flag && !lastFlag && !autoShotDone)
		{
			autoShotAt = Time.unscaledTime + 4f;
		}
		if (autoShotAt > 0f && Time.unscaledTime > autoShotAt)
		{
			autoShotAt = 0f;
			autoShotDone = true;
			shotPending = true;
		}
		lastFlag = flag;
		return flag;
	}

	private Vector3 BonnetPoint()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (UsesProtectorBonnetPoint())
		{
			return new Vector3(0f, 1.65f, 0.61f);
		}
		float num = 1.6f;
		float num2 = 2f;
		try
		{
			bool flag = false;
			float num3 = -1E+09f;
			float num4 = -1E+09f;
			foreach (Renderer componentsInChild in ((Component)body).GetComponentsInChildren<Renderer>(false))
			{
				string text = ((Object)componentsInChild).name.ToLowerInvariant();
				if (!text.Contains("wheel") && !text.Contains("tire") && !text.Contains("tyre") && !text.Contains("shadow") && !text.StartsWith("othvr") && !text.StartsWith("part"))
				{
					Bounds val = CarLocalBounds(componentsInChild);
					num3 = Mathf.Max(num3, val.max.y);
					num4 = Mathf.Max(num4, val.max.z);
					flag = true;
				}
			}
			if (flag)
			{
				num = num3;
				num2 = num4;
			}
		}
		catch
		{
		}
		return new Vector3(0f, num * 0.85f, num2 * 0.3f);
	}

	private static Material Unlit(Color c, Texture tex = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Material val = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
		val.SetColor("_BaseColor", c);
		val.SetFloat("_Cull", 0f);
		if ((Object)(object)tex != (Object)null)
		{
			val.SetTexture("_BaseMap", tex);
			val.mainTexture = tex;
		}
		return val;
	}

	private GameObject Box(string name, Vector3 pos, Vector3 size, Material m)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Collider component = val.GetComponent<Collider>();
		if ((Object)(object)component != (Object)null)
		{
			Object.Destroy((Object)(object)component);
		}
		((Object)val).name = name;
		val.layer = 0;
		val.transform.SetParent(theaterRoot.transform, false);
		val.transform.localPosition = pos;
		val.transform.localScale = size;
		MeshRenderer component2 = val.GetComponent<MeshRenderer>();
		((Renderer)component2).sharedMaterial = m;
		((Renderer)component2).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)component2).receiveShadows = false;
		return val;
	}

	private void BuildTheater()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected Obj, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected Obj, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected Obj, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected Obj, but got Unknown
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)theaterRoot != (Object)null)
		{
			return;
		}
		theaterRoot = new GameObject("OTHVR Theater");
		Object.DontDestroyOnLoad((Object)(object)theaterRoot);
		theaterRoot.transform.position = TheaterOrigin;
		Material m = Unlit(new Color(0.02f, 0.02f, 0.025f, 1f));
		float value = Mod.TheaterScreenWidth.Value;
		float num = ((Screen.height > 0) ? ((float)Screen.width / (float)Screen.height) : 1.7777778f);
		float num2 = value / num;
		float value2 = Mod.TheaterDistance.Value;
		float value3 = Mod.TheaterEyeHeight.Value;
		Box("ScreenFrame", VMath.V(0f, value3, value2 + 0.03f), VMath.V(value + 0.06f, num2 + 0.06f, 0.04f), m);
		theaterScreen = new GameObject("Screen");
		theaterScreen.layer = 0;
		theaterScreen.transform.SetParent(theaterRoot.transform, false);
		theaterScreen.transform.localPosition = VMath.V(0f, value3, value2);
		Mesh val = new Mesh();
		float num3 = value / 2f;
		float num4 = num2 / 2f;
		val.vertices = new Vector3[4]
		{
			VMath.V(0f - num3, 0f - num4, 0f),
			VMath.V(num3, 0f - num4, 0f),
			VMath.V(num3, num4, 0f),
			VMath.V(0f - num3, num4, 0f)
		};
		bool value4 = Mod.TheaterFlip.Value;
		val.uv = new Vector2[4]
		{
			new Vector2(0f, (float)(value4 ? 1 : 0)),
			new Vector2(1f, (float)(value4 ? 1 : 0)),
			new Vector2(1f, (float)((!value4) ? 1 : 0)),
			new Vector2(0f, (float)((!value4) ? 1 : 0))
		};
		val.triangles = new int[12]
		{
			0, 2, 1, 0, 3, 2, 0, 1, 2, 0,
			2, 3
		};
		val.RecalculateBounds();
		theaterScreen.AddComponent<MeshFilter>().sharedMesh = val;
		MeshRenderer val2 = theaterScreen.AddComponent<MeshRenderer>();
		theaterScreenMat = Unlit(Color.white);
		((Renderer)val2).sharedMaterial = theaterScreenMat;
		((Renderer)val2).shadowCastingMode = (ShadowCastingMode)0;
		for (int i = 0; i < 2; i++)
		{
			GameObject val3 = new GameObject("OTHVR Theater Eye " + i);
			Object.DontDestroyOnLoad((Object)val3);
			Camera val4 = val3.AddComponent<Camera>();
			((Behaviour)val4).enabled = false;
			val4.clearFlags = (CameraClearFlags)2;
			val4.backgroundColor = Color.black;
			val4.cullingMask = 1;
			val4.stereoTargetEye = (StereoTargetEyeMask)0;
			val4.depth = -40 + i;
			val4.useOcclusionCulling = false;
			try
			{
				UniversalAdditionalCameraData universalAdditionalCameraData = CameraExtensions.GetUniversalAdditionalCameraData(val4);
				universalAdditionalCameraData.renderPostProcessing = false;
				universalAdditionalCameraData.renderShadows = false;
				universalAdditionalCameraData.antialiasing = (AntialiasingMode)1;
			}
			catch
			{
			}
			tEyes[i] = val4;
		}
		L.Msg("Theater built.");
	}

	private bool TheaterFrame()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (!Mod.Theater.Value)
		{
			SetTheaterActive(on: false);
			return false;
		}
		BuildTheater();
		EnsureScreenTex();
		if ((Object)(object)theaterScreenMat != (Object)null && (Object)(object)screenRT != (Object)null && (Object)(object)theaterScreenMat.mainTexture != (Object)(object)screenRT)
		{
			theaterScreenMat.SetTexture("_BaseMap", (Texture)(object)screenRT);
			theaterScreenMat.mainTexture = (Texture)(object)screenRT;
		}
		SetTheaterActive(on: true);
		if (needRecenter && hmdValid)
		{
			needRecenter = false;
			origin = hmdPos;
			originYaw = VMath.YawOf(hmdRot);
		}
		Quaternion val = VMath.Yaw(0f - originYaw);
		Vector3 a = VMath.Add(VMath.Add(TheaterOrigin, VMath.V(0f, Mod.TheaterEyeHeight.Value, 0f)), VMath.Rotate(val, VMath.Sub(hmdPos, origin)));
		Quaternion val2 = VMath.Mul(val, hmdRot);
		for (int i = 0; i < 2; i++)
		{
			Camera obj = tEyes[i];
			obj.targetTexture = eyeRT[i];
			((Component)obj).transform.SetPositionAndRotation(VMath.Add(a, VMath.Rotate(val2, eyeOffset[i])), VMath.Mul(val2, eyeRotOffset[i]));
			obj.nearClipPlane = 0.05f;
			obj.farClipPlane = 60f;
			obj.projectionMatrix = VMath.Projection(raw[i][0], raw[i][1], raw[i][2], raw[i][3], 0.05f, 60f);
		}
		frameReady = true;
		return true;
	}

	private void SetTheaterActive(bool on)
	{
		theaterActive = on;
		for (int i = 0; i < 2; i++)
		{
			if ((Object)(object)tEyes[i] != (Object)null && ((Behaviour)tEyes[i]).enabled != on)
			{
				((Behaviour)tEyes[i]).enabled = on;
			}
		}
		if ((Object)(object)theaterRoot != (Object)null && theaterRoot.activeSelf != on)
		{
			theaterRoot.SetActive(on);
		}
	}

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	private static extern bool SetForegroundWindow(IntPtr h);

	[DllImport("user32.dll")]
	private static extern bool BringWindowToTop(IntPtr h);

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr h, int cmd);

	[DllImport("user32.dll")]
	private static extern void keybd_event(byte vk, byte scan, uint flags, UIntPtr extra);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumProc cb, IntPtr l);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr h);

	private static IntPtr GameWindow()
	{
		if (gameWnd != IntPtr.Zero)
		{
			return gameWnd;
		}
		uint me = (uint)Environment.ProcessId;
		EnumWindows((IntPtr h, IntPtr l) =>
		{
			GetWindowThreadProcessId(h, out var pid);
			if (pid == me && IsWindowVisible(h))
			{
				gameWnd = h;
				return false;
			}
			return true;
		}, IntPtr.Zero);
		return gameWnd;
	}

	private static bool GameFocused()
	{
		try
		{
			GetWindowThreadProcessId(GetForegroundWindow(), out var pid);
			return pid == (uint)Environment.ProcessId;
		}
		catch
		{
			return true;
		}
	}

	private void FocusGame(string why)
	{
		if (Mod.AutoFocus.Value && !GameFocused())
		{
			IntPtr intPtr = GameWindow();
			if (!(intPtr == IntPtr.Zero))
			{
				keybd_event(18, 0, 0u, UIntPtr.Zero);
				keybd_event(18, 0, 2u, UIntPtr.Zero);
				ShowWindow(intPtr, 9);
				BringWindowToTop(intPtr);
				SetForegroundWindow(intPtr);
				L.Msg("Brought game window to front (" + why + ").");
			}
		}
	}

	private void FocusTick()
	{
		if ((Pad.AnyDown() || WheelPad.AnyDown()) && !GameFocused())
		{
			FocusGame("controller pressed");
		}
		else if (vrOn && focusTries < 6 && Time.unscaledTime > nextFocusTry && !GameFocused())
		{
			focusTries++;
			nextFocusTry = Time.unscaledTime + 2f;
			FocusGame("VR started");
		}
	}

	private void KillVrBlur()
	{
		if (!Mod.NoMotionBlur.Value || Time.unscaledTime < nextFxScan)
		{
			return;
		}
		nextFxScan = Time.unscaledTime + 3f;
		try
		{
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<Volume>()))
			{
				Volume val = ((Il2CppObjectBase)(object)item).TryCast<Volume>();
				if ((Object)(object)val == (Object)null)
				{
					continue;
				}
				VolumeProfile val2 = null;
				try
				{
					val2 = (val.HasInstantiatedProfile() ? val.profile : val.sharedProfile);
				}
				catch
				{
					val2 = val.sharedProfile;
				}
				if ((Object)(object)val2 == (Object)null)
				{
					continue;
				}
				Enumerator<VolumeComponent> enumerator2 = val2.components.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					VolumeComponent current = enumerator2.Current;
					if ((Object)(object)current == (Object)null || !current.active)
					{
						continue;
					}
					string name = ((MemberInfo)((Object)current).GetIl2CppType()).Name;
					string[] vrBadEffects = VrBadEffects;
					foreach (string text in vrBadEffects)
					{
						if (name == text)
						{
							current.active = false;
							disabledFx.Add(current);
							L.Msg($"VR: turned off {name} on '{((Object)val).name}'.");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			L.Warning("blur scan: " + ex.Message);
			nextFxScan = float.MaxValue;
		}
	}

	private void ApplyVrPerf(bool on)
	{
		try
		{
			UniversalRenderPipelineAsset val = ((Il2CppObjectBase)(object)GraphicsSettings.currentRenderPipeline)?.TryCast<UniversalRenderPipelineAsset>();
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			if (on && Mod.VrShadowDistance.Value > 0f)
			{
				if (savedShadowDist < 0f)
				{
					savedShadowDist = val.shadowDistance;
				}
				if (val.shadowDistance > Mod.VrShadowDistance.Value)
				{
					val.shadowDistance = Mod.VrShadowDistance.Value;
					L.Msg($"VR: shadow distance {savedShadowDist:0} -> {val.shadowDistance:0} m.");
				}
			}
			else if (!on && savedShadowDist >= 0f)
			{
				val.shadowDistance = savedShadowDist;
				savedShadowDist = -1f;
			}
		}
		catch (Exception ex)
		{
			L.Warning("perf: " + ex.Message);
		}
	}

	private void RestoreVrBlur()
	{
		foreach (VolumeComponent item in disabledFx)
		{
			try
			{
				if ((Object)(object)item != (Object)null)
				{
					item.active = true;
				}
			}
			catch
			{
			}
		}
		disabledFx.Clear();
		nextFxScan = 0f;
	}

	internal VrDriver()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		MelonCoroutines.Start(EndOfFrameLoop());
	}

	internal void Update()
	{
		WatchdogCheck();
		try
		{
			Keys.Poll();
			Pad.Poll();
			if (wheelErrors <= 20)
			{
				try
				{
					WheelTick();
				}
				catch (Exception ex)
				{
					if (++wheelErrors <= 3)
					{
						L.Warning("Wheel: " + ex.Message);
					}
				}
			}
			if (ffbErrors < 20)
			{
				try
				{
					FfbTick();
				}
				catch (Exception ex2)
				{
					if (++ffbErrors <= 3)
					{
						L.Warning("FFB tick: " + ex2.Message);
					}
				}
			}
			try
			{
				FocusTick();
			}
			catch
			{
			}
			if (Keys.Down(119) || Pad.ComboDown(32768) || WheelPad.HeldFor(Mod.WbVr.Value, 1f))
			{
				if (vrOn)
				{
					StopVr();
				}
				else
				{
					StartVr();
				}
			}
			if (Keys.Down(117) || Pad.RecenterDown() || WheelPad.Down(Mod.WbRecenter.Value))
			{
				Recenter();
			}
			if (Keys.Down(122))
			{
				ToggleCockpit();
			}
			if (Keys.Down(116) || Pad.HeldFor(192, 1f) || WheelPad.Down(Mod.WbShot.Value))
			{
				shotPending = true;
			}
			if (Keys.Down(123) || Pad.ComboDown(8192) || WheelPad.Down(Mod.WbHud.Value))
			{
				hudOn = !hudOn;
				L.Msg("HUD panel " + (hudOn ? "on" : "off"));
			}
			if (Keys.Down(120) || Pad.ComboDown(16384))
			{
				forceMode = (forceMode + 1) % 3;
				MelonLogger.Instance l = L;
				string text;
				if (forceMode != 0)
				{
					text = ((forceMode == 1) ? "forced 3D" : "forced flat screen");
				}
				else
				{
					text = "auto";
				}
				l.Msg("View mode: " + text);
			}
			if (Keys.Down(118))
			{
				Mod.FlipEyeY.Value = !Mod.FlipEyeY.Value;
				UpdateBounds();
			}
			SeatKeys();
			if (!autoStarted && Time.frameCount > 60)
			{
				autoStarted = true;
				if (Mod.StartInVR.Value)
				{
					StartVr();
				}
			}
			if (vrOn && !fallbackEndHook && Time.unscaledTime > coroutineCheck && lastCoroutineFrame < Time.frameCount - 30)
			{
				fallbackEndHook = true;
				try
				{
					endRender = Action<ScriptableRenderContext, List<Camera>>.op_Implicit((Action<ScriptableRenderContext, List<Camera>>)((ScriptableRenderContext c, List<Camera> val) =>
					{
						EndOfFrame();
					}));
					RenderPipelineManager.endContextRendering += endRender;
					L.Warning("End-of-frame coroutine not running; using URP endContextRendering instead.");
				}
				catch (Exception ex3)
				{
					L.Error("No end-of-frame hook available: " + ex3.Message);
				}
			}
			if (vrOn)
			{
				if (sSubmitError != 0 && sSubmitError != loggedSubmitError)
				{
					loggedSubmitError = sSubmitError;
					MelonLogger.Instance l2 = L;
					EVRCompositorError eVRCompositorError = (EVRCompositorError)sSubmitError;
					l2.Warning("SteamVR submit error: " + eVRCompositorError);
				}
				CheckSubmitMode();
				FindPlayerCar();
				ZoomTick();
				KillVrBlur();
				if (savedShadowDist < 0f)
				{
					ApplyVrPerf(on: true);
				}
			}
		}
		catch (Exception e)
		{
			Fail(e);
		}
	}

	private void SeatKeys()
	{
		float num = 0.01f;
		bool flag = false;
		if (Keys.Held(33) || Pad.ComboHeld(1))
		{
			Mod.SeatUp.Value += num * 0.5f;
			flag = true;
		}
		if (Keys.Held(34) || Pad.ComboHeld(2))
		{
			Mod.SeatUp.Value -= num * 0.5f;
			flag = true;
		}
		if (WheelPad.Down(Mod.WbSeatUp.Value))
		{
			Mod.SeatUp.Value += 0.01f;
			flag = true;
		}
		if (WheelPad.Down(Mod.WbSeatDown.Value))
		{
			Mod.SeatUp.Value -= 0.01f;
			flag = true;
		}
		if (Keys.Held(36) || Pad.ComboHeld(8))
		{
			Mod.SeatForward.Value += num * 0.5f;
			flag = true;
		}
		if (Keys.Held(35) || Pad.ComboHeld(4))
		{
			Mod.SeatForward.Value -= num * 0.5f;
			flag = true;
		}
		if (Keys.Held(45))
		{
			Mod.SeatRight.Value -= num * 0.5f;
			flag = true;
		}
		if (Keys.Held(46))
		{
			Mod.SeatRight.Value += num * 0.5f;
			flag = true;
		}
		if (flag)
		{
			seatSaveAt = Time.unscaledTime + 1f;
		}
		if (seatSaveAt > 0f && Time.unscaledTime > seatSaveAt)
		{
			seatSaveAt = 0f;
			MelonPreferences.Save();
			L.Msg($"Seat offset saved: up {Mod.SeatUp.Value:0.00} fwd {Mod.SeatForward.Value:0.00} right {Mod.SeatRight.Value:0.00}");
		}
	}

	internal void LateUpdate()
	{
		SafeFrame();
	}

	private void SafeFrame()
	{
		try
		{
			FrameUpdate();
		}
		catch (Exception e)
		{
			Fail(e);
		}
	}

	internal void Quit()
	{
		if (vrOn)
		{
			StopVr();
		}
		FfbStop();
	}

	private void Recenter()
	{
		needRecenter = true;
		seatValid = false;
		sPlaceScreen = true;
		sPlaceHud = true;
		cockpitSince = -1f;
		bonnetValid = false;
		bonnetSince = -1f;
		L.Msg("Recenter.");
	}

	private void StartVr()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Expected Obj, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Invalid comparison between Unknown and I4
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected Obj, but got Unknown
		if (vrOn)
		{
			return;
		}
		if ((int)SystemInfo.graphicsDeviceType != 2)
		{
			L.Error($"VR needs DirectX 11 but the game runs {SystemInfo.graphicsDeviceType}. Add -force-d3d11 to Steam launch options.");
			return;
		}
		EVRInitError peError = EVRInitError.None;
		sys = OpenVR.Init(ref peError);
		if (peError != EVRInitError.None || sys == null)
		{
			L.Error("SteamVR init failed: " + peError.ToString() + ". Is SteamVR running with the headset connected?");
			sys = null;
			return;
		}
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor == null)
		{
			L.Error("SteamVR compositor unavailable.");
			OpenVR.Shutdown();
			sys = null;
			return;
		}
		sys.GetRecommendedRenderTargetSize(ref eyeW, ref eyeH);
		float num = Mathf.Clamp(Mod.ResolutionScale.Value, 0.3f, 1.5f);
		eyeW = (uint)Math.Max(256f, (float)eyeW * num);
		eyeH = (uint)Math.Max(256f, (float)eyeH * num);
		for (int i = 0; i < 2; i++)
		{
			EVREye eEye = ((i != 0) ? EVREye.Eye_Right : EVREye.Eye_Left);
			sys.GetProjectionRaw(eEye, ref raw[i][0], ref raw[i][1], ref raw[i][2], ref raw[i][3]);
			HmdMatrix34_t eyeToHeadTransform = sys.GetEyeToHeadTransform(eEye);
			eyeOffset[i] = VMath.PosOf(eyeToHeadTransform);
			eyeRotOffset[i] = VMath.RotOf(eyeToHeadTransform);
			RenderTexture val = new RenderTexture((int)eyeW, (int)eyeH, 24, (RenderTextureFormat)0, (RenderTextureReadWrite)0);
			((Object)val).name = "OTHVR Eye " + i;
			val.antiAliasing = 1;
			((Object)val).hideFlags = (HideFlags)32;
			if (!val.Create())
			{
				throw new Exception("Eye texture allocation failed.");
			}
			eyeRT[i] = val;
			Texture2D val2 = new Texture2D((int)eyeW, (int)eyeH, (TextureFormat)4, false, (int)QualitySettings.activeColorSpace == 0);
			((Object)val2).name = "OTHVR Submit " + i;
			((Object)val2).hideFlags = (HideFlags)32;
			val2.Apply(false, true);
			eyeCopy[i] = val2;
		}
		UpdateBounds();
		modeConfirmed = false;
		ApplyMode(0);
		sSubmitError = 0;
		loggedSubmitError = 0;
		try
		{
			sOverlay = OpenVR.Overlay;
		}
		catch
		{
			sOverlay = null;
		}
		sScreenWidth = Mod.ScreenWidth.Value;
		sScreenDist = Mod.ScreenDistance.Value;
		sScreenFlip = Mod.FlipScreen.Value;
		sHudWidth = Mod.HudWidth.Value;
		sHudDist = Mod.HudDistance.Value;
		sHudDown = Mod.HudDown.Value;
		lock (sCallLock)
		{
			sWaited = false;
			sSys = sys;
			sComp = compositor;
			sOvHandle = 0uL;
			sPlaceScreen = true;
		}
		vrOn = true;
		needRecenter = true;
		seatValid = false;
		srcPtr = IntPtr.Zero;
		L.Msg($"VR on: {eyeW}x{eyeH} per eye, color space {QualitySettings.activeColorSpace}, GPU {SystemInfo.graphicsDeviceName}.");
	}

	private void StopVr()
	{
		bool flag = vrOn;
		vrOn = false;
		frameReady = false;
		lock (sCallLock)
		{
			try
			{
				if (sOverlay != null && sOvHandle != 0L)
				{
					sOverlay.DestroyOverlay(sOvHandle);
				}
			}
			catch
			{
			}
			try
			{
				if (sOverlay != null && sHudHandle != 0L)
				{
					sOverlay.DestroyOverlay(sHudHandle);
				}
			}
			catch
			{
			}
			sOvHandle = 0uL;
			sHudHandle = 0uL;
			sHudTex = IntPtr.Zero;
			sComp = null;
			sSys = null;
			sWaited = false;
		}
		try
		{
			RestoreHud();
		}
		catch (Exception ex)
		{
			L.Warning(ex.Message);
		}
		try
		{
			ClearDress();
			DestroyMirrors();
			SetTheaterActive(on: false);
		}
		catch (Exception ex2)
		{
			L.Warning(ex2.Message);
		}
		try
		{
			RestoreVrBlur();
			ApplyVrPerf(on: false);
		}
		catch
		{
		}
		if ((Object)(object)hudCam != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)hudCam).gameObject);
			hudCam = null;
		}
		if ((Object)(object)hudRT != (Object)null)
		{
			hudRT.Release();
			Object.Destroy((Object)(object)hudRT);
			hudRT = null;
		}
		hudShown = false;
		try
		{
			DestroyEyes();
		}
		catch (Exception ex3)
		{
			L.Warning(ex3.Message);
		}
		for (int i = 0; i < 2; i++)
		{
			if ((Object)(object)eyeRT[i] != (Object)null)
			{
				eyeRT[i].Release();
				Object.Destroy((Object)(object)eyeRT[i]);
				eyeRT[i] = null;
			}
			if ((Object)(object)eyeCopy[i] != (Object)null)
			{
				Object.Destroy((Object)(object)eyeCopy[i]);
				eyeCopy[i] = null;
			}
			sTex[i] = IntPtr.Zero;
		}
		if ((Object)(object)screenRT != (Object)null)
		{
			screenRT.Release();
			Object.Destroy((Object)(object)screenRT);
			screenRT = null;
		}
		if ((Object)(object)screenTex != (Object)null)
		{
			Object.Destroy((Object)(object)screenTex);
			screenTex = null;
		}
		sScreenTex = IntPtr.Zero;
		screenShown = false;
		if (sys != null)
		{
			lock (sCallLock)
			{
				try
				{
					OpenVR.Shutdown();
				}
				catch
				{
				}
			}
			sys = null;
		}
		if (flag)
		{
			L.Msg("VR off. Game camera restored.");
		}
	}

	private void Fail(Exception e)
	{
		L.Error(e.ToString());
		if (vrOn)
		{
			StopVr();
		}
	}

	private void FrameUpdate()
	{
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_0940: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0951: Unknown result type (might be due to invalid IL or missing references)
		//IL_095b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0960: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		if (!vrOn || Time.frameCount == lastFrame)
		{
			return;
		}
		lastFrame = Time.frameCount;
		ReadPoses();
		Camera val = PickCamera();
		bool flag = DetectCockpit(val);
		inCockpit = ((Object)(object)val != (Object)null) & flag;
		if (Mod.DebugLog.Value && Time.unscaledTime >= nextStatus)
		{
			nextStatus = Time.unscaledTime + 5f;
			MelonLogger.Instance l = L;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(124, 13);
			defaultInterpolatedStringHandler.AppendLiteral("Status: camera '");
			defaultInterpolatedStringHandler.AppendFormatted(((Object)(object)val != (Object)null) ? ((Object)val).name : "NONE");
			defaultInterpolatedStringHandler.AppendLiteral("' (main ");
			defaultInterpolatedStringHandler.AppendFormatted(((Object)(object)Camera.main != (Object)null) ? ((Object)Camera.main).name : "none");
			defaultInterpolatedStringHandler.AppendLiteral("), cockpit ");
			defaultInterpolatedStringHandler.AppendFormatted(flag);
			defaultInterpolatedStringHandler.AppendLiteral(", car ");
			defaultInterpolatedStringHandler.AppendFormatted(((Object)(object)body != (Object)null) ? ((Object)body).name : "none");
			defaultInterpolatedStringHandler.AppendLiteral(", stereo ");
			defaultInterpolatedStringHandler.AppendFormatted(stereoView);
			defaultInterpolatedStringHandler.AppendLiteral(", hmd ");
			defaultInterpolatedStringHandler.AppendFormatted(hmdValid);
			defaultInterpolatedStringHandler.AppendLiteral(", frames ok ");
			defaultInterpolatedStringHandler.AppendFormatted(sOk);
			defaultInterpolatedStringHandler.AppendLiteral(", view ");
			defaultInterpolatedStringHandler.AppendFormatted(viewLevel);
			defaultInterpolatedStringHandler.AppendLiteral(", mode ");
			defaultInterpolatedStringHandler.AppendFormatted(forceMode);
			defaultInterpolatedStringHandler.AppendLiteral(", cam pos ");
			string value;
			Vector3 position;
			if (!((Object)(object)val != (Object)null))
			{
				value = "-";
			}
			else
			{
				position = ((Component)val).transform.position;
				value = position.ToString("F0");
			}
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral(", car pos ");
			string value2;
			if (!((Object)(object)body != (Object)null))
			{
				value2 = "-";
			}
			else
			{
				position = body.position;
				value2 = position.ToString("F0");
			}
			defaultInterpolatedStringHandler.AppendFormatted(value2);
			defaultInterpolatedStringHandler.AppendLiteral(", fps ");
			defaultInterpolatedStringHandler.AppendFormatted(1f / Mathf.Max(0.001f, Time.unscaledDeltaTime), "0");
			defaultInterpolatedStringHandler.AppendLiteral(", hud canvases ");
			defaultInterpolatedStringHandler.AppendFormatted(hudCanvases.Count);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			l.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		bool flag2 = (Object)(object)body != (Object)null && (Object)(object)val != (Object)null && Vector3.Distance(((Component)val).transform.position, body.position) > Mod.LeaveCarDistance.Value;
		bool flag3 = (Object)(object)body != (Object)null && !flag2;
		if (flag2)
		{
			inCockpit = false;
		}
		bool flag4 = (Object)(object)val != (Object)null && (inCockpit || (Mod.ChaseCamInVR.Value & flag3));
		if (forceMode == 1)
		{
			flag4 = (Object)(object)val != (Object)null;
		}
		else if (forceMode == 2)
		{
			flag4 = false;
		}
		if (flag4 != stereoView)
		{
			stereoView = flag4;
			if (Mod.DebugLog.Value)
			{
				L.Msg(flag4 ? ("Stereo view on (" + (inCockpit ? "cockpit" : "chase") + ").") : "Theatre screen on.");
			}
			needRecenter = true;
			if (flag4)
			{
				seatValid = false;
			}
		}
		try
		{
			if (dressFailed <= 20 && (Object)(object)val != (Object)null && (Object)(object)body != (Object)null)
			{
				UpdateDress(val);
			}
		}
		catch (Exception ex)
		{
			if (dressFailed++ < 5)
			{
				L.Warning("dress: " + ex);
			}
			try
			{
				ClearDress();
			}
			catch
			{
			}
		}
		if (!flag4)
		{
			if (hudCanvases.Count > 0)
			{
				RestoreHud();
			}
			SetEyesActive(on: false);
			Unmute();
			frameReady = false;
			TheaterFrame();
			return;
		}
		SetTheaterActive(on: false);
		if (((Il2CppObjectBase)(object)val).Pointer != srcPtr)
		{
			SetupEyes(val);
		}
		SetEyesActive(on: true);
		SyncMute();
		Vector3 a;
		Quaternion a2;
		if (inCockpit && (Object)(object)body != (Object)null)
		{
			if (!seatValid)
			{
				seatLocal = SeatEye;
				seatValid = true;
			}
			Vector3 val2 = VMath.Add(seatValid ? seatLocal : body.InverseTransformPoint(((Component)val).transform.position), VMath.V(Mod.SeatRight.Value, Mod.SeatUp.Value, Mod.SeatForward.Value));
			a = body.TransformPoint(val2);
			a2 = (Mod.FollowCarPitchRoll.Value ? body.rotation : VMath.Yaw(VMath.YawOf(body.rotation)));
		}
		else if ((Object)(object)body != (Object)null && viewLevel >= 1 && !flag2)
		{
			cockpitSince = -1f;
			seatWarned = false;
			chaseInit = false;
			if (!bonnetValid)
			{
				bonnetLocal = BonnetPoint();
				bonnetValid = true;
				if (Mod.DebugLog.Value)
				{
					L.Msg($"Bonnet view at car-local {bonnetLocal.x:0.00}, {bonnetLocal.y:0.00}, {bonnetLocal.z:0.00}.");
				}
			}
			a = body.TransformPoint(bonnetLocal + VMath.V(Mod.SeatRight.Value, Mod.SeatUp.Value, Mod.SeatForward.Value));
			a2 = (Mod.FollowCarPitchRoll.Value ? body.rotation : VMath.Yaw(VMath.YawOf(body.rotation)));
		}
		else if ((Object)(object)body != (Object)null && Mod.VrChaseCam.Value)
		{
			cockpitSince = -1f;
			seatWarned = false;
			bonnetSince = -1f;
			bonnetValid = false;
			float num = VMath.YawOf(body.rotation);
			Vector3 val3 = VMath.Sub(((Component)val).transform.position, body.position);
			float num2 = Mathf.Clamp((float)Math.Sqrt(val3.x * val3.x + val3.z * val3.z), 2f, 60f);
			float num3 = Mathf.Clamp(val3.y, 0.6f, 40f);
			if (!chaseInit)
			{
				chaseYaw = num;
				chaseDist = num2;
				chaseH = num3;
				chaseInit = true;
			}
			float num4 = 1f - Mathf.Exp((0f - Mod.ChaseTurnSpeed.Value) * Time.unscaledDeltaTime);
			float num5 = 1f - Mathf.Exp(-4f * Time.unscaledDeltaTime);
			chaseYaw += Mathf.DeltaAngle(chaseYaw, num) * num4;
			chaseDist += (num2 - chaseDist) * num5;
			chaseH += (num3 - chaseH) * num5;
			Quaternion val4 = VMath.Yaw(chaseYaw);
			a = VMath.Add(body.position, VMath.Rotate(val4, VMath.V(0f, chaseH, 0f - chaseDist)));
			a2 = val4;
		}
		else
		{
			cockpitSince = -1f;
			seatWarned = false;
			chaseInit = false;
			a = ((Component)val).transform.position;
			a2 = VMath.Yaw(VMath.YawOf(((Component)val).transform.rotation));
		}
		if (needRecenter && hmdValid)
		{
			needRecenter = false;
			origin = hmdPos;
			originYaw = VMath.YawOf(hmdRot);
		}
		Quaternion val5 = VMath.Mul(a2, VMath.Yaw(0f - originYaw));
		Vector3 a3 = VMath.Add(a, VMath.Rotate(val5, VMath.Sub(hmdPos, origin)));
		Quaternion val6 = VMath.Mul(val5, hmdRot);
		float num6 = Mathf.Max(0.01f, Mod.NearClip.Value);
		float num7 = Mathf.Max(num6 + 1f, val.farClipPlane);
		for (int i = 0; i < 2; i++)
		{
			Vector3 val7 = VMath.Add(a3, VMath.Rotate(val6, eyeOffset[i]));
			Quaternion val8 = VMath.Mul(val6, eyeRotOffset[i]);
			Camera val9 = eyes[i];
			if (!((Object)(object)val9 == (Object)null))
			{
				((Component)val9).transform.SetPositionAndRotation(val7, val8);
				val9.nearClipPlane = num6;
				val9.farClipPlane = num7;
				val9.projectionMatrix = VMath.Projection(raw[i][0], raw[i][1], raw[i][2], raw[i][3], num6, num7);
				val9.cullingMask = val.cullingMask & -33;
			}
		}
		if (Mod.HudInVR.Value && hudOn)
		{
			UpdateHud();
		}
		else if (hudCanvases.Count > 0)
		{
			RestoreHud();
		}
		if (Time.frameCount % 90 == 0)
		{
			lock (sCallLock)
			{
				RefreshHandles();
			}
		}
		frameReady = true;
	}

	private void TakeShot()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected Obj, but got Unknown
		//IL_00bf: Expected Obj, but got Unknown
		shotPending = false;
		try
		{
			RenderTexture val = ((stereoView && (Object)(object)eyeRT[0] != (Object)null) ? eyeRT[0] : screenRT);
			if ((Object)(object)val == (Object)null)
			{
				L.Warning("Screenshot: nothing to capture yet.");
				return;
			}
			string text = System.IO.Path.Combine(MelonEnvironment.UserDataDirectory, "OTHVR_shots");
			Directory.CreateDirectory(text);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = val;
			Texture2D val2 = new Texture2D(((Texture)val).width, ((Texture)val).height, (TextureFormat)3, false);
			val2.ReadPixels(new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), 0, 0);
			val2.Apply();
			RenderTexture.active = active;
			Il2CppStructArray<byte> il2CppStructArray = ImageConversion.EncodeToPNG(val2);
			Object.Destroy((Object)val2);
			string text2 = System.IO.Path.Combine(text, "shot_" + DateTime.Now.ToString("HHmmss") + ((shotName != null) ? ("_" + shotName) : "") + ".png");
			shotName = null;
			File.WriteAllBytes(text2, il2CppStructArray);
			L.Msg("Screenshot saved: " + text2);
		}
		catch (Exception ex)
		{
			L.Warning("Screenshot failed: " + ex.Message);
		}
	}

	private Camera PickCamera()
	{
		Camera val = null;
		try
		{
			val = Camera.main;
		}
		catch
		{
		}
		if ((Object)(object)val != (Object)null && ((Behaviour)val).isActiveAndEnabled && (Object)(object)val.targetTexture == (Object)null)
		{
			return val;
		}
		Camera val2 = null;
		try
		{
			foreach (Camera allCamera in Camera.allCameras)
			{
				if (!((Object)(object)allCamera == (Object)null) && !((Object)(object)allCamera.targetTexture != (Object)null) && ((Behaviour)allCamera).isActiveAndEnabled && !((Object)allCamera).name.StartsWith("OTHVR") && ((Object)(object)val2 == (Object)null || allCamera.depth > val2.depth))
				{
					val2 = allCamera;
				}
			}
		}
		catch
		{
		}
		return val2;
	}


	private void CheckGuardianRecenter()
	{
		if (!hmdValid) return;
		if (prevRawValid)
		{
			float d = Vector3.Distance(hmdPos, prevRawPos);
			if (d > 0.25f)
			{
				needRecenter = true;
				seatValid = false;
				bonnetValid = false;
				L.Msg($"VR: External Guardian recenter detected ({d:0.00}m jump). Auto-aligning cockpit view.");
			}
		}
		prevRawPos = hmdPos;
		prevRawValid = true;
	}

	private void ReadPoses()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		lock (sDataLock)
		{
			Array.Copy(sPoses, renderPoses, sPoses.Length);
		}
		TrackedDevicePose_t trackedDevicePose_t = renderPoses[0];
		hmdValid = trackedDevicePose_t.bPoseIsValid;
		renderSlot = (renderSlot + 1) & 3;
		sSlotPose[renderSlot] = trackedDevicePose_t.mDeviceToAbsoluteTracking;
		sSlotValid[renderSlot] = hmdValid;
		if (hmdValid)
		{
			hmdPos = VMath.PosOf(trackedDevicePose_t.mDeviceToAbsoluteTracking);
			hmdRot = VMath.RotOf(trackedDevicePose_t.mDeviceToAbsoluteTracking);
		}
		CheckGuardianRecenter();
	}

	private IEnumerator EndOfFrameLoop()
	{
		while (true)
		{
			yield return (object)new WaitForEndOfFrame();
			lastCoroutineFrame = Time.frameCount;
			if (!fallbackEndHook)
			{
				EndOfFrame();
			}
		}
	}

	private void EndOfFrame()
	{
		if (shotPending && vrOn)
		{
			TakeShot();
		}
		if (!vrOn || Time.frameCount == lastEndFrame)
		{
			return;
		}
		lastEndFrame = Time.frameCount;
		try
		{
			if (stereoView && frameReady)
			{
				if (screenShown)
				{
					screenShown = false;
					GL.IssuePluginEvent(sRenderFn, 4);
				}
				if (Modes[mode].copy)
				{
					for (int i = 0; i < 2; i++)
					{
						if ((Object)(object)eyeRT[i] != (Object)null && (Object)(object)eyeCopy[i] != (Object)null)
						{
							Graphics.CopyTexture((Texture)(object)eyeRT[i], (Texture)(object)eyeCopy[i]);
						}
					}
				}
				if ((Object)(object)hudRT != (Object)null && hudCanvases.Count > 0)
				{
					sHudTex = ((Texture)hudRT).GetNativeTexturePtr();
					GL.IssuePluginEvent(sRenderFn, 5);
					hudShown = true;
				}
				else if (hudShown)
				{
					hudShown = false;
					GL.IssuePluginEvent(sRenderFn, 6);
				}
				GL.IssuePluginEvent(sRenderFn, 10 + renderSlot);
				frameReady = false;
			}
			else if (!stereoView && theaterActive && frameReady)
			{
				if (hudShown)
				{
					hudShown = false;
					GL.IssuePluginEvent(sRenderFn, 6);
				}
				if (screenShown)
				{
					screenShown = false;
					GL.IssuePluginEvent(sRenderFn, 4);
				}
				EnsureScreenTex();
				ScreenCapture.CaptureScreenshotIntoRenderTexture(screenRT);
				if (Modes[mode].copy)
				{
					for (int j = 0; j < 2; j++)
					{
						if ((Object)(object)eyeRT[j] != (Object)null && (Object)(object)eyeCopy[j] != (Object)null)
						{
							Graphics.CopyTexture((Texture)(object)eyeRT[j], (Texture)(object)eyeCopy[j]);
						}
					}
				}
				GL.IssuePluginEvent(sRenderFn, 10 + renderSlot);
				frameReady = false;
			}
			else if (!stereoView)
			{
				if (hudShown)
				{
					hudShown = false;
					GL.IssuePluginEvent(sRenderFn, 6);
				}
				EnsureScreenTex();
				ScreenCapture.CaptureScreenshotIntoRenderTexture(screenRT);
				Graphics.CopyTexture((Texture)(object)screenRT, (Texture)(object)screenTex);
				screenShown = true;
				GL.IssuePluginEvent(sRenderFn, 3);
			}
			else
			{
				GL.IssuePluginEvent(sRenderFn, 2);
			}
		}
		catch (Exception ex)
		{
			L.Warning("end of frame: " + ex.Message);
		}
	}

	private void EnsureScreenTex()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected Obj, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected Obj, but got Unknown
		int num = Math.Max(640, Screen.width);
		int num2 = Math.Max(360, Screen.height);
		if ((Object)(object)screenRT != (Object)null && ((Texture)screenRT).width == num && ((Texture)screenRT).height == num2)
		{
			return;
		}
		if ((Object)(object)screenRT != (Object)null)
		{
			screenRT.Release();
			Object.Destroy((Object)(object)screenRT);
		}
		if ((Object)(object)screenTex != (Object)null)
		{
			Object.Destroy((Object)(object)screenTex);
		}
		screenRT = new RenderTexture(num, num2, 0, (RenderTextureFormat)0);
		screenRT.Create();
		screenTex = new Texture2D(num, num2, (TextureFormat)4, false, (int)QualitySettings.activeColorSpace == 0);
		screenTex.Apply(false, true);
		sScreenTex = ((Texture)screenTex).GetNativeTexturePtr();
		lock (sCallLock)
		{
			if (sOverlay != null && sOvHandle != 0L)
			{
				sOverlay.DestroyOverlay(sOvHandle);
				sOvHandle = 0uL;
			}
			sPlaceScreen = true;
		}
	}

	private static void OnRenderEvent(int id)
	{
		lock (sCallLock)
		{
			CVRCompositor cVRCompositor = sComp;
			CVRSystem cVRSystem = sSys;
			if (cVRCompositor == null || cVRSystem == null)
			{
				return;
			}
			try
			{
				if (id >= 10 && sWaited && sTex[0] != IntPtr.Zero && sTex[1] != IntPtr.Zero)
				{
					int num = (id - 10) & 3;
					bool flag = sSlotValid[num];
					for (int i = 0; i < 2; i++)
					{
						VRTextureWithPose_t source = new VRTextureWithPose_t
						{
							handle = sTex[i],
							eType = ETextureType.DirectX,
							eColorSpace = sColor,
							mDeviceToAbsoluteTracking = sSlotPose[num]
						};
						VRTextureBounds_t pBounds = sBounds;
						EVRCompositorError eVRCompositorError = cVRCompositor.Submit((i != 0) ? EVREye.Eye_Right : EVREye.Eye_Left, ref Unsafe.As<VRTextureWithPose_t, Texture_t>(ref source), ref pBounds, flag ? EVRSubmitFlags.Submit_TextureWithPose : EVRSubmitFlags.Submit_Default);
						if (eVRCompositorError != EVRCompositorError.None)
						{
							sSubmitError = (int)eVRCompositorError;
							Interlocked.Increment(ref sErr);
						}
						else
						{
							Interlocked.Increment(ref sOk);
						}
					}
					cVRCompositor.PostPresentHandoff();
				}
				if (id == 3 && sOverlay != null && sScreenTex != IntPtr.Zero)
				{
					ScreenOverlay(cVRCompositor);
				}
				if (id == 4 && sOverlay != null && sOvHandle != 0L)
				{
					sOverlay.HideOverlay(sOvHandle);
				}
				switch (id)
				{
				case 5:
					if (sOverlay != null && sHudTex != IntPtr.Zero)
					{
						HudOverlay(cVRCompositor);
					}
					return;
				case 6:
					if (sOverlay != null && sHudHandle != 0L)
					{
						sOverlay.HideOverlay(sHudHandle);
					}
					return;
				}
				cVRCompositor.WaitGetPoses(rtPoses, rtNoGame);
				sWaited = true;
				lock (sDataLock)
				{
					Array.Copy(rtPoses, sPoses, rtPoses.Length);
				}
			}
			catch
			{
				sSubmitError = -1;
			}
		}
	}

	private static void HudOverlay(CVRCompositor c)
	{
		if (sHudHandle == 0L)
		{
			ulong pOverlayHandle = 0uL;
			if (sOverlay.CreateOverlay("othvr.hud", "Over The Hill HUD", ref pOverlayHandle) != EVROverlayError.None)
			{
				return;
			}
			sHudHandle = pOverlayHandle;
			sOverlay.SetOverlayWidthInMeters(sHudHandle, sHudWidth);
			sOverlay.SetOverlaySortOrder(sHudHandle, 10u);
			VRTextureBounds_t pOverlayTextureBounds = new VRTextureBounds_t
			{
				uMin = 0f,
				uMax = 1f,
				vMin = 1f,
				vMax = 0f
			};
			sOverlay.SetOverlayTextureBounds(sHudHandle, ref pOverlayTextureBounds);
			sPlaceHud = true;
		}
		TrackedDevicePose_t trackedDevicePose_t = rtPoses[0];
		if (sPlaceHud && trackedDevicePose_t.bPoseIsValid)
		{
			HmdMatrix34_t mDeviceToAbsoluteTracking = trackedDevicePose_t.mDeviceToAbsoluteTracking;
			float num = 0f - mDeviceToAbsoluteTracking.m2;
			float num2 = 0f - mDeviceToAbsoluteTracking.m10;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			if (num3 > 0.001f)
			{
				num /= num3;
				num2 /= num3;
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform = new HmdMatrix34_t
				{
					m0 = 0f - num2,
					m1 = 0f,
					m2 = 0f - num,
					m3 = mDeviceToAbsoluteTracking.m3 + num * sHudDist,
					m4 = 0f,
					m5 = 1f,
					m6 = 0f,
					m7 = mDeviceToAbsoluteTracking.m7 - sHudDown,
					m8 = num,
					m9 = 0f,
					m10 = 0f - num2,
					m11 = mDeviceToAbsoluteTracking.m11 + num2 * sHudDist
				};
				sOverlay.SetOverlayTransformAbsolute(sHudHandle, c.GetTrackingSpace(), ref pmatTrackingOriginToOverlayTransform);
				sPlaceHud = false;
			}
		}
		Texture_t pTexture = new Texture_t
		{
			handle = sHudTex,
			eType = ETextureType.DirectX,
			eColorSpace = EColorSpace.Auto
		};
		sOverlay.SetOverlayTexture(sHudHandle, ref pTexture);
		sOverlay.ShowOverlay(sHudHandle);
	}

	private static void ScreenOverlay(CVRCompositor c)
	{
		if (sOvHandle == 0L)
		{
			ulong pOverlayHandle = 0uL;
			if (sOverlay.CreateOverlay("othvr.screen", "Over The Hill", ref pOverlayHandle) != EVROverlayError.None)
			{
				return;
			}
			sOvHandle = pOverlayHandle;
			sOverlay.SetOverlayWidthInMeters(sOvHandle, sScreenWidth);
			VRTextureBounds_t pOverlayTextureBounds = new VRTextureBounds_t
			{
				uMin = 0f,
				uMax = 1f,
				vMin = (sScreenFlip ? 1 : 0),
				vMax = ((!sScreenFlip) ? 1 : 0)
			};
			sOverlay.SetOverlayTextureBounds(sOvHandle, ref pOverlayTextureBounds);
			sPlaceScreen = true;
		}
		TrackedDevicePose_t trackedDevicePose_t = rtPoses[0];
		if (sPlaceScreen && trackedDevicePose_t.bPoseIsValid)
		{
			HmdMatrix34_t mDeviceToAbsoluteTracking = trackedDevicePose_t.mDeviceToAbsoluteTracking;
			float num = 0f - mDeviceToAbsoluteTracking.m2;
			float num2 = 0f - mDeviceToAbsoluteTracking.m10;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			if (num3 > 0.001f)
			{
				num /= num3;
				num2 /= num3;
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform = new HmdMatrix34_t
				{
					m0 = 0f - num2,
					m1 = 0f,
					m2 = 0f - num,
					m3 = mDeviceToAbsoluteTracking.m3 + num * sScreenDist,
					m4 = 0f,
					m5 = 1f,
					m6 = 0f,
					m7 = mDeviceToAbsoluteTracking.m7,
					m8 = num,
					m9 = 0f,
					m10 = 0f - num2,
					m11 = mDeviceToAbsoluteTracking.m11 + num2 * sScreenDist
				};
				sOverlay.SetOverlayTransformAbsolute(sOvHandle, c.GetTrackingSpace(), ref pmatTrackingOriginToOverlayTransform);
				sPlaceScreen = false;
			}
		}
		Texture_t pTexture = new Texture_t
		{
			handle = sScreenTex,
			eType = ETextureType.DirectX,
			eColorSpace = EColorSpace.Auto
		};
		sOverlay.SetOverlayTexture(sOvHandle, ref pTexture);
		sOverlay.ShowOverlay(sOvHandle);
	}

	private void RefreshHandles()
	{
		bool item = Modes[mode].copy;
		for (int i = 0; i < 2; i++)
		{
			if (!((Object)(object)eyeRT[i] == (Object)null))
			{
				if (!eyeRT[i].IsCreated())
				{
					eyeRT[i].Create();
				}
				IntPtr intPtr = ((item && (Object)(object)eyeCopy[i] != (Object)null) ? ((Texture)eyeCopy[i]).GetNativeTexturePtr() : ((Texture)eyeRT[i]).GetNativeTexturePtr());
				if (intPtr != IntPtr.Zero)
				{
					sTex[i] = intPtr;
				}
			}
		}
		sColor = Modes[mode].cs;
	}

	private void ApplyMode(int m)
	{
		mode = m;
		lock (sCallLock)
		{
			RefreshHandles();
		}
		Interlocked.Exchange(ref sOk, 0);
		Interlocked.Exchange(ref sErr, 0);
		nextModeCheck = Time.unscaledTime + 2f;
	}

	private void CheckSubmitMode()
	{
		if (Time.unscaledTime < nextModeCheck)
		{
			return;
		}
		nextModeCheck = Time.unscaledTime + 2f;
		int num = Interlocked.Exchange(ref sOk, 0);
		int num2 = Interlocked.Exchange(ref sErr, 0);
		if (num + num2 == 0)
		{
			return;
		}
		if (num2 == 0 || num > num2 * 20)
		{
			if (!modeConfirmed)
			{
				modeConfirmed = true;
				L.Msg("SteamVR accepting frames using " + Modes[mode].name + ".");
			}
		}
		else if (modeConfirmed)
		{
			L.Warning($"SteamVR errors ({num2}/{num + num2}): {(EVRCompositorError)sSubmitError}");
		}
		else
		{
			int num3 = (mode + 1) % Modes.Length;
			L.Warning($"SteamVR rejected {Modes[mode].name} ({(EVRCompositorError)sSubmitError}). Trying {Modes[num3].name}.");
			ApplyMode(num3);
		}
	}

	private void UpdateBounds()
	{
		bool value = Mod.FlipEyeY.Value;
		sBounds = new VRTextureBounds_t
		{
			uMin = 0f,
			uMax = 1f,
			vMin = (value ? 1 : 0),
			vMax = ((!value) ? 1 : 0)
		};
	}

	private void SetupEyes(Camera cam)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected Obj, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Invalid comparison between Unknown and I4
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		DestroyEyes();
		src = cam;
		srcPtr = ((Il2CppObjectBase)(object)cam).Pointer;
		UniversalAdditionalCameraData val = null;
		try
		{
			val = CameraExtensions.GetUniversalAdditionalCameraData(cam);
		}
		catch
		{
		}
		for (int i = 0; i < 2; i++)
		{
			GameObject val2 = new GameObject("OTHVR Eye " + i);
			Object.DontDestroyOnLoad((Object)(object)val2);
			Camera val3 = val2.AddComponent<Camera>();
			((Behaviour)val3).enabled = false;
			val3.CopyFrom(cam);
			try
			{
				val2.AddComponent<GPUICamera>();
				if (i == 0 && Mod.DebugLog.Value)
				{
					L.Msg("GPU Instancer camera added to VR eyes.");
				}
			}
			catch (Exception ex)
			{
				if (i == 0)
				{
					L.Warning("GPUICamera add failed: " + ex.Message);
				}
			}
			val3.stereoTargetEye = (StereoTargetEyeMask)0;
			val3.targetTexture = eyeRT[i];
			val3.depth = cam.depth - 20f + (float)i;
			val3.allowDynamicResolution = false;
			eyes[i] = val3;
			if ((Object)(object)val != (Object)null)
			{
				try
				{
					UniversalAdditionalCameraData universalAdditionalCameraData = CameraExtensions.GetUniversalAdditionalCameraData(val3);
					universalAdditionalCameraData.renderType = (CameraRenderType)0;
					bool value = Mod.PostProcessing.Value;
					universalAdditionalCameraData.renderPostProcessing = value && val.renderPostProcessing;
					universalAdditionalCameraData.antialiasing = (AntialiasingMode)((!value || (int)val.antialiasing == 3) ? 1 : ((int)val.antialiasing));
					universalAdditionalCameraData.renderShadows = val.renderShadows;
					universalAdditionalCameraData.requiresDepthTexture = val.requiresDepthTexture;
					universalAdditionalCameraData.requiresColorTexture = val.requiresColorTexture;
					universalAdditionalCameraData.volumeLayerMask = val.volumeLayerMask;
					universalAdditionalCameraData.volumeTrigger = (((Object)(object)val.volumeTrigger != (Object)null) ? val.volumeTrigger : ((Component)cam).transform);
					universalAdditionalCameraData.cameraStack.Clear();
				}
				catch (Exception ex2)
				{
					L.Warning("URP camera data: " + ex2.Message);
				}
			}
		}
		L.Msg($"VR eyes attached to camera '{((Object)cam).name}' (mask 0x{cam.cullingMask:X}).");
	}

	private void SyncMute()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (Mod.MirrorToDesktop.Value || (Object)(object)src == (Object)null)
		{
			Unmute();
			return;
		}
		if ((Object)(object)muted == (Object)null || ((Il2CppObjectBase)(object)muted).Pointer != ((Il2CppObjectBase)(object)src).Pointer)
		{
			Unmute();
			muted = src;
			mutedRect = src.rect;
		}
		Rect rect = src.rect;
		if (rect.width > 0.01f || rect.height > 0.01f)
		{
			mutedRect = rect;
			src.rect = new Rect(0f, 0f, 0.002f, 0.002f);
		}
	}

	private void Unmute()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)muted != (Object)null)
		{
			try
			{
				muted.rect = mutedRect;
			}
			catch
			{
			}
		}
		muted = null;
	}

	private void SetEyesActive(bool on)
	{
		for (int i = 0; i < 2; i++)
		{
			if ((Object)(object)eyes[i] != (Object)null && ((Behaviour)eyes[i]).enabled != on)
			{
				((Behaviour)eyes[i]).enabled = on;
			}
		}
	}

	private void DestroyEyes()
	{
		Unmute();
		for (int i = 0; i < 2; i++)
		{
			if ((Object)(object)eyes[i] != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)eyes[i]).gameObject);
			}
			eyes[i] = null;
		}
		src = null;
		srcPtr = IntPtr.Zero;
	}

	private void EnsureHudCam()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected Obj, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected Obj, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)hudCam != (Object)null && (Object)(object)hudRT != (Object)null)
		{
			return;
		}
		hudRT = new RenderTexture(1920, 1080, 24, (RenderTextureFormat)0);
		((Object)hudRT).name = "OTHVR HUD";
		((Object)hudRT).hideFlags = (HideFlags)32;
		hudRT.Create();
		GameObject val = new GameObject("OTHVR HUD Camera");
		Object.DontDestroyOnLoad((Object)(object)val);
		val.transform.position = VMath.V(0f, -20000f, 0f);
		hudCam = val.AddComponent<Camera>();
		hudCam.clearFlags = (CameraClearFlags)2;
		hudCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		hudCam.cullingMask = 32;
		hudCam.targetTexture = hudRT;
		hudCam.nearClipPlane = 0.01f;
		hudCam.farClipPlane = 10f;
		hudCam.depth = -100f;
		try
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = CameraExtensions.GetUniversalAdditionalCameraData(hudCam);
			universalAdditionalCameraData.renderPostProcessing = false;
			universalAdditionalCameraData.renderShadows = false;
			universalAdditionalCameraData.antialiasing = (AntialiasingMode)0;
		}
		catch
		{
		}
	}

	private void UpdateHud()
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Invalid comparison between Unknown and I4
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Invalid comparison between Unknown and I4
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if (Time.unscaledTime < nextHudScan)
		{
			return;
		}
		nextHudScan = Time.unscaledTime + 0.5f;
		EnsureHudCam();
		for (int num = hudCanvases.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)hudCanvases[num].c == (Object)null)
			{
				hudCanvases.RemoveAt(num);
			}
		}
		Object[] array;
		try
		{
			array = Object.FindObjectsOfType(Il2CppType.Of<Canvas>());
		}
		catch (Exception ex)
		{
			L.Warning("HUD scan: " + ex.Message);
			nextHudScan = float.MaxValue;
			return;
		}
		int num2 = 32;
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Canvas val = ((Il2CppObjectBase)(object)array2[i]).TryCast<Canvas>();
			if ((Object)(object)val == (Object)null || !val.isRootCanvas || (int)val.renderMode == 2)
			{
				continue;
			}
			if ((int)val.renderMode == 1 && (Object)(object)val.worldCamera != (Object)null && ((Il2CppObjectBase)(object)val.worldCamera).Pointer == ((Il2CppObjectBase)(object)hudCam).Pointer)
			{
				num2 |= 1 << ((Component)val).gameObject.layer;
				continue;
			}
			HudRec item = new HudRec
			{
				c = val,
				mode = val.renderMode,
				cam = val.worldCamera,
				plane = val.planeDistance
			};
			val.renderMode = (RenderMode)1;
			val.worldCamera = hudCam;
			val.planeDistance = 1f;
			hudCanvases.Add(item);
			num2 |= 1 << ((Component)val).gameObject.layer;
			if (Mod.DebugLog.Value && hudSeen.Add(((Il2CppObjectBase)(object)val).Pointer))
			{
				L.Msg($"HUD canvas '{((Object)val).name}' (layer {((Component)val).gameObject.layer}) moved to VR panel.");
			}
		}
		hudCam.cullingMask = num2;
		if (!((Behaviour)hudCam).enabled)
		{
			((Behaviour)hudCam).enabled = true;
		}
	}

	private void RestoreHud()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (HudRec hudCanvase in hudCanvases)
		{
			try
			{
				if (!((Object)(object)hudCanvase.c == (Object)null))
				{
					hudCanvase.c.renderMode = hudCanvase.mode;
					hudCanvase.c.worldCamera = hudCanvase.cam;
					hudCanvase.c.planeDistance = hudCanvase.plane;
				}
			}
			catch
			{
			}
		}
		hudCanvases.Clear();
		if ((Object)(object)hudCam != (Object)null)
		{
			((Behaviour)hudCam).enabled = false;
		}
		nextHudScan = 0f;
	}

	private static IntPtr SafePtr(Object o)
	{
		if (!(o == (Object)null))
		{
			return ((Il2CppObjectBase)(object)o).Pointer;
		}
		return IntPtr.Zero;
	}

	private static string Path(Transform t)
	{
		string text = ((Object)t).name;
		Transform parent = t.parent;
		while ((Object)(object)parent != (Object)null)
		{
			text = ((Object)parent).name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	private void SendConst(int v)
	{
		try
		{
			DIManager.UpdateConstantForceSimple(ffbDev, v);
			ffbLastConst = v;
		}
		catch
		{
		}
	}

	private void FfbTick()
	{
		ffbLastTick = Time.unscaledTime;
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		if (!Mod.FfbOn.Value)
		{
			if (ffbReady)
			{
				FfbStop();
			}
			return;
		}
		if (!ffbReady)
		{
			if (Time.unscaledTime >= ffbNextTry)
			{
				FfbConnect();
			}
			if (!ffbReady)
			{
				return;
			}
		}
		bool flag = (Object)(object)body != (Object)null && (Object)(object)bodyRb != (Object)null && Time.timeScale > 0.01f && Time.deltaTime > 0f;
		float num = 0f;
		float num2 = Mod.FfbDamper.Value;
		float value = Mod.FfbCentering.Value;
		if (flag)
		{
			float deltaTime = Time.deltaTime;
			VehicleBase val = carVb;
			float num3 = 1f;
			try
			{
				if ((Object)(object)val != (Object)null && val.wheelCount >= 2)
				{
					int num4 = 0;
					for (int i = 0; i < 2; i++)
					{
						if (val.wheelState[i].grounded)
						{
							num4++;
						}
					}
					num3 = (float)num4 / 2f;
				}
			}
			catch
			{
			}
			float num5 = Mathf.Clamp(CarSteer(), -1f, 1f);
			Vector3 val2 = body.TransformPoint(new Vector3(0f, 0.35f, Mod.FfbFrontAxleZ.Value));
			Vector3 val3 = body.InverseTransformDirection(bodyRb.GetPointVelocity(val2));
			float num6 = Mathf.Abs(val3.z);
			float num7 = Mathf.Clamp01(num6 / 8f);
			float num8 = ((num6 > 0.5f) ? (Mathf.Atan2(val3.x, num6) * 57.29578f) : 0f);
			float num9 = num5 * Mod.FfbMaxSteerDeg.Value * ((val3.z < 0f) ? (-1f) : 1f);
			float num10 = num8 - num9;
			float num11 = Mathf.Clamp(num10 / 7f, -1f, 1f);
			float num12 = 1f - 0.45f * Mathf.Clamp01((Mathf.Abs(num10) - 7f) / 20f);
			float num13 = num11 * num12 * Mathf.Pow(num7, 0.7f) * num3 * ((val3.z < 0f) ? 0.35f : 1f);
			float num14 = Vector3.Dot(bodyRb.angularVelocity, body.forward);
			Vector3 velocity = bodyRb.velocity;
			float num15 = 0f;
			if (ffbHavePrev)
			{
				num15 = Mathf.Clamp((0f - (num14 - ffbPrevRollRate) / deltaTime) * 0.012f, -0.5f, 0.5f) * Mathf.Clamp01(num6 / 3f) * Mod.FfbRoad.Value;
				Vector3 val4 = (velocity - ffbPrevVel) / deltaTime;
				float num16 = Vector3.Dot(val4, body.right);
				if (val4.magnitude > 35f)
				{
					ffbHit = Mathf.Clamp((0f - num16) / 40f, -1f, 1f) * Mod.FfbCollision.Value;
					ffbHitT = 0.18f;
				}
			}
			ffbPrevRollRate = num14;
			ffbPrevVel = velocity;
			ffbHavePrev = true;
			if (ffbHitT > 0f)
			{
				ffbHitT -= deltaTime;
			}
			else
			{
				ffbHit = 0f;
			}
			num = num13 * Mod.FfbAligning.Value + num15 + ffbHit;
			num2 += (1f - num7) * Mod.FfbParkedWeight.Value;
		}
		else
		{
			ffbHavePrev = false;
		}
		if (flag && Mod.FfbAutoDirection.Value && ffbDir == 0)
		{
			float num17 = CarSteer();
			Vector3 velocity2 = bodyRb.velocity;
			float magnitude = velocity2.magnitude;
			if (!(ffbTestT < 0f))
			{
				ffbTestT += Time.unscaledDeltaTime;
				if (ffbTestT < 0.35f)
				{
					SendConst(800);
					return;
				}
				SendConst(0);
				float num18 = num17 - ffbTestStart;
				ffbTestT = -1f;
				ffbTestNext = Time.unscaledTime + 8f;
				if (Mathf.Abs(num18) > 0.02f)
				{
					ffbDir = ((num18 > 0f) ? 1 : (-1));
					L.Msg($"FFB: direction checked ({((ffbDir > 0) ? "normal" : "inverted")}, wheel moved {num18:+0.00;-0.00}).");
				}
				else
				{
					L.Msg("FFB: direction check inconclusive (wheel held?), will retry.");
				}
				return;
			}
			if (magnitude < 1f && Mathf.Abs(num17) < 0.3f && Time.unscaledTime > ffbTestNext)
			{
				ffbTestT = 0f;
				ffbTestStart = num17;
			}
		}
		float num19 = 1f - Mathf.Exp(-25f * Time.unscaledDeltaTime);
		ffbConst += (num - ffbConst) * num19;
		int num20 = Mathf.RoundToInt(Mathf.Clamp(ffbConst * Mod.FfbStrength.Value, 0f - Mod.FfbMaxForce.Value, Mod.FfbMaxForce.Value) * 10000f) * ((!Mod.FfbInvert.Value) ? 1 : (-1)) * ((ffbDir >= 0) ? 1 : (-1));
		num20 = SafeFfb(num20, ffbLastConst, Time.unscaledDeltaTime, flag, GameFocused(), isDirectDrive);
		int num21 = Mathf.RoundToInt(Mathf.Clamp01(num2 * Mod.FfbStrength.Value) * 10000f);
		num21 = SafeDamper(num21, isDirectDrive, flag);
		int num22 = Mathf.RoundToInt(Mathf.Clamp01(value * Mod.FfbStrength.Value) * 10000f);
		try
		{
			bool flag2 = true;
			if (Math.Abs(num20 - ffbLastConst) >= 40 || (num20 == 0 && ffbLastConst != 0))
			{
				flag2 &= DIManager.UpdateConstantForceSimple(ffbDev, num20);
				ffbLastConst = num20;
			}
			if (Math.Abs(num21 - ffbLastDamper) >= 150)
			{
				flag2 &= DIManager.UpdateDamperSimple(ffbDev, num21);
				ffbLastDamper = num21;
			}
			if (Math.Abs(num22 - ffbLastSpring) >= 150)
			{
				flag2 &= DIManager.UpdateSpringSimple(ffbDev, 0u, 0, num22, num22, 10000u, 10000u);
				ffbLastSpring = num22;
			}
			if (!flag2 && ++ffbFails > 30)
			{
				L.Warning("Force feedback updates rejected; reconnecting.");
				FfbStop();
				ffbFails = 0;
				ffbNextTry = Time.unscaledTime + 2f;
			}
			else if (flag2)
			{
				ffbFails = 0;
			}
		}
		catch (Exception ex)
		{
			L.Warning("Force feedback: " + ex.Message);
			FfbStop();
			ffbNextTry = Time.unscaledTime + 5f;
		}
		if ((Mod.FfbLog.Value && Time.unscaledTime > ffbNextLog) & flag)
		{
			ffbNextLog = Time.unscaledTime + 5f;
			L.Msg($"FFB: force {num20}, damper {num21}, spring {num22}.");
		}
	}

	private void FfbConnect()
	{
		ffbNextTry = Time.unscaledTime + 5f;
		try
		{
			if (!WheelConnect())
			{
				return;
			}
			DeviceInfo val = wheelDev;
			if (wheelGotData)
			{
				bool value = false;
				try
				{
					value = DIManager.FFBCapable(val);
				}
				catch
				{
				}
				ffbTried = true;
				bool flag = DIManager.EnableFFBEffect(val, (FFBEffects)0);
				bool value2 = DIManager.EnableFFBEffect(val, (FFBEffects)8);
				bool value3 = DIManager.EnableFFBEffect(val, (FFBEffects)7);
				bool flag2 = DIManager.UpdateConstantForceSimple(val, 0);
				if (!flag)
				{
					flag = flag2;
				}
				ffbDev = val;
				ffbReady = flag;
				ffbLastConst = 99999;
				ffbLastDamper = -1;
				ffbLastSpring = -1;
				L.Msg($"FFB: {(flag ? "on" : "not available")} (reported capable {value}, constant {flag}, damper {value2}, spring {value3}).");
				if (!flag)
				{
					L.Warning("FFB: wheel refused force feedback. Close other wheel tools/mods and restart the game.");
				}
			}
		}
		catch (Exception ex)
		{
			L.Warning("FFB connect: " + ex.Message);
		}
	}


	private static int SafeFfb(int targetConst, int lastConst, float dt, bool driving, bool focused, bool isDD)
	{
		if (!driving || !focused) return 0;
		if (isDD) targetConst = (int)(targetConst * 0.65f);
		int baseline = (lastConst == 99999) ? 0 : lastConst;
		float maxDeltaF = 12000f * dt;
		if (maxDeltaF < 50f) maxDeltaF = 50f;
		int maxDelta = (int)maxDeltaF;
		int delta = targetConst - baseline;
		if (delta > maxDelta) targetConst = baseline + maxDelta;
		else if (delta < -maxDelta) targetConst = baseline - maxDelta;
		if (targetConst > 8500) targetConst = 8500;
		if (targetConst < -8500) targetConst = -8500;
		return targetConst;
	}

	private static int SafeDamper(int targetDamper, bool isDD, bool driving)
	{
		if (!driving) return 0;
		if (isDD && targetDamper < 1200) targetDamper = 1200;
		if (targetDamper > 10000) targetDamper = 10000;
		return targetDamper;
	}

	private void WatchdogCheck()
	{
		if (ffbReady && ffbDev != null)
		{
			if (Time.unscaledTime - ffbLastTick > 0.20f)
			{
				if (ffbLastConst != 0)
				{
					DIManager.UpdateConstantForceSimple(ffbDev, 0);
					ffbLastConst = 0;
				}
			}
		}
	}

	private void FfbStop()
	{
		try
		{
			if (ffbDev != null)
			{
				DIManager.UpdateConstantForceSimple(ffbDev, 0);
				DIManager.StopAllFFBEffects(ffbDev);
			}
		}
		catch
		{
		}
		ffbReady = false;
		ffbConst = 0f;
	}

	private bool WheelConnect()
	{
		if (wheelDev != null)
		{
			return true;
		}
		if (Time.unscaledTime < wheelNextTry)
		{
			return false;
		}
		wheelNextTry = Time.unscaledTime + 5f;
		try
		{
			if (!DIManager.IsInitialized)
			{
				DIManager.Initialize();
			}
			Il2CppReferenceArray<DeviceInfo> devices = DIManager.Devices;
			if (devices == null || devices.Length == 0)
			{
				DIManager.EnumerateDevices();
				devices = DIManager.Devices;
			}
			if (devices == null || devices.Length == 0)
			{
				if (!wheelTried)
				{
					L.Msg("Wheel: no DirectInput devices found (plug in the wheel and start G HUB).");
				}
				wheelTried = true;
				return false;
			}
			DeviceInfo val = null;
			int num = 0;
			for (int i = 0; i < devices.Length; i++)
			{
				DeviceInfo val2 = devices[i];
				if (val2 != null)
				{
					bool flag = false;
					try
					{
						flag = DIManager.FFBCapable(val2);
					}
					catch
					{
					}
					string text = (val2.productName + " " + val2.instanceName).Trim();
					int num2 = (flag ? 10 : 0) + ((text.Contains("G29") || text.Contains("G920") || text.Contains("G923") || text.Contains("CSL") || text.Contains("Fanatec")) ? 20 : 0) + ((text.IndexOf("wheel", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("racing", StringComparison.OrdinalIgnoreCase) >= 0) ? 8 : 0) - ((text.IndexOf("xbox", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("controller", StringComparison.OrdinalIgnoreCase) >= 0) ? 50 : 0);
					if (!wheelTried)
					{
						L.Msg($"Wheel: found '{text}' (force feedback {flag}).");
					}
					if (num2 > num)
					{
						num = num2;
						val = val2;
					}
				}
			}
			wheelTried = true;
			if (val == null)
			{
				return false;
			}
			if (!GameFocused())
			{
				if (vrOn || Time.unscaledTime > wheelNextFocus)
				{
					wheelNextFocus = Time.unscaledTime + 4f;
					FocusGame("wheel");
				}
				wheelNextTry = Time.unscaledTime + 1f;
				return false;
			}
			bool value = DIManager.IsDeviceActive(val) || DIManager.Attach(val);
			bool value2 = false;
			try
			{
				value2 = DIManager.IsDeviceActive(val);
			}
			catch
			{
			}
			wheelDev = val;
			wheelAttachedAt = Time.unscaledTime;
			wheelGotData = false;
			string devName = (val.productName + " " + val.instanceName).ToLowerInvariant();
			if (devName.Contains("fanatec") || devName.Contains("csl") || devName.Contains("clubsport") || devName.Contains("podium"))
			{
				ffbDir = 1;
				isDirectDrive = true;
				L.Msg("FFB: Fanatec Direct Drive profile active (direction normal, calibration kick loop bypassed, hardware safeties engaged).");
			}
			L.Msg($"Wheel: using '{val.productName}' (attached {value}, active {value2}).");
			return true;
		}
		catch (Exception ex)
		{
			L.Warning("Wheel connect: " + ex.Message);
			return false;
		}
	}

	private void DisableRawWheelDevices()
	{
		if (Time.unscaledTime < nextDevScan)
		{
			return;
		}
		nextDevScan = Time.unscaledTime + 1f;
		try
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			for (int i = 0; i < devices.Count; i++)
			{
				InputDevice val = devices[i];
				if (val != null && (vpad == null || val.deviceId != vpad.deviceId))
				{
					string text = ((InputControl)val).name ?? "";
					string text2 = ((InputControl)val).layout ?? "";
					string text3 = "";
					string text4 = "";
					try
					{
						text3 = val.description.product ?? "";
						text4 = val.description.interfaceName ?? "";
					}
					catch
					{
					}
					string text5 = (text + " " + text2 + " " + text3 + " " + text4).ToLowerInvariant();
					if (seenDevs.Add(val.deviceId))
					{
						L.Msg($"Input device: '{text}' layout '{text2}' product '{text3}' interface '{text4}' enabled {val.enabled}.");
					}
					if ((text5.Contains("g29") || text5.Contains("g920") || text5.Contains("g923") || text5.Contains("racing wheel") || text5.Contains("directinput") || text5.Contains("fanatec") || text5.Contains("csl") || text5.Contains("clubsport") || text5.Contains("podium")) && val.enabled)
					{
						InputSystem.DisableDevice(val, false);
						L.Msg("Wheel: switched off raw input device '" + text + "' (the mod handles the wheel).");
					}
				}
			}
		}
		catch (Exception ex)
		{
			_ = Time.unscaledTime;
			_ = 30f;
			L.Msg("device scan: " + ex.Message);
			nextDevScan = Time.unscaledTime + 10f;
		}
	}

	private void WheelTick()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		if (Mod.WheelSupport.Value)
		{
			DisableRawWheelDevices();
		}
		if (!Mod.WheelSupport.Value || wheelErrors > 20)
		{
			WheelPad.Idle();
			SendNeutral();
			return;
		}
		if (!WheelConnect())
		{
			WheelPad.Idle();
			SendNeutral();
			return;
		}
		FlatJoyState2 deviceState;
		try
		{
			DIManager.Poll(wheelDev);
			deviceState = DIManager.GetDeviceState(wheelDev);
		}
		catch (Exception ex)
		{
			if (++wheelErrors <= 3)
			{
				L.Warning("Wheel read: " + ex.Message);
			}
			WheelPad.Idle();
			SendNeutral();
			return;
		}
		if (deviceState.lX == 0 && deviceState.lY == 0 && deviceState.lRz == 0 && deviceState.buttonsA == 0)
		{
			WheelPad.Idle();
			SendNeutral();
			if (GameFocused() && Time.unscaledTime - wheelAttachedAt > 3f && wheelReattach < 10)
			{
				wheelReattach++;
				L.Msg($"Wheel: no data yet, reattaching (try {wheelReattach}).");
				try
				{
					DIManager.Destroy(wheelDev);
				}
				catch
				{
				}
				try
				{
					DIManager.Attach(wheelDev);
				}
				catch
				{
				}
				wheelAttachedAt = Time.unscaledTime;
				ffbReady = false;
				ffbNextTry = 0f;
			}
			else if (!GameFocused() && Mod.AutoFocus.Value && Time.unscaledTime > wheelNextFocus)
			{
				wheelNextFocus = Time.unscaledTime + 4f;
				FocusGame("wheel");
			}
			return;
		}
		if (!wheelGotData)
		{
			wheelGotData = true;
			L.Msg($"Wheel: reading input (steer {deviceState.lX}, gas {deviceState.lY}, brake {deviceState.lRz}).");
		}
		if (Mod.WheelButtonLog.Value && Time.unscaledTime > wheelNextAxisLog && (deviceState.lX != lastAx[0] || deviceState.lY != lastAx[1] || deviceState.lRz != lastAx[2] || deviceState.rgdwPOV != lastPov) && Time.unscaledTime - wheelAttachedAt < 60f)
		{
			wheelNextAxisLog = Time.unscaledTime + 1f;
			lastAx[0] = deviceState.lX;
			lastAx[1] = deviceState.lY;
			lastAx[2] = deviceState.lRz;
			lastPov = deviceState.rgdwPOV;
			L.Msg($"Wheel axes: steer {deviceState.lX}, gas {deviceState.lY}, brake {deviceState.lRz}, clutch {deviceState.lZ}/{deviceState.lRy}/{deviceState.lU}, pov {deviceState.rgdwPOV}.");
		}
		ulong buttonsA = deviceState.buttonsA;
		uint num = ReadPov(deviceState.rgdwPOV);
		WheelPad.Feed(buttonsA, num);
		if (Mod.WheelButtonLog.Value)
		{
			ulong pressed = WheelPad.Pressed;
			for (int i = 0; i < 64; i++)
			{
				if ((pressed & (ulong)(1L << i)) != 0L)
				{
					L.Msg($"Wheel button {i + 1} pressed.");
				}
			}
		}
		float num2 = Mathf.Clamp(((float)(int)deviceState.lX - 32767.5f) / 32767.5f * Mod.WheelSteerGain.Value, -1f, 1f);
		float num3 = Pedal(0, GetPedalAxis(0, deviceState));
		float num4 = Pedal(1, GetPedalAxis(1, deviceState));
		if (Mathf.Abs(num2 - sentLS.x) > 0.01f || num3 > 0.05f || num4 > 0.05f || WheelPad.AnyDown())
		{
			wheelLastUse = Time.unscaledTime;
		}
		if (Mod.AutoFocus.Value && vrOn && Time.unscaledTime > wheelNextFocus && !GameFocused())
		{
			wheelNextFocus = Time.unscaledTime + 4f;
			FocusGame("wheel");
		}
		if (Time.unscaledTime - wheelLastUse > 10f)
		{
			SendNeutral();
			return;
		}
		uint b = 0u;
		if (num != 65535 && num != uint.MaxValue)
		{
			if (num >= 31500 || num <= 4500)
			{
				b |= 1u;
			}
			if (num >= 13500 && num <= 22500)
			{
				b |= 2u;
			}
			if (num >= 22500 && num <= 31500)
			{
				b |= 4u;
			}
			if (num >= 4500 && num <= 13500)
			{
				b |= 8u;
			}
		}
		Map(Mod.WbSouth.Value, 6);
		Map(Mod.WbEast.Value, 5);
		Map(Mod.WbWest.Value, 7);
		Map(Mod.WbNorth.Value, 4);
		Map(Mod.WbStart.Value, 12);
		Map(Mod.WbSelect.Value, 13);
		Map(Mod.WbL3.Value, 8);
		Map(Mod.WbR3.Value, 9);
		Map(Mod.WbLB.Value, 10);
		Map(Mod.WbRB.Value, 11);
		bool flag = (Object)(object)body != (Object)null && ((!vrOn) ? CameraNearCar() : (stereoView && !theaterActive));
		float num5;
		if (flag)
		{
			num5 = ((Mathf.Abs(num2) < 0.015f) ? 0f : num2);
		}
		else
		{
			num5 = 0f;
		}
		SendPad(b, new Vector2(num5, 0f), flag ? num3 : 0f, flag ? num4 : 0f);
		void Map(int wheelBtn1, int padBit)
		{
			if (WheelPad.Held(wheelBtn1))
			{
				b |= (uint)(1 << padBit);
			}
		}
	}

	private uint ReadPov(ushort flat)
	{
		if (povMode == 0)
		{
			try
			{
				Il2CppStructArray<int> rgdwPOV = DIManager.GetDeviceStateRaw(wheelDev).rgdwPOV;
				if (rgdwPOV != null && rgdwPOV.Length > 0)
				{
					uint num = (uint)rgdwPOV[0];
					if (!povLogged)
					{
						povLogged = true;
						L.Msg($"Wheel: d-pad read from raw state ({num}).");
					}
					return ((num & 0xFFFF) == 65535) ? 65535u : num;
				}
			}
			catch (Exception ex)
			{
				L.Msg("Wheel: raw d-pad unavailable (" + ex.Message + "), using change detection.");
			}
			povMode = 1;
		}
		if (povBase == uint.MaxValue)
		{
			povBase = flat;
		}
		if (flat != povBase)
		{
			return flat;
		}
		return 65535u;
	}

	private ushort GetPedalAxis(int pedalIndex, FlatJoyState2 state)
	{
		if (pedalIndex == 0)
		{
			if (state.lY != 0) return state.lY;
			if (state.lZ != 0) return state.lZ;
			return state.lY;
		}
		if (state.lRz != 0) return state.lRz;
		if (state.lZ != 0) return state.lZ; // Fanatec Load Cell brake
		return state.lRz;
	}

	private float Pedal(int i, ushort raw)
	{
		if (!pedalSeen[i])
		{
			if (Math.Abs(raw - 32767) < 400)
			{
				return 0f;
			}
			pedalSeen[i] = true;
			pedalSeenAt[i] = Time.unscaledTime;
			pedalRest[i] = (int)raw;
		}
		if (Time.unscaledTime - pedalSeenAt[i] < 0.5f)
		{
			pedalRest[i] = (int)raw;
			return 0f;
		}
		if (pedalSeenAt[i] > 0f)
		{
			pedalSeenAt[i] = -1f;
			L.Msg($"Wheel: {((i == 0) ? "gas" : "brake")} pedal rests at {pedalRest[i]:0} ({((pedalRest[i] > 32767f) ? "pressing lowers it" : "pressing raises it")}).");
		}
		float num = pedalRest[i];
		float num2 = ((num > 32767f) ? ((num - (float)(int)raw) / Mathf.Max(1f, num)) : (((float)(int)raw - num) / Mathf.Max(1f, 65535f - num)));
		if (!(num2 < 0.03f))
		{
			return Mathf.Clamp01((num2 - 0.03f) / 0.94f);
		}
		return 0f;
	}

	private bool CameraNearCar()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Camera main = Camera.main;
			return (Object)(object)main != (Object)null && (Object)(object)body != (Object)null && Vector3.Distance(((Component)main).transform.position, body.position) < Mod.LeaveCarDistance.Value;
		}
		catch
		{
			return false;
		}
	}

	private void SendNeutral()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (vpad != null)
		{
			SendPad(0u, Vector2.zero, 0f, 0f);
		}
	}

	private unsafe void SendPad(uint btn, Vector2 ls, float rt, float lt)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if (btn != sentButtons || !(Mathf.Abs(ls.x - sentLS.x) < 0.003f) || !(Mathf.Abs(lt - sentLT) < 0.003f) || !(Mathf.Abs(rt - sentRT) < 0.003f))
		{
			if (vpad == null || !vpad.added)
			{
				vpad = InputSystem.AddDevice("Gamepad", "OTHVR Wheel", (string)null);
				L.Msg($"Wheel: game sees it as a gamepad now (device {vpad.deviceId}).");
			}
			if (evBuf == IntPtr.Zero)
			{
				evBuf = Marshal.AllocHGlobal(64);
			}
			byte* ptr = (byte*)(void*)evBuf;
			for (int i = 0; i < 64; i++)
			{
				ptr[i] = 0;
			}
			*(int*)ptr = 1398030676;
			((short*)ptr)[2] = 52;
			((short*)ptr)[3] = (short)(ushort)vpad.deviceId;
			((double*)ptr)[1] = InputState.currentTime;
			((int*)ptr)[5] = 1196441924;
			((int*)ptr)[6] = (int)btn;
			float* ptr2 = (float*)ptr + 7;
			*ptr2 = ls.x;
			ptr2[1] = ls.y;
			ptr2[2] = 0f;
			ptr2[3] = 0f;
			ptr2[4] = Mathf.Clamp01(lt);
			ptr2[5] = Mathf.Clamp01(rt);
			InputSystem.QueueEvent(new InputEventPtr((InputEvent*)ptr));
			sentButtons = btn;
			sentLS = ls;
			sentLT = lt;
			sentRT = rt;
		}
	}

	static VrDriver()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
	}

	private bool CockpitAllowed()
	{
		return (Object)(object)body != (Object)null;
	}
}
