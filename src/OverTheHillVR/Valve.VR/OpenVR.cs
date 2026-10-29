using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class OpenVR
{
	private class COpenVRContext
	{
		private CVRSystem m_pVRSystem;

		private CVRChaperone m_pVRChaperone;

		private CVRChaperoneSetup m_pVRChaperoneSetup;

		private CVRCompositor m_pVRCompositor;

		private CVRHeadsetView m_pVRHeadsetView;

		private CVROverlay m_pVROverlay;

		private CVROverlayView m_pVROverlayView;

		private CVRRenderModels m_pVRRenderModels;

		private CVRResources m_pVRResources;

		private CVRExtendedDisplay m_pVRExtendedDisplay;

		private CVRSettings m_pVRSettings;

		private CVRApplications m_pVRApplications;

		private CVRScreenshots m_pVRScreenshots;

		private CVRTrackedCamera m_pVRTrackedCamera;

		private CVRInput m_pVRInput;

		private CVRIOBuffer m_pVRIOBuffer;

		private CVRSpatialAnchors m_pVRSpatialAnchors;

		private CVRNotifications m_pVRNotifications;

		private CVRDebug m_pVRDebug;

		public COpenVRContext()
		{
			Clear();
		}

		public void Clear()
		{
			m_pVRSystem = null;
			m_pVRChaperone = null;
			m_pVRChaperoneSetup = null;
			m_pVRCompositor = null;
			m_pVRHeadsetView = null;
			m_pVROverlay = null;
			m_pVROverlayView = null;
			m_pVRRenderModels = null;
			m_pVRResources = null;
			m_pVRExtendedDisplay = null;
			m_pVRSettings = null;
			m_pVRApplications = null;
			m_pVRScreenshots = null;
			m_pVRTrackedCamera = null;
			m_pVRInput = null;
			m_pVRIOBuffer = null;
			m_pVRSpatialAnchors = null;
			m_pVRNotifications = null;
			m_pVRDebug = null;
		}

		private void CheckClear()
		{
			if (VRToken != GetInitToken())
			{
				Clear();
				VRToken = GetInitToken();
			}
		}

		public CVRSystem VRSystem()
		{
			CheckClear();
			if (m_pVRSystem == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSystem_026", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRSystem = new CVRSystem(genericInterface);
				}
			}
			return m_pVRSystem;
		}

		public CVRChaperone VRChaperone()
		{
			CheckClear();
			if (m_pVRChaperone == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperone_004", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRChaperone = new CVRChaperone(genericInterface);
				}
			}
			return m_pVRChaperone;
		}

		public CVRChaperoneSetup VRChaperoneSetup()
		{
			CheckClear();
			if (m_pVRChaperoneSetup == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperoneSetup_006", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRChaperoneSetup = new CVRChaperoneSetup(genericInterface);
				}
			}
			return m_pVRChaperoneSetup;
		}

		public CVRCompositor VRCompositor()
		{
			CheckClear();
			if (m_pVRCompositor == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRCompositor_029", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRCompositor = new CVRCompositor(genericInterface);
				}
			}
			return m_pVRCompositor;
		}

		public CVRHeadsetView VRHeadsetView()
		{
			CheckClear();
			if (m_pVRHeadsetView == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRHeadsetView_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRHeadsetView = new CVRHeadsetView(genericInterface);
				}
			}
			return m_pVRHeadsetView;
		}

		public CVROverlay VROverlay()
		{
			CheckClear();
			if (m_pVROverlay == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlay_028", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVROverlay = new CVROverlay(genericInterface);
				}
			}
			return m_pVROverlay;
		}

		public CVROverlayView VROverlayView()
		{
			CheckClear();
			if (m_pVROverlayView == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlayView_003", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVROverlayView = new CVROverlayView(genericInterface);
				}
			}
			return m_pVROverlayView;
		}

		public CVRRenderModels VRRenderModels()
		{
			CheckClear();
			if (m_pVRRenderModels == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRRenderModels_006", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRRenderModels = new CVRRenderModels(genericInterface);
				}
			}
			return m_pVRRenderModels;
		}

		public CVRResources VRResources()
		{
			CheckClear();
			if (m_pVRResources == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRResources_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRResources = new CVRResources(genericInterface);
				}
			}
			return m_pVRResources;
		}

		public CVRExtendedDisplay VRExtendedDisplay()
		{
			CheckClear();
			if (m_pVRExtendedDisplay == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRExtendedDisplay_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRExtendedDisplay = new CVRExtendedDisplay(genericInterface);
				}
			}
			return m_pVRExtendedDisplay;
		}

		public CVRSettings VRSettings()
		{
			CheckClear();
			if (m_pVRSettings == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSettings_003", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRSettings = new CVRSettings(genericInterface);
				}
			}
			return m_pVRSettings;
		}

		public CVRApplications VRApplications()
		{
			CheckClear();
			if (m_pVRApplications == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRApplications_008", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRApplications = new CVRApplications(genericInterface);
				}
			}
			return m_pVRApplications;
		}

		public CVRScreenshots VRScreenshots()
		{
			CheckClear();
			if (m_pVRScreenshots == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRScreenshots_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRScreenshots = new CVRScreenshots(genericInterface);
				}
			}
			return m_pVRScreenshots;
		}

		public CVRTrackedCamera VRTrackedCamera()
		{
			CheckClear();
			if (m_pVRTrackedCamera == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRTrackedCamera_006", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRTrackedCamera = new CVRTrackedCamera(genericInterface);
				}
			}
			return m_pVRTrackedCamera;
		}

		public CVRInput VRInput()
		{
			CheckClear();
			if (m_pVRInput == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRInput_011", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRInput = new CVRInput(genericInterface);
				}
			}
			return m_pVRInput;
		}

		public CVRIOBuffer VRIOBuffer()
		{
			CheckClear();
			if (m_pVRIOBuffer == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRIOBuffer_002", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRIOBuffer = new CVRIOBuffer(genericInterface);
				}
			}
			return m_pVRIOBuffer;
		}

		public CVRSpatialAnchors VRSpatialAnchors()
		{
			CheckClear();
			if (m_pVRSpatialAnchors == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSpatialAnchors_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRSpatialAnchors = new CVRSpatialAnchors(genericInterface);
				}
			}
			return m_pVRSpatialAnchors;
		}

		public CVRDebug VRDebug()
		{
			CheckClear();
			if (m_pVRDebug == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRDebug_001", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRDebug = new CVRDebug(genericInterface);
				}
			}
			return m_pVRDebug;
		}

		public CVRNotifications VRNotifications()
		{
			CheckClear();
			if (m_pVRNotifications == null)
			{
				EVRInitError peError = EVRInitError.None;
				IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRNotifications_002", ref peError);
				if (genericInterface != IntPtr.Zero && peError == EVRInitError.None)
				{
					m_pVRNotifications = new CVRNotifications(genericInterface);
				}
			}
			return m_pVRNotifications;
		}
	}

	public const uint MaxDmabufPlaneCount = 4u;

	public const uint k_nDriverNone = uint.MaxValue;

	public const uint k_unMaxDriverDebugResponseSize = 32768u;

	public const uint k_unTrackedDeviceIndex_Hmd = 0u;

	public const uint k_unMaxTrackedDeviceCount = 64u;

	public const uint k_unTrackedDeviceIndexOther = 4294967294u;

	public const uint k_unTrackedDeviceIndexInvalid = uint.MaxValue;

	public const ulong k_ulInvalidPropertyContainer = 0uL;

	public const uint k_unInvalidPropertyTag = 0u;

	public const ulong k_ulInvalidDriverHandle = 0uL;

	public const uint k_unFloatPropertyTag = 1u;

	public const uint k_unInt32PropertyTag = 2u;

	public const uint k_unUint64PropertyTag = 3u;

	public const uint k_unBoolPropertyTag = 4u;

	public const uint k_unStringPropertyTag = 5u;

	public const uint k_unErrorPropertyTag = 6u;

	public const uint k_unDoublePropertyTag = 7u;

	public const uint k_unHmdMatrix34PropertyTag = 20u;

	public const uint k_unHmdMatrix44PropertyTag = 21u;

	public const uint k_unHmdVector3PropertyTag = 22u;

	public const uint k_unHmdVector4PropertyTag = 23u;

	public const uint k_unHmdVector2PropertyTag = 24u;

	public const uint k_unHmdQuadPropertyTag = 25u;

	public const uint k_unHiddenAreaPropertyTag = 30u;

	public const uint k_unPathHandleInfoTag = 31u;

	public const uint k_unActionPropertyTag = 32u;

	public const uint k_unInputValuePropertyTag = 33u;

	public const uint k_unWildcardPropertyTag = 34u;

	public const uint k_unHapticVibrationPropertyTag = 35u;

	public const uint k_unSkeletonPropertyTag = 36u;

	public const uint k_unSpatialAnchorPosePropertyTag = 40u;

	public const uint k_unJsonPropertyTag = 41u;

	public const uint k_unActiveActionSetPropertyTag = 42u;

	public const uint k_unOpenVRInternalReserved_Start = 1000u;

	public const uint k_unOpenVRInternalReserved_End = 10000u;

	public const uint k_unMaxPropertyStringSize = 32768u;

	public const ulong k_ulInvalidActionHandle = 0uL;

	public const ulong k_ulInvalidActionSetHandle = 0uL;

	public const ulong k_ulInvalidInputValueHandle = 0uL;

	public const ulong k_ulInvalidInputComponentHandle = 0uL;

	public const uint k_unControllerStateAxisCount = 5u;

	public const ulong k_ulOverlayHandleInvalid = 0uL;

	public const uint k_unMaxDistortionFunctionParameters = 8u;

	public const uint k_unScreenshotHandleInvalid = 0u;

	public const string IVRSystem_Version = "IVRSystem_026";

	public const string IVRExtendedDisplay_Version = "IVRExtendedDisplay_001";

	public const string IVRTrackedCamera_Version = "IVRTrackedCamera_006";

	public const uint k_unMaxApplicationKeyLength = 128u;

	public const string k_pch_MimeType_HomeApp = "vr/home";

	public const string k_pch_MimeType_GameTheater = "vr/game_theater";

	public const string IVRApplications_Version = "IVRApplications_008";

	public const string IVRChaperone_Version = "IVRChaperone_004";

	public const string IVRChaperoneSetup_Version = "IVRChaperoneSetup_006";

	public const string IVRCompositor_Version = "IVRCompositor_029";

	public const uint k_unVROverlayMaxKeyLength = 128u;

	public const uint k_unVROverlayMaxNameLength = 128u;

	public const uint k_unMaxOverlayCount = 128u;

	public const uint k_unMaxOverlayIntersectionMaskPrimitivesCount = 32u;

	public const string IVROverlay_Version = "IVROverlay_028";

	public const string IVROverlayView_Version = "IVROverlayView_003";

	public const uint k_unHeadsetViewMaxWidth = 3840u;

	public const uint k_unHeadsetViewMaxHeight = 2160u;

	public const string k_pchHeadsetViewOverlayKey = "system.HeadsetView";

	public const string IVRHeadsetView_Version = "IVRHeadsetView_001";

	public const string k_pch_Controller_Component_GDC2015 = "gdc2015";

	public const string k_pch_Controller_Component_Base = "base";

	public const string k_pch_Controller_Component_Tip = "tip";

	public const string k_pch_Controller_Component_OpenXR_Aim = "openxr_aim";

	public const string k_pch_Controller_Component_HandGrip = "handgrip";

	public const string k_pch_Controller_Component_OpenXR_Grip = "openxr_grip";

	public const string k_pch_Controller_Component_OpenXR_HandModel = "openxr_handmodel";

	public const string k_pch_Controller_Component_Status = "status";

	public const string IVRRenderModels_Version = "IVRRenderModels_006";

	public const uint k_unNotificationTextMaxSize = 256u;

	public const string IVRNotifications_Version = "IVRNotifications_002";

	public const uint k_unMaxSettingsKeyLength = 128u;

	public const string IVRSettings_Version = "IVRSettings_003";

	public const string k_pch_SteamVR_Section = "steamvr";

	public const string k_pch_SteamVR_Contrast_Float = "contrast";

	public const string k_pch_SteamVR_RequireHmd_String = "requireHmd";

	public const string k_pch_SteamVR_ForcedDriverKey_String = "forcedDriver";

	public const string k_pch_SteamVR_ForcedHmdKey_String = "forcedHmd";

	public const string k_pch_SteamVR_DisplayDebug_Bool = "displayDebug";

	public const string k_pch_SteamVR_DebugProcessPipe_String = "debugProcessPipe";

	public const string k_pch_SteamVR_DisplayDebugX_Int32 = "displayDebugX";

	public const string k_pch_SteamVR_DisplayDebugY_Int32 = "displayDebugY";

	public const string k_pch_SteamVR_SendSystemButtonToAllApps_Bool = "sendSystemButtonToAllApps";

	public const string k_pch_SteamVR_LogLevel_Int32 = "loglevel";

	public const string k_pch_SteamVR_IPD_Float = "ipd";

	public const string k_pch_SteamVR_Background_String = "background";

	public const string k_pch_SteamVR_BackgroundUseDomeProjection_Bool = "backgroundUseDomeProjection";

	public const string k_pch_SteamVR_BackgroundCameraHeight_Float = "backgroundCameraHeight";

	public const string k_pch_SteamVR_BackgroundDomeRadius_Float = "backgroundDomeRadius";

	public const string k_pch_SteamVR_GridColor_String = "gridColor";

	public const string k_pch_SteamVR_PlayAreaColor_String = "playAreaColor";

	public const string k_pch_SteamVR_TrackingLossColor_String = "trackingLossColor";

	public const string k_pch_SteamVR_StartColor_String = "startColor";

	public const string k_pch_SteamVR_ShowStage_Bool = "showStage";

	public const string k_pch_SteamVR_DrawTrackingReferences_Bool = "drawTrackingReferences";

	public const string k_pch_SteamVR_ActivateMultipleDrivers_Bool = "activateMultipleDrivers";

	public const string k_pch_SteamVR_UsingSpeakers_Bool = "usingSpeakers";

	public const string k_pch_SteamVR_SpeakersForwardYawOffsetDegrees_Float = "speakersForwardYawOffsetDegrees";

	public const string k_pch_SteamVR_BaseStationPowerManagement_Int32 = "basestationPowerManagement";

	public const string k_pch_SteamVR_ShowBaseStationPowerManagementTip_Int32 = "ShowBaseStationPowerManagementTip";

	public const string k_pch_SteamVR_NeverKillProcesses_Bool = "neverKillProcesses";

	public const string k_pch_SteamVR_SupersampleScale_Float = "supersampleScale";

	public const string k_pch_SteamVR_MaxRecommendedResolution_Int32 = "maxRecommendedResolution";

	public const string k_pch_SteamVR_MotionSmoothing_Bool = "motionSmoothing";

	public const string k_pch_SteamVR_MotionSmoothingOverride_Int32 = "motionSmoothingOverride";

	public const string k_pch_SteamVR_FoveatedSharpening_Bool = "sharpening";

	public const string k_pch_SteamVR_FoveatedSharpeningOverride_Int32 = "sharpeningOverride";

	public const string k_pch_SteamVR_FramesToThrottle_Int32 = "framesToThrottle";

	public const string k_pch_SteamVR_AdditionalFramesToPredict_Int32 = "additionalFramesToPredict";

	public const string k_pch_SteamVR_WorldScale_Float = "worldScale";

	public const string k_pch_SteamVR_FovScale_Int32 = "fovScale";

	public const string k_pch_SteamVR_FovScaleInner_Int32 = "fovScaleInner";

	public const string k_pch_SteamVR_FovScaleUpper_Int32 = "fovScaleUpper";

	public const string k_pch_SteamVR_FovScaleLower_Int32 = "fovScaleLower";

	public const string k_pch_SteamVR_FovScaleFormat_Int32 = "fovScaleFormat";

	public const string k_pch_SteamVR_FovScaleLetterboxed_Bool = "fovScaleLetterboxed";

	public const string k_pch_SteamVR_DisableAsyncReprojection_Bool = "disableAsync";

	public const string k_pch_SteamVR_ForceFadeOnBadTracking_Bool = "forceFadeOnBadTracking";

	public const string k_pch_SteamVR_DefaultMirrorView_Int32 = "mirrorView";

	public const string k_pch_SteamVR_ShowLegacyMirrorView_Bool = "showLegacyMirrorView";

	public const string k_pch_SteamVR_MirrorViewVisibility_Bool = "showMirrorView";

	public const string k_pch_SteamVR_MirrorViewDisplayMode_Int32 = "mirrorViewDisplayMode";

	public const string k_pch_SteamVR_MirrorViewEye_Int32 = "mirrorViewEye";

	public const string k_pch_SteamVR_MirrorViewGeometry_String = "mirrorViewGeometry";

	public const string k_pch_SteamVR_MirrorViewGeometryMaximized_String = "mirrorViewGeometryMaximized";

	public const string k_pch_SteamVR_PerfGraphVisibility_Bool = "showPerfGraph";

	public const string k_pch_SteamVR_StartMonitorFromAppLaunch = "startMonitorFromAppLaunch";

	public const string k_pch_SteamVR_StartCompositorFromAppLaunch_Bool = "startCompositorFromAppLaunch";

	public const string k_pch_SteamVR_StartDashboardFromAppLaunch_Bool = "startDashboardFromAppLaunch";

	public const string k_pch_SteamVR_StartOverlayAppsFromDashboard_Bool = "startOverlayAppsFromDashboard";

	public const string k_pch_SteamVR_EnableHomeApp = "enableHomeApp";

	public const string k_pch_SteamVR_CycleBackgroundImageTimeSec_Int32 = "CycleBackgroundImageTimeSec";

	public const string k_pch_SteamVR_RetailDemo_Bool = "retailDemo";

	public const string k_pch_SteamVR_IpdOffset_Float = "ipdOffset";

	public const string k_pch_SteamVR_AllowSupersampleFiltering_Bool = "allowSupersampleFiltering";

	public const string k_pch_SteamVR_SupersampleManualOverride_Bool = "supersampleManualOverride";

	public const string k_pch_SteamVR_EnableLinuxVulkanAsync_Bool = "enableLinuxVulkanAsync";

	public const string k_pch_SteamVR_AllowDisplayLockedMode_Bool = "allowDisplayLockedMode";

	public const string k_pch_SteamVR_HaveStartedTutorialForNativeChaperoneDriver_Bool = "haveStartedTutorialForNativeChaperoneDriver";

	public const string k_pch_SteamVR_DebugInputBinding = "debugInputBinding";

	public const string k_pch_SteamVR_DoNotFadeToGrid = "doNotFadeToGrid";

	public const string k_pch_SteamVR_EnableSharedResourceJournaling = "enableSharedResourceJournaling";

	public const string k_pch_SteamVR_EnableSafeMode = "enableSafeMode";

	public const string k_pch_SteamVR_PreferredRefreshRate = "preferredRefreshRate";

	public const string k_pch_SteamVR_LastVersionNotice = "lastVersionNotice";

	public const string k_pch_SteamVR_LastVersionNoticeDate = "lastVersionNoticeDate";

	public const string k_pch_SteamVR_HmdDisplayColorGainR_Float = "hmdDisplayColorGainR";

	public const string k_pch_SteamVR_HmdDisplayColorGainG_Float = "hmdDisplayColorGainG";

	public const string k_pch_SteamVR_HmdDisplayColorGainB_Float = "hmdDisplayColorGainB";

	public const string k_pch_SteamVR_CustomIconStyle_String = "customIconStyle";

	public const string k_pch_SteamVR_CustomOffIconStyle_String = "customOffIconStyle";

	public const string k_pch_SteamVR_CustomIconForceUpdate_String = "customIconForceUpdate";

	public const string k_pch_SteamVR_AllowGlobalActionSetPriority = "globalActionSetPriority";

	public const string k_pch_SteamVR_OverlayRenderQuality = "overlayRenderQuality_2";

	public const string k_pch_SteamVR_BlockOculusSDKOnOpenVRLaunchOption_Bool = "blockOculusSDKOnOpenVRLaunchOption";

	public const string k_pch_SteamVR_BlockOculusSDKOnAllLaunches_Bool = "blockOculusSDKOnAllLaunches";

	public const string k_pch_SteamVR_HDCPLegacyCompatibility_Bool = "hdcp14legacyCompatibility";

	public const string k_pch_SteamVR_DisplayPortTrainingMode_Int = "displayPortTrainingMode";

	public const string k_pch_SteamVR_UsePrism_Bool = "usePrism";

	public const string k_pch_SteamVR_AllowFallbackMirrorWindowLinux_Bool = "allowFallbackMirrorWindowLinux";

	public const string k_pch_SteamVR_DisableKeyboardPrivacy_Bool = "disableKeyboardPrivacy";

	public const string k_pch_OpenXR_Section = "openxr";

	public const string k_pch_OpenXR_MetaUnityPluginCompatibility_Int32 = "metaUnityPluginCompatibility";

	public const string k_pch_DirectMode_Section = "direct_mode";

	public const string k_pch_DirectMode_Enable_Bool = "enable";

	public const string k_pch_DirectMode_Count_Int32 = "count";

	public const string k_pch_DirectMode_EdidVid_Int32 = "edidVid";

	public const string k_pch_DirectMode_EdidPid_Int32 = "edidPid";

	public const string k_pch_Lighthouse_Section = "driver_lighthouse";

	public const string k_pch_Lighthouse_DisableIMU_Bool = "disableimu";

	public const string k_pch_Lighthouse_DisableIMUExceptHMD_Bool = "disableimuexcepthmd";

	public const string k_pch_Lighthouse_UseDisambiguation_String = "usedisambiguation";

	public const string k_pch_Lighthouse_DisambiguationDebug_Int32 = "disambiguationdebug";

	public const string k_pch_Lighthouse_PrimaryBasestation_Int32 = "primarybasestation";

	public const string k_pch_Lighthouse_DBHistory_Bool = "dbhistory";

	public const string k_pch_Lighthouse_EnableBluetooth_Bool = "enableBluetooth";

	public const string k_pch_Lighthouse_PowerManagedBaseStations_String = "PowerManagedBaseStations";

	public const string k_pch_Lighthouse_PowerManagedBaseStations2_String = "PowerManagedBaseStations2";

	public const string k_pch_Lighthouse_InactivityTimeoutForBaseStations_Int32 = "InactivityTimeoutForBaseStations";

	public const string k_pch_Lighthouse_EnableImuFallback_Bool = "enableImuFallback";

	public const string k_pch_Null_Section = "driver_null";

	public const string k_pch_Null_SerialNumber_String = "serialNumber";

	public const string k_pch_Null_ModelNumber_String = "modelNumber";

	public const string k_pch_Null_WindowX_Int32 = "windowX";

	public const string k_pch_Null_WindowY_Int32 = "windowY";

	public const string k_pch_Null_WindowWidth_Int32 = "windowWidth";

	public const string k_pch_Null_WindowHeight_Int32 = "windowHeight";

	public const string k_pch_Null_RenderWidth_Int32 = "renderWidth";

	public const string k_pch_Null_RenderHeight_Int32 = "renderHeight";

	public const string k_pch_Null_SecondsFromVsyncToPhotons_Float = "secondsFromVsyncToPhotons";

	public const string k_pch_Null_DisplayFrequency_Float = "displayFrequency";

	public const string k_pch_WindowsMR_Section = "driver_holographic";

	public const string k_pch_UserInterface_Section = "userinterface";

	public const string k_pch_UserInterface_StatusAlwaysOnTop_Bool = "StatusAlwaysOnTop";

	public const string k_pch_UserInterface_MinimizeToTray_Bool = "MinimizeToTray";

	public const string k_pch_UserInterface_HidePopupsWhenStatusMinimized_Bool = "HidePopupsWhenStatusMinimized";

	public const string k_pch_UserInterface_Screenshots_Bool = "screenshots";

	public const string k_pch_UserInterface_ScreenshotType_Int = "screenshotType";

	public const string k_pch_UserInterface_CheckStatusInterval_Int = "vrmStatusCheckInterval";

	public const string k_pch_UserInterface_CheckForSteam_Bool = "vrmCheckForSteam";

	public const string k_pch_Notifications_Section = "notifications";

	public const string k_pch_Notifications_DoNotDisturb_Bool = "DoNotDisturb";

	public const string k_pch_Keyboard_Section = "keyboard";

	public const string k_pch_Keyboard_TutorialCompletions = "TutorialCompletions";

	public const string k_pch_Keyboard_ScaleX = "ScaleX";

	public const string k_pch_Keyboard_ScaleY = "ScaleY";

	public const string k_pch_Keyboard_OffsetLeftX = "OffsetLeftX";

	public const string k_pch_Keyboard_OffsetRightX = "OffsetRightX";

	public const string k_pch_Keyboard_OffsetY = "OffsetY";

	public const string k_pch_Keyboard_Smoothing = "Smoothing";

	public const string k_pch_Perf_Section = "perfcheck";

	public const string k_pch_Perf_PerfGraphInHMD_Bool = "perfGraphInHMD";

	public const string k_pch_Perf_AllowTimingStore_Bool = "allowTimingStore";

	public const string k_pch_Perf_SaveTimingsOnExit_Bool = "saveTimingsOnExit";

	public const string k_pch_Perf_TestData_Float = "perfTestData";

	public const string k_pch_Perf_GPUProfiling_Bool = "GPUProfiling";

	public const string k_pch_Perf_GpuBusMonitoring_Bool = "gpuBusMonitoring";

	public const string k_pch_CollisionBounds_Section = "collisionBounds";

	public const string k_pch_CollisionBounds_Style_Int32 = "CollisionBoundsStyle";

	public const string k_pch_CollisionBounds_GroundPerimeterOn_Bool = "CollisionBoundsGroundPerimeterOn";

	public const string k_pch_CollisionBounds_CenterMarkerOn_Bool = "CollisionBoundsCenterMarkerOn";

	public const string k_pch_CollisionBounds_PlaySpaceOn_Bool = "CollisionBoundsPlaySpaceOn";

	public const string k_pch_CollisionBounds_FadeDistance_Float = "CollisionBoundsFadeDistance";

	public const string k_pch_CollisionBounds_WallHeight_Float = "CollisionBoundsWallHeight";

	public const string k_pch_CollisionBounds_ColorGammaR_Int32 = "CollisionBoundsColorGammaR";

	public const string k_pch_CollisionBounds_ColorGammaG_Int32 = "CollisionBoundsColorGammaG";

	public const string k_pch_CollisionBounds_ColorGammaB_Int32 = "CollisionBoundsColorGammaB";

	public const string k_pch_CollisionBounds_ColorGammaA_Int32 = "CollisionBoundsColorGammaA";

	public const string k_pch_CollisionBounds_EnableDriverImport = "enableDriverBoundsImport";

	public const string k_pch_Camera_Section = "camera";

	public const string k_pch_Camera_EnableCamera_Bool = "enableCamera";

	public const string k_pch_Camera_ShowOnController_Bool = "showOnController";

	public const string k_pch_Camera_EnableCameraForCollisionBounds_Bool = "enableCameraForCollisionBounds";

	public const string k_pch_Camera_RoomView_Int32 = "roomView";

	public const string k_pch_Camera_BoundsColorGammaR_Int32 = "cameraBoundsColorGammaR";

	public const string k_pch_Camera_BoundsColorGammaG_Int32 = "cameraBoundsColorGammaG";

	public const string k_pch_Camera_BoundsColorGammaB_Int32 = "cameraBoundsColorGammaB";

	public const string k_pch_Camera_BoundsColorGammaA_Int32 = "cameraBoundsColorGammaA";

	public const string k_pch_Camera_BoundsStrength_Int32 = "cameraBoundsStrength";

	public const string k_pch_Camera_RoomViewStyle_Int32 = "roomViewStyle";

	public const string k_pch_audio_Section = "audio";

	public const string k_pch_audio_SetOsDefaultPlaybackDevice_Bool = "setOsDefaultPlaybackDevice";

	public const string k_pch_audio_EnablePlaybackDeviceOverride_Bool = "enablePlaybackDeviceOverride";

	public const string k_pch_audio_PlaybackDeviceOverride_String = "playbackDeviceOverride";

	public const string k_pch_audio_PlaybackDeviceOverrideName_String = "playbackDeviceOverrideName";

	public const string k_pch_audio_SetOsDefaultRecordingDevice_Bool = "setOsDefaultRecordingDevice";

	public const string k_pch_audio_EnableRecordingDeviceOverride_Bool = "enableRecordingDeviceOverride";

	public const string k_pch_audio_RecordingDeviceOverride_String = "recordingDeviceOverride";

	public const string k_pch_audio_RecordingDeviceOverrideName_String = "recordingDeviceOverrideName";

	public const string k_pch_audio_EnablePlaybackMirror_Bool = "enablePlaybackMirror";

	public const string k_pch_audio_PlaybackMirrorDevice_String = "playbackMirrorDevice";

	public const string k_pch_audio_PlaybackMirrorDeviceName_String = "playbackMirrorDeviceName";

	public const string k_pch_audio_OldPlaybackMirrorDevice_String = "onPlaybackMirrorDevice";

	public const string k_pch_audio_ActiveMirrorDevice_String = "activePlaybackMirrorDevice";

	public const string k_pch_audio_EnablePlaybackMirrorIndependentVolume_Bool = "enablePlaybackMirrorIndependentVolume";

	public const string k_pch_audio_LastHmdPlaybackDeviceId_String = "lastHmdPlaybackDeviceId";

	public const string k_pch_audio_VIVEHDMIGain = "viveHDMIGain";

	public const string k_pch_audio_DualSpeakerAndJackOutput_Bool = "dualSpeakerAndJackOutput";

	public const string k_pch_audio_MuteMicMonitor_Bool = "muteMicMonitor";

	public const string k_pch_Power_Section = "power";

	public const string k_pch_Power_PowerOffOnExit_Bool = "powerOffOnExit";

	public const string k_pch_Power_TurnOffScreensTimeout_Float = "turnOffScreensTimeout";

	public const string k_pch_Power_TurnOffControllersTimeout_Float = "turnOffControllersTimeout";

	public const string k_pch_Power_ReturnToWatchdogTimeout_Float = "returnToWatchdogTimeout";

	public const string k_pch_Power_AutoLaunchSteamVROnButtonPress = "autoLaunchSteamVROnButtonPress";

	public const string k_pch_Power_PauseCompositorOnStandby_Bool = "pauseCompositorOnStandby";

	public const string k_pch_Power_OverrideWindowsPowerScheme_Bool = "overrideWindowsPowerScheme";

	public const string k_pch_Dashboard_Section = "dashboard";

	public const string k_pch_Dashboard_EnableDashboard_Bool = "enableDashboard";

	public const string k_pch_Dashboard_ArcadeMode_Bool = "arcadeMode";

	public const string k_pch_Dashboard_Position = "position";

	public const string k_pch_Dashboard_DashboardScale = "dashboardScale";

	public const string k_pch_Dashboard_UseStandaloneSystemLayer = "standaloneSystemLayer";

	public const string k_pch_Dashboard_AllowSteamOverlays_Bool = "allowSteamOverlays";

	public const string k_pch_Dashboard_AllowVRGamepadUI_Bool = "allowVRGamepadUI";

	public const string k_pch_Dashboard_SteamMatchesHMDFramerate = "steamMatchesHMDFramerate";

	public const string k_pch_Dashboard_GrabHandleAcceleration = "grabHandleAcceleration";

	public const string k_pch_Dashboard_OverlayBacksideColor_String = "overlayBacksideColor";

	public const string k_pch_modelskin_Section = "modelskins";

	public const string k_pch_Driver_Enable_Bool = "enable";

	public const string k_pch_Driver_BlockedBySafemode_Bool = "blocked_by_safe_mode";

	public const string k_pch_Driver_LoadPriority_Int32 = "loadPriority";

	public const string k_pch_Driver_Hmd_AllowsClientToControlTextureIndex_Bool = "hmdAllowsClientToControlTextureIndex";

	public const string k_pch_Driver_ForceSystemLayerUseAppPoses_Bool = "forceSystemLayerUseAppPoses";

	public const string k_pch_WebInterface_Section = "WebInterface";

	public const string k_pch_VRWebHelper_Section = "VRWebHelper";

	public const string k_pch_VRWebHelper_DebuggerEnabled_Bool = "DebuggerEnabled";

	public const string k_pch_VRWebHelper_DebuggerPort_Int32 = "DebuggerPort";

	public const string k_pch_TrackingOverride_Section = "TrackingOverrides";

	public const string k_pch_App_BindingAutosaveURLSuffix_String = "AutosaveURL";

	public const string k_pch_App_BindingLegacyAPISuffix_String = "_legacy";

	public const string k_pch_App_BindingSteamVRInputAPISuffix_String = "_steamvrinput";

	public const string k_pch_App_BindingOpenXRAPISuffix_String = "_openxr";

	public const string k_pch_App_BindingCurrentURLSuffix_String = "CurrentURL";

	public const string k_pch_App_BindingPreviousURLSuffix_String = "PreviousURL";

	public const string k_pch_App_NeedToUpdateAutosaveSuffix_Bool = "NeedToUpdateAutosave";

	public const string k_pch_App_DominantHand_Int32 = "DominantHand";

	public const string k_pch_App_BlockOculusSDK_Bool = "blockOculusSDK";

	public const string k_pch_Trackers_Section = "trackers";

	public const string k_pch_DesktopUI_Section = "DesktopUI";

	public const string k_pch_LastKnown_Section = "LastKnown";

	public const string k_pch_LastKnown_HMDManufacturer_String = "HMDManufacturer";

	public const string k_pch_LastKnown_HMDModel_String = "HMDModel";

	public const string k_pch_LastKnown_ActualHMDDriver_String = "ActualHMDDriver";

	public const string k_pch_LastKnown_HMDSerialNumber_String = "HMDSerialNumber";

	public const string k_pch_LastKnown_HMDRemoteClientID_String = "RemoteClientID";

	public const string k_pch_DismissedWarnings_Section = "DismissedWarnings";

	public const string k_pch_Input_Section = "input";

	public const string k_pch_Input_LeftThumbstickRotation_Float = "leftThumbstickRotation";

	public const string k_pch_Input_RightThumbstickRotation_Float = "rightThumbstickRotation";

	public const string k_pch_Input_ThumbstickDeadzone_Float = "thumbstickDeadzone";

	public const string k_pch_GpuSpeed_Section = "GpuSpeed";

	public const string k_pch_XRRenderModelCache_Section = "XRRenderModelUuidCache";

	public const string IVRScreenshots_Version = "IVRScreenshots_001";

	public const string IVRResources_Version = "IVRResources_001";

	public const string IVRDriverManager_Version = "IVRDriverManager_001";

	public const uint k_unMaxActionNameLength = 64u;

	public const uint k_unMaxActionSetNameLength = 64u;

	public const uint k_unMaxActionOriginCount = 16u;

	public const uint k_unMaxBoneNameLength = 32u;

	public const int k_nActionSetOverlayGlobalPriorityMin = 16777216;

	public const int k_nActionSetOverlayGlobalPriorityMax = 33554431;

	public const int k_nActionSetPriorityReservedMin = 33554432;

	public const string IVRInput_Version = "IVRInput_011";

	public const ulong k_ulInvalidIOBufferHandle = 0uL;

	public const string IVRIOBuffer_Version = "IVRIOBuffer_002";

	public const uint k_ulInvalidSpatialAnchorHandle = 0u;

	public const string IVRSpatialAnchors_Version = "IVRSpatialAnchors_001";

	public const string IVRDebug_Version = "IVRDebug_001";

	public const string IVRIPCResourceManagerClient_Version = "IVRIPCResourceManagerClient_003";

	public const uint k_nSteamVRVersionMajor = 2u;

	public const uint k_nSteamVRVersionMinor = 15u;

	public const uint k_nSteamVRVersionBuild = 6u;

	public const ulong k_ulDisplayRedirectContainer = 25769803779uL;

	public const string IVRProperties_Version = "IVRProperties_001";

	public const string k_pchPathUserHandRight = "/user/hand/right";

	public const string k_pchPathUserHandLeft = "/user/hand/left";

	public const string k_pchPathUserHandPrimary = "/user/hand/primary";

	public const string k_pchPathUserHandSecondary = "/user/hand/secondary";

	public const string k_pchPathUserHead = "/user/head";

	public const string k_pchPathUserGamepad = "/user/gamepad";

	public const string k_pchPathUserTreadmill = "/user/treadmill";

	public const string k_pchPathUserStylus = "/user/stylus";

	public const string k_pchPathDevices = "/devices";

	public const string k_pchPathDevicePath = "/device_path";

	public const string k_pchPathBestAliasPath = "/best_alias_path";

	public const string k_pchPathBoundTrackerAliasPath = "/bound_tracker_path";

	public const string k_pchPathBoundTrackerRole = "/bound_tracker_role";

	public const string k_pchPathPoseRaw = "/pose/raw";

	public const string k_pchPathPoseTip = "/pose/tip";

	public const string k_pchPathPoseGrip = "/pose/grip";

	public const string k_pchPathSystemButtonClick = "/input/system/click";

	public const string k_pchPathProximity = "/proximity";

	public const string k_pchPathControllerTypePrefix = "/controller_type/";

	public const string k_pchPathInputProfileSuffix = "/input_profile";

	public const string k_pchPathBindingNameSuffix = "/binding_name";

	public const string k_pchPathBindingUrlSuffix = "/binding_url";

	public const string k_pchPathBindingErrorSuffix = "/binding_error";

	public const string k_pchPathActiveActionSets = "/active_action_sets";

	public const string k_pchPathComponentUpdates = "/total_component_updates";

	public const string k_pchPathUserFootLeft = "/user/foot/left";

	public const string k_pchPathUserFootRight = "/user/foot/right";

	public const string k_pchPathUserShoulderLeft = "/user/shoulder/left";

	public const string k_pchPathUserShoulderRight = "/user/shoulder/right";

	public const string k_pchPathUserElbowLeft = "/user/elbow/left";

	public const string k_pchPathUserElbowRight = "/user/elbow/right";

	public const string k_pchPathUserKneeLeft = "/user/knee/left";

	public const string k_pchPathUserKneeRight = "/user/knee/right";

	public const string k_pchPathUserWristLeft = "/user/wrist/left";

	public const string k_pchPathUserWristRight = "/user/wrist/right";

	public const string k_pchPathUserAnkleLeft = "/user/ankle/left";

	public const string k_pchPathUserAnkleRight = "/user/ankle/right";

	public const string k_pchPathUserWaist = "/user/waist";

	public const string k_pchPathUserChest = "/user/chest";

	public const string k_pchPathUserCamera = "/user/camera";

	public const string k_pchPathUserKeyboard = "/user/keyboard";

	public const string k_pchPathClientAppKey = "/client_info/app_key";

	public const ulong k_ulInvalidPathHandle = 0uL;

	public const string IVRPaths_Version = "IVRPaths_002";

	public const string IVRBlockQueue_Version = "IVRBlockQueue_005";

	private const string FnTable_Prefix = "FnTable:";

	private static COpenVRContext _OpenVRInternal_ModuleContext;

	private static uint VRToken { get; set; }

	private static COpenVRContext OpenVRInternal_ModuleContext
	{
		get
		{
			if (_OpenVRInternal_ModuleContext == null)
			{
				_OpenVRInternal_ModuleContext = new COpenVRContext();
			}
			return _OpenVRInternal_ModuleContext;
		}
	}

	public static CVRSystem System => OpenVRInternal_ModuleContext.VRSystem();

	public static CVRChaperone Chaperone => OpenVRInternal_ModuleContext.VRChaperone();

	public static CVRChaperoneSetup ChaperoneSetup => OpenVRInternal_ModuleContext.VRChaperoneSetup();

	public static CVRCompositor Compositor => OpenVRInternal_ModuleContext.VRCompositor();

	public static CVRHeadsetView HeadsetView => OpenVRInternal_ModuleContext.VRHeadsetView();

	public static CVROverlay Overlay => OpenVRInternal_ModuleContext.VROverlay();

	public static CVROverlayView OverlayView => OpenVRInternal_ModuleContext.VROverlayView();

	public static CVRRenderModels RenderModels => OpenVRInternal_ModuleContext.VRRenderModels();

	public static CVRResources Resources => OpenVRInternal_ModuleContext.VRResources();

	public static CVRExtendedDisplay ExtendedDisplay => OpenVRInternal_ModuleContext.VRExtendedDisplay();

	public static CVRSettings Settings => OpenVRInternal_ModuleContext.VRSettings();

	public static CVRApplications Applications => OpenVRInternal_ModuleContext.VRApplications();

	public static CVRScreenshots Screenshots => OpenVRInternal_ModuleContext.VRScreenshots();

	public static CVRTrackedCamera TrackedCamera => OpenVRInternal_ModuleContext.VRTrackedCamera();

	public static CVRInput Input => OpenVRInternal_ModuleContext.VRInput();

	public static CVRIOBuffer IOBuffer => OpenVRInternal_ModuleContext.VRIOBuffer();

	public static CVRSpatialAnchors SpatialAnchors => OpenVRInternal_ModuleContext.VRSpatialAnchors();

	public static CVRNotifications Notifications => OpenVRInternal_ModuleContext.VRNotifications();

	public static CVRDebug Debug => OpenVRInternal_ModuleContext.VRDebug();

	public static uint InitInternal(ref EVRInitError peError, EVRApplicationType eApplicationType)
	{
		return OpenVRInterop.InitInternal(ref peError, eApplicationType);
	}

	public static uint InitInternal2(ref EVRInitError peError, EVRApplicationType eApplicationType, string pchStartupInfo)
	{
		return OpenVRInterop.InitInternal2(ref peError, eApplicationType, pchStartupInfo);
	}

	public static void ShutdownInternal()
	{
		OpenVRInterop.ShutdownInternal();
	}

	public static bool IsHmdPresent()
	{
		return OpenVRInterop.IsHmdPresent();
	}

	public static bool IsRuntimeInstalled()
	{
		return OpenVRInterop.IsRuntimeInstalled();
	}

	public static string RuntimePath()
	{
		try
		{
			uint num = 512u;
			uint punRequiredBufferSize = 512u;
			StringBuilder stringBuilder = new StringBuilder((int)num);
			if (!OpenVRInterop.GetRuntimePath(stringBuilder, num, ref punRequiredBufferSize))
			{
				return null;
			}
			return stringBuilder.ToString();
		}
		catch
		{
			return OpenVRInterop.RuntimePath();
		}
	}

	public static string GetStringForHmdError(EVRInitError error)
	{
		return Marshal.PtrToStringAnsi(OpenVRInterop.GetStringForHmdError(error));
	}

	public static IntPtr GetGenericInterface(string pchInterfaceVersion, ref EVRInitError peError)
	{
		return OpenVRInterop.GetGenericInterface(pchInterfaceVersion, ref peError);
	}

	public static bool IsInterfaceVersionValid(string pchInterfaceVersion)
	{
		return OpenVRInterop.IsInterfaceVersionValid(pchInterfaceVersion);
	}

	public static uint GetInitToken()
	{
		return OpenVRInterop.GetInitToken();
	}

	public static CVRSystem Init(ref EVRInitError peError, EVRApplicationType eApplicationType = EVRApplicationType.VRApplication_Scene, string pchStartupInfo = "")
	{
		CVRSystem cVRSystem = null;
		try
		{
			VRToken = InitInternal2(ref peError, eApplicationType, pchStartupInfo);
		}
		catch (EntryPointNotFoundException)
		{
			VRToken = InitInternal(ref peError, eApplicationType);
		}
		OpenVRInternal_ModuleContext.Clear();
		if (peError == EVRInitError.None && !IsInterfaceVersionValid("IVRSystem_026"))
		{
			peError = EVRInitError.Init_InterfaceNotFound;
		}
		if (peError == EVRInitError.None)
		{
			cVRSystem = System;
			peError = cVRSystem.SetSDKVersion(2u, 15u, 6u);
		}
		if (peError != EVRInitError.None)
		{
			cVRSystem = null;
			ShutdownInternal();
		}
		return cVRSystem;
	}

	public static void Shutdown()
	{
		ShutdownInternal();
	}
}
