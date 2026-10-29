using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using Valve.VR;

namespace OverTheHillVR;

public class Mod : MelonMod
{
	public const string Version = "0.3.0";

	internal static MelonLogger.Instance L;

	internal static string ModDir;

	internal static MelonPreferences_Entry<bool> StartInVR;

	internal static MelonPreferences_Entry<float> ResolutionScale;

	internal static MelonPreferences_Entry<bool> FlipEyeY;

	internal static MelonPreferences_Entry<bool> MirrorToDesktop;

	internal static MelonPreferences_Entry<bool> PostProcessing;

	internal static MelonPreferences_Entry<float> NearClip;

	internal static MelonPreferences_Entry<bool> SmoothCarBody;

	internal static MelonPreferences_Entry<float> SeatUp;

	internal static MelonPreferences_Entry<float> SeatForward;

	internal static MelonPreferences_Entry<float> SeatRight;

	internal static MelonPreferences_Entry<bool> FollowCarPitchRoll;

	internal static MelonPreferences_Entry<float> CockpitRadius;

	internal static MelonPreferences_Entry<bool> ChaseCamInVR;

	internal static MelonPreferences_Entry<float> ScreenWidth;

	internal static MelonPreferences_Entry<float> ScreenDistance;

	internal static MelonPreferences_Entry<bool> FlipScreen;

	internal static MelonPreferences_Entry<bool> DebugLog;

	internal static MelonPreferences_Entry<bool> ControllerCombos;

	internal static MelonPreferences_Entry<bool> VrChaseCam;

	internal static MelonPreferences_Entry<bool> HudInVR;

	internal static MelonPreferences_Entry<float> LeaveCarDistance;

	internal static MelonPreferences_Entry<bool> ModelWheel;

	internal static MelonPreferences_Entry<bool> ModelShifter;

	internal static MelonPreferences_Entry<bool> Sunroof;

	internal static MelonPreferences_Entry<bool> TallWindshield;

	internal static MelonPreferences_Entry<bool> Mirrors;

	internal static MelonPreferences_Entry<bool> ArmsOn;

	internal static MelonPreferences_Entry<bool> FlipPalms;

	internal static MelonPreferences_Entry<bool> SunroofFrame;

	internal static MelonPreferences_Entry<float> ShifterStraighten;

	internal static MelonPreferences_Entry<float> TheaterScreenWidth;

	internal static MelonPreferences_Entry<float> TheaterEyeHeight;

	internal static MelonPreferences_Entry<float> TheaterDistance;

	internal static MelonPreferences_Entry<float> HandFollow;

	internal static MelonPreferences_Entry<float> CabOffsetX;

	internal static MelonPreferences_Entry<float> CabOffsetY;

	internal static MelonPreferences_Entry<float> CabOffsetZ;

	internal static MelonPreferences_Entry<float> CabScale;

	internal static MelonPreferences_Entry<bool> TruckCab;

	internal static MelonPreferences_Entry<bool> RealInterior;

	internal static MelonPreferences_Entry<bool> NoMotionBlur;

	internal static MelonPreferences_Entry<bool> RoofOn;

	internal static MelonPreferences_Entry<bool> MatchCarColor;

	internal static MelonPreferences_Entry<int> InteriorStyle;

	internal static MelonPreferences_Entry<float> InteriorBrightness;

	internal static MelonPreferences_Entry<float> InteriorNightDim;

	internal static MelonPreferences_Entry<float> FfbStrength;

	internal static MelonPreferences_Entry<float> FfbMaxForce;

	internal static MelonPreferences_Entry<float> FfbAligning;

	internal static MelonPreferences_Entry<float> FfbRoad;

	internal static MelonPreferences_Entry<float> FfbCollision;

	internal static MelonPreferences_Entry<float> FfbDamper;

	internal static MelonPreferences_Entry<float> FfbParkedWeight;

	internal static MelonPreferences_Entry<float> FfbCentering;

	internal static MelonPreferences_Entry<float> FfbMaxSteerDeg;

	internal static MelonPreferences_Entry<float> FfbFrontAxleZ;

	internal static MelonPreferences_Entry<bool> FfbOn;

	internal static MelonPreferences_Entry<bool> FfbInvert;

	internal static MelonPreferences_Entry<bool> FfbLog;

	internal static MelonPreferences_Entry<bool> FfbAutoDirection;

	internal static MelonPreferences_Entry<bool> WheelSupport;

	internal static MelonPreferences_Entry<bool> WheelPedalsInverted;

	internal static MelonPreferences_Entry<float> WheelSteerGain;

	internal static MelonPreferences_Entry<int> WbSouth;

	internal static MelonPreferences_Entry<int> WbWest;

	internal static MelonPreferences_Entry<int> WbEast;

	internal static MelonPreferences_Entry<int> WbNorth;

	internal static MelonPreferences_Entry<int> WbRB;

	internal static MelonPreferences_Entry<int> WbLB;

	internal static MelonPreferences_Entry<int> WbSelect;

	internal static MelonPreferences_Entry<int> WbStart;

	internal static MelonPreferences_Entry<int> WbR3;

	internal static MelonPreferences_Entry<int> WbL3;

	internal static MelonPreferences_Entry<float> VrShadowDistance;

	internal static MelonPreferences_Entry<float> WheelLock;

	internal static MelonPreferences_Entry<bool> WheelButtons;

	internal static MelonPreferences_Entry<bool> WheelButtonLog;

	internal static MelonPreferences_Entry<bool> WheelFromCar;

	internal static MelonPreferences_Entry<bool> WheelInvert;

	internal static MelonPreferences_Entry<int> WbRecenter;

	internal static MelonPreferences_Entry<int> WbShot;

	internal static MelonPreferences_Entry<int> WbSeatUp;

	internal static MelonPreferences_Entry<int> WbSeatDown;

	internal static MelonPreferences_Entry<int> WbHud;

	internal static MelonPreferences_Entry<int> WbVr;

	internal static MelonPreferences_Entry<int> GripVariant;

	internal static MelonPreferences_Entry<bool> GripCalibrate;

	internal static MelonPreferences_Entry<bool> Theater;

	internal static MelonPreferences_Entry<bool> TheaterFlip;

	internal static MelonPreferences_Entry<bool> AutoFocus;

	internal static MelonPreferences_Entry<float> SunroofFront;

	internal static MelonPreferences_Entry<float> SunroofBack;

	internal static MelonPreferences_Entry<float> GripAngle;

	internal static MelonPreferences_Entry<float> GripOut;

	internal static MelonPreferences_Entry<float> GripBack;

	internal static MelonPreferences_Entry<float> FingerCurl;

	internal static MelonPreferences_Entry<float> ShoulderWidth;

	internal static MelonPreferences_Entry<float> ShoulderDrop;

	internal static MelonPreferences_Entry<float> ShoulderBack;

	internal static MelonPreferences_Entry<float> WheelScale;

	internal static MelonPreferences_Entry<float> ShifterScale;

	internal static MelonPreferences_Entry<float> SunroofClearance;

	internal static MelonPreferences_Entry<float> MirrorSize;

	internal static MelonPreferences_Entry<float> ChaseDistance;

	internal static MelonPreferences_Entry<float> ChaseHeight;

	internal static MelonPreferences_Entry<float> ChaseTurnSpeed;

	internal static MelonPreferences_Entry<float> HudWidth;

	internal static MelonPreferences_Entry<float> HudDistance;

	internal static MelonPreferences_Entry<float> HudDown;

	private static IntPtr ovrHandle;

	public override void OnInitializeMelon()
	{
		L = LoggerInstance;
		ModDir = Path.GetDirectoryName(typeof(Mod).Assembly.Location);
		MelonPreferences_Category melonPreferences_Category = MelonPreferences.CreateCategory("OverTheHillVR", "Over The Hill VR");
		StartInVR = melonPreferences_Category.CreateEntry("StartInVR", default_value: true, null, "Start VR automatically. F8 toggles VR any time.");
		ResolutionScale = melonPreferences_Category.CreateEntry("EyeResolution", 0.65f, null, "Fraction of SteamVR recommended per-eye resolution (0.3 to 1.5). Lower = faster.");
		FlipEyeY = melonPreferences_Category.CreateEntry("FlipEyeY", default_value: true, null, "Flip eye images vertically. F7 toggles if the headset image is upside down.");
		MirrorToDesktop = melonPreferences_Category.CreateEntry("MirrorToDesktop", default_value: false, null, "Keep rendering the normal game view on the monitor while driving in VR (costs performance).");
		PostProcessing = melonPreferences_Category.CreateEntry("PostProcessing", default_value: true, null, "Game post effects (color grading, bloom) in VR. Motion blur/TAA are always off in VR.");
		NearClip = melonPreferences_Category.CreateEntry("NearClip", 0.05f, null, "Near clip in meters. Raise if car parts clip into your face.");
		SmoothCarBody = melonPreferences_Category.CreateEntry("SmoothCarBody", default_value: true, null, "Turn on rigidbody interpolation for your car so the cabin does not judder in VR.");
		SeatUp = melonPreferences_Category.CreateEntry("SeatUp", 0f, null, "Seat height offset in meters (PageUp/PageDown).");
		SeatForward = melonPreferences_Category.CreateEntry("SeatForward", 0f, null, "Seat forward offset in meters (Home/End).");
		SeatRight = melonPreferences_Category.CreateEntry("SeatRight", 0f, null, "Seat sideways offset in meters (Insert/Delete).");
		FollowCarPitchRoll = melonPreferences_Category.CreateEntry("FollowCarPitchRoll", default_value: true, null, "View tilts with the car body (real cockpit feel). False = horizon stays level (comfort).");
		CockpitRadius = melonPreferences_Category.CreateEntry("CockpitRadius", 1.6f, null, "Game camera within this many meters of the driver seat counts as in-cockpit.");
		ChaseCamInVR = melonPreferences_Category.CreateEntry("VRWhileDriving", default_value: true, null, "3D VR in chase/bonnet views too. False = only the driver seat is 3D, other views show on the flat screen.");
		ScreenWidth = melonPreferences_Category.CreateEntry("ScreenWidth", 3.2f, null, "Theatre screen width in meters (menus, map, camp, chase cam).");
		ScreenDistance = melonPreferences_Category.CreateEntry("ScreenDistance", 2.6f, null, "Theatre screen distance in meters.");
		FlipScreen = melonPreferences_Category.CreateEntry("FlipScreen", default_value: false, null, "Flip the theatre screen vertically if it shows upside down.");
		ControllerCombos = melonPreferences_Category.CreateEntry("ControllerCombos", default_value: true, null, "Xbox controller: View+Y VR on/off, L3+R3 or View+A recenter, View+D-pad up/down seat height, View+D-pad left/right seat back/forward.");
		VrChaseCam = melonPreferences_Category.CreateEntry("VrChaseCam", default_value: true, null, "Chase view in VR follows behind the car (the game's own chase camera does not turn with the car).");
		ChaseDistance = melonPreferences_Category.CreateEntry("ChaseDistance", 6f, null, "Unused: chase distance now follows the game zoom (D-pad).");
		ChaseHeight = melonPreferences_Category.CreateEntry("ChaseHeight", 2.5f, null, "VR chase cam height above the car, meters.");
		ChaseTurnSpeed = melonPreferences_Category.CreateEntry("ChaseTurnSpeed", 2.5f, null, "How fast the VR chase cam swings behind the car. Higher = tighter.");
		LeaveCarDistance = melonPreferences_Category.CreateEntry("LeaveCarDistance", 25f, null, "In camp/garage, switch to the flat screen once the game camera is this far from the car, meters.");
		ModelWheel = melonPreferences_Category.CreateEntry("SteeringWheelModel", default_value: true, null, "Use the classic wood wheel model.");
		WheelScale = melonPreferences_Category.CreateEntry("WheelScale", 1f, null, "Size of the wheel model relative to the stock wheel.");
		ModelShifter = melonPreferences_Category.CreateEntry("ShifterModel", default_value: true, null, "Use the leather-boot shifter model.");
		ShifterScale = melonPreferences_Category.CreateEntry("ShifterSize", 0.9f, null, "Shifter model height relative to FPP's lever.");
		Theater = melonPreferences_Category.CreateEntry("BlackVoidMenus", default_value: true, null, "Menus, map and camp float on a screen in black space (instead of SteamVR's home).");
		TheaterScreenWidth = melonPreferences_Category.CreateEntry("ScreenWidthVoid", 3.6f, null, "Menu screen width in meters.");
		TheaterDistance = melonPreferences_Category.CreateEntry("ScreenDistanceVoid", 2.8f, null, "Menu screen distance in meters.");
		TheaterEyeHeight = melonPreferences_Category.CreateEntry("TheaterEyeHeight", 1.15f, null, "Seated eye height in the menu void, meters (screen is centered there).");
		TheaterFlip = melonPreferences_Category.CreateEntry("MenuScreenFlip", default_value: true, null, "Flip the cinema screen if the picture is upside down.");
		AutoFocus = melonPreferences_Category.CreateEntry("AutoFocus", default_value: true, null, "Bring the game window to the front when VR starts and when you press a controller button, so the controller works without clicking.");
		ShifterStraighten = melonPreferences_Category.CreateEntry("ShifterStraighten", 0.85f, null, "How much to straighten the shifter's built-in lean (0 = as modeled, 1 = upright).");
		Sunroof = melonPreferences_Category.CreateEntry("OpenRoof", default_value: true, null, "Open the roof above your head so you can see the mountains.");
		SunroofClearance = melonPreferences_Category.CreateEntry("SunroofClearance", 0.02f, null, "Roof parts whose bottom is this far above eye height get removed, meters.");
		TallWindshield = melonPreferences_Category.CreateEntry("TallWindshield", default_value: true, null, "Also remove the thin windshield header bar ahead of you, for a taller windshield.");
		SunroofFrame = melonPreferences_Category.CreateEntry("SunroofFrame", default_value: true, null, "Keep a roof frame (windshield top bar, side rails, roof behind you) around the sunroof opening.");
		SunroofFront = melonPreferences_Category.CreateEntry("WindshieldBarDepth", 0.07f, null, "Depth of the roof bar across the top of the windshield, meters.");
		SunroofBack = melonPreferences_Category.CreateEntry("SunroofBack", 0.45f, null, "Sunroof opening extends this far behind your eyes, meters.");
		Mirrors = melonPreferences_Category.CreateEntry("MirrorsOn", default_value: false, null, "Working rear-view and side mirrors in the cockpit (costs some fps).");
		MirrorSize = melonPreferences_Category.CreateEntry("MirrorSize", 1f, null, "Mirror size multiplier.");
		ArmsOn = melonPreferences_Category.CreateEntry("ArmsOn", default_value: false, null, "Show arms with hands gripping the steering wheel in the cockpit.");
		FlipPalms = melonPreferences_Category.CreateEntry("FlipPalms2", default_value: true, null, "Turn the hands around if palms face the wrong way.");
		VrShadowDistance = melonPreferences_Category.CreateEntry("VrShadowDistance", 50f, null, "Max shadow distance in VR, meters (shadows are drawn per VR camera; lower = more fps). 0 = leave the game's setting.");
		WheelFromCar = melonPreferences_Category.CreateEntry("WheelFromCar", default_value: true, null, "Turn the cockpit steering wheel from the car's real steering (works with G29/TrailWheel, pad and keyboard).");
		WheelLock = melonPreferences_Category.CreateEntry("WheelLockDegrees", 540f, null, "Cockpit wheel rotation lock-to-lock, degrees (G29 default range is 900; the game's steering may use less).");
		WheelInvert = melonPreferences_Category.CreateEntry("WheelInvertCockpit", default_value: true, null, "Reverse the cockpit wheel's turning direction.");
		WheelButtons = melonPreferences_Category.CreateEntry("WheelButtons", default_value: true, null, "Use racing wheel buttons for VR controls (numbers as shown in Windows 'Set up USB game controllers').");
		WheelButtonLog = melonPreferences_Category.CreateEntry("WheelButtonLog", default_value: true, null, "Log each wheel button number when pressed (to find the right numbers).");
		WbRecenter = melonPreferences_Category.CreateEntry("WheelBtnRecenter", 20, null, "Wheel button: recenter view (G29: 20 = Plus).");
		WbShot = melonPreferences_Category.CreateEntry("WheelBtnScreenshot", 21, null, "Wheel button: screenshot (G29: 21 = Minus).");
		WbSeatUp = melonPreferences_Category.CreateEntry("WheelBtnSeatUp", 22, null, "Wheel button: seat up 1 cm (G29: 22 = red dial clockwise).");
		WbSeatDown = melonPreferences_Category.CreateEntry("WheelBtnSeatDown", 23, null, "Wheel button: seat down 1 cm (G29: 23 = red dial counter-clockwise).");
		WbHud = melonPreferences_Category.CreateEntry("WheelBtnHud", 24, null, "Wheel button: HUD panel on/off (G29: 24 = red dial press).");
		WbVr = melonPreferences_Category.CreateEntry("WheelBtnVrHold", 25, null, "Wheel button held 1 s: VR on/off (G29: 25 = PS button). 0 = none.");
		NoMotionBlur = melonPreferences_Category.CreateEntry("NoMotionBlur", default_value: true, null, "Turn off the game's motion blur, depth of field, lens distortion and chromatic aberration while in VR (they smear when you look around).");
		RoofOn = melonPreferences_Category.CreateEntry("HardtopRoof", default_value: false, null, "Cockpit has a hardtop roof (true) or is open/convertible (false). In the cockpit, zoom in once more (D-pad up / F4) to switch.");
		MatchCarColor = melonPreferences_Category.CreateEntry("InteriorMatchCarColor", default_value: true, null, "Paint the cockpit's painted parts (dash, sills, pillars, roof) in the car's own color.");
		InteriorStyle = melonPreferences_Category.CreateEntry("InteriorStyle", -1, null, "Cockpit trim style: -1 = picked per car/livery automatically, 0 Classic, 1 Safari, 2 Rally, 3 Ranch, 4 Alpine, 5 Expedition. In the cockpit, hold D-pad up 1 s (or F3) to cycle.");
		InteriorBrightness = melonPreferences_Category.CreateEntry("InteriorDayGlow", 0.15f, null, "Extra self-light on the cockpit interior so it is not too dark in shadow (0 = off).");
		InteriorNightDim = melonPreferences_Category.CreateEntry("InteriorNightDim", 0.6f, null, "How much darker the cockpit gets at night, so headlights/moonlight don't light it up (0..1).");
		WheelSupport = melonPreferences_Category.CreateEntry("WheelSupport", default_value: true, null, "Racing wheel support (G29 etc.): steering, pedals and buttons go to the game as a gamepad. No other mods needed.");
		WheelSteerGain = melonPreferences_Category.CreateEntry("WheelSteerGain", 1f, null, "Steering sensitivity (1 = full wheel rotation for full lock; set the rotation in G HUB, 540 or 900).");
		WheelPedalsInverted = melonPreferences_Category.CreateEntry("WheelPedalsInverted", default_value: true, null, "Pedals read backwards (true for Logitech G29/G920 in G HUB). Flip if gas/brake are stuck on.");
		WbSouth = melonPreferences_Category.CreateEntry("WheelBtnA", 1, null, "Wheel button (1-based, see log) for gamepad A. G29: cross = 1.");
		WbWest = melonPreferences_Category.CreateEntry("WheelBtnX", 2, null, "Wheel button for gamepad X. G29: square = 2.");
		WbEast = melonPreferences_Category.CreateEntry("WheelBtnB", 3, null, "Wheel button for gamepad B. G29: circle = 3.");
		WbNorth = melonPreferences_Category.CreateEntry("WheelBtnY", 4, null, "Wheel button for gamepad Y. G29: triangle = 4.");
		WbRB = melonPreferences_Category.CreateEntry("WheelBtnRB", 7, null, "Wheel button for gamepad RB. G29: R2 = 7.");
		WbLB = melonPreferences_Category.CreateEntry("WheelBtnLB", 8, null, "Wheel button for gamepad LB. G29: L2 = 8.");
		WbSelect = melonPreferences_Category.CreateEntry("WheelBtnView", 9, null, "Wheel button for gamepad View/Back. G29: share = 9.");
		WbStart = melonPreferences_Category.CreateEntry("WheelBtnMenu", 10, null, "Wheel button for gamepad Menu/Start. G29: options = 10.");
		WbR3 = melonPreferences_Category.CreateEntry("WheelBtnR3", 11, null, "Wheel button for gamepad R3. G29: R3 = 11.");
		WbL3 = melonPreferences_Category.CreateEntry("WheelBtnL3", 12, null, "Wheel button for gamepad L3. G29: L3 = 12.");
		FfbOn = melonPreferences_Category.CreateEntry("FfbOn", default_value: true, null, "Force feedback for DirectInput wheels (G29 etc.) from the VR mod. Turn TrailWheel's own force feedback off so they don't fight.");
		FfbStrength = melonPreferences_Category.CreateEntry("FfbStrength3", 1f, null, "Overall force feedback strength (0..1).");
		FfbMaxForce = melonPreferences_Category.CreateEntry("FfbMaxForce", 0.85f, null, "Cap on the main steering force (0..1).");
		FfbInvert = melonPreferences_Category.CreateEntry("FfbInvert", default_value: false, null, "Flip the force direction (on top of the automatic check). Set true if the wheel pulls AWAY from center while driving.");
		FfbAutoDirection = melonPreferences_Category.CreateEntry("FfbAutoDirection", default_value: true, null, "When parked with the wheel near center, give one short gentle push to learn which way the force turns the wheel.");
		FfbAligning = melonPreferences_Category.CreateEntry("FfbAligning2", 1f, null, "Self-aligning pull from the front tires (the main 'road feel').");
		FfbRoad = melonPreferences_Category.CreateEntry("FfbRoad", 0.6f, null, "Kickback from bumps, rocks and ruts.");
		FfbCollision = melonPreferences_Category.CreateEntry("FfbCollision", 0.7f, null, "Jolt when you hit something.");
		FfbDamper = melonPreferences_Category.CreateEntry("FfbDamper2", 0.22f, null, "Steering damping at speed.");
		FfbParkedWeight = melonPreferences_Category.CreateEntry("FfbParkedWeight2", 0.3f, null, "Extra heaviness at walking pace / parked.");
		FfbCentering = melonPreferences_Category.CreateEntry("FfbCentering3", 0.4f, null, "Light centering spring.");
		FfbMaxSteerDeg = melonPreferences_Category.CreateEntry("FfbMaxSteerDegrees", 35f, null, "Front wheel angle at full steering lock (for the slip calculation).");
		FfbFrontAxleZ = melonPreferences_Category.CreateEntry("FfbFrontAxleZ", 1.4f, null, "Front axle position (car-local meters).");
		FfbLog = melonPreferences_Category.CreateEntry("FfbLog", default_value: true, null, "Log force feedback values every 5 s while driving.");
		RealInterior = melonPreferences_Category.CreateEntry("RealInterior", default_value: true, null, "Modeled vintage 4x4 interior (rounded dash, leather seats, wood trim, open roof) instead of FPP's boxes.");
		TruckCab = melonPreferences_Category.CreateEntry("TruckCabOn", default_value: false, null, "Use the pickup truck interior (with big sunroof) instead of FPP's box cabin.");
		CabOffsetX = melonPreferences_Category.CreateEntry("CabOffsetX", 0f, null, "Truck cab offset right(+)/left(-), meters.");
		CabOffsetY = melonPreferences_Category.CreateEntry("CabOffsetY", 0f, null, "Truck cab offset up(+)/down(-), meters.");
		CabOffsetZ = melonPreferences_Category.CreateEntry("CabOffsetZ", 0f, null, "Truck cab offset forward(+)/back(-), meters.");
		CabScale = melonPreferences_Category.CreateEntry("CabScale", 1f, null, "Truck cab size multiplier.");
		HandFollow = melonPreferences_Category.CreateEntry("HandFollow", 70f, null, "Hands turn with the wheel up to this many degrees, then the rim slides through them (stops arms crossing).");
		GripVariant = melonPreferences_Category.CreateEntry("GripVariant", 0, null, "Hand orientation variant 0-5 (set from calibration screenshots).");
		GripCalibrate = melonPreferences_Category.CreateEntry("GripCalibrate2", default_value: false, null, "On first cockpit entry, cycle hand orientations and screenshot each (for tuning).");
		GripAngle = melonPreferences_Category.CreateEntry("GripAngle", 60f, null, "Hand position on the wheel in degrees from the top (60 = 10 and 2, 90 = 9 and 3).");
		GripOut = melonPreferences_Category.CreateEntry("GripIn", 0.07f, null, "Wrist distance inside the rim (toward the hub), meters.");
		GripBack = melonPreferences_Category.CreateEntry("GripBack2", 0.035f, null, "Wrist offset toward you from the rim, meters.");
		FingerCurl = melonPreferences_Category.CreateEntry("FingerCurl", 1f, null, "How tightly the fingers wrap the rim (0 = flat, 1 = full grip).");
		ShoulderWidth = melonPreferences_Category.CreateEntry("ShoulderWidth", 0.38f, null, "Distance between shoulders, meters.");
		ShoulderDrop = melonPreferences_Category.CreateEntry("ShoulderDrop2", 0.34f, null, "Shoulders below eye height, meters.");
		ShoulderBack = melonPreferences_Category.CreateEntry("ShoulderBack2", 0.14f, null, "Shoulders behind the eyes, meters.");
		HudInVR = melonPreferences_Category.CreateEntry("HudInVR", default_value: true, null, "Show game HUD, tutorial and prompts on a floating panel in VR. Back+B (F12) toggles.");
		HudWidth = melonPreferences_Category.CreateEntry("HudWidth", 1.4f, null, "HUD panel width, meters.");
		HudDistance = melonPreferences_Category.CreateEntry("HudDistance", 1.1f, null, "HUD panel distance, meters.");
		HudDown = melonPreferences_Category.CreateEntry("HudDown", 0.2f, null, "HUD panel below eye level, meters.");
		DebugLog = melonPreferences_Category.CreateEntry("DebugLog", default_value: true, null, "Extra log lines for tuning (camera, cockpit, car).");
		try
		{
			string text = Path.Combine(ModDir, "OverTheHillVR", "openvr_api.dll");
			if (!File.Exists(text))
			{
				text = Path.Combine(ModDir, "openvr_api.dll");
			}
			if (!File.Exists(text))
			{
				L.Error("openvr_api.dll missing. Put it in Mods\\OverTheHillVR\\.");
				return;
			}
			ovrHandle = System.Runtime.InteropServices.NativeLibrary.Load(text);
			System.Runtime.InteropServices.NativeLibrary.SetDllImportResolver(typeof(OpenVR).Assembly, Resolve);
		}
		catch (Exception ex)
		{
			L.Error("Failed to load openvr_api.dll: " + ex);
			return;
		}
		VrDriver.Instance = new VrDriver();
		L.Msg("Over The Hill VR 0.3.2 loaded (wheel event hotfix + all-vehicle cockpit) (standalone: no other mods needed). F8 VR on/off, F6 recenter, F7 flip eyes, PgUp/PgDn seat height, Home/End seat fwd/back, Ins/Del seat sideways. Pad: View+Y VR, View+X 3D/flat, View+B HUD, hold L3+R3 1s screenshot, D-pad zoom (closest = bonnet, then cockpit), F11 cockpit, L3+R3 recenter, View+D-pad seat. Needs SteamVR + -force-d3d11.");
	}

	private static IntPtr Resolve(string name, Assembly asm, DllImportSearchPath? path)
	{
		if (!name.StartsWith("openvr_api", StringComparison.OrdinalIgnoreCase))
		{
			return IntPtr.Zero;
		}
		return ovrHandle;
	}

	public override void OnUpdate()
	{
		VrDriver.Instance?.Update();
	}

	public override void OnLateUpdate()
	{
		VrDriver.Instance?.LateUpdate();
	}

	public override void OnApplicationQuit()
	{
		VrDriver.Instance?.Quit();
	}
}
