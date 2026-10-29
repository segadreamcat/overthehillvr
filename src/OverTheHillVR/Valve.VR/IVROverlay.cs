using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVROverlay
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _FindOverlay(IntPtr pchOverlayKey, ref ulong pOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _CreateOverlay(IntPtr pchOverlayKey, IntPtr pchOverlayName, ref ulong pOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _CreateSubviewOverlay(ulong parentOverlayHandle, IntPtr pchSubviewOverlayKey, IntPtr pchSubviewOverlayName, ref ulong pSubviewOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _DestroyOverlay(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayName(ulong ulOverlayHandle, IntPtr pchName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate IntPtr _GetOverlayErrorNameFromEnum(EVROverlayError error);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayRenderingPid(ulong ulOverlayHandle, uint unPID);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetOverlayRenderingPid(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, [MarshalAs(UnmanagedType.I1)] bool bEnabled);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, [MarshalAs(UnmanagedType.I1)] ref bool pbEnabled);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayFlags(ulong ulOverlayHandle, ref uint pFlags);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTexelAspect(ulong ulOverlayHandle, float fTexelAspect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTexelAspect(ulong ulOverlayHandle, ref float pfTexelAspect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlaySortOrder(ulong ulOverlayHandle, uint unSortOrder);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlaySortOrder(ulong ulOverlayHandle, ref uint punSortOrder);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayCurvature(ulong ulOverlayHandle, float fCurvature);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayCurvature(ulong ulOverlayHandle, ref float pfCurvature);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayPreCurvePitch(ulong ulOverlayHandle, float fRadians);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayPreCurvePitch(ulong ulOverlayHandle, ref float pfRadians);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, IntPtr pchComponentName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, StringBuilder pchComponentName, uint unComponentNameSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTransformCursor(ulong ulCursorOverlayHandle, ref HmdVector2_t pvHotspot);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTransformCursor(ulong ulOverlayHandle, ref HmdVector2_t pvHotspot);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTransformProjection(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform, ref VROverlayProjection_t pProjection, EVREye eEye);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetSubviewPosition(ulong ulOverlayHandle, float fX, float fY);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ShowOverlay(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _HideOverlay(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsOverlayVisible(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _WaitFrameSync(uint nTimeoutMs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsHoverTargetOverlay(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayIntersectionMask(ulong ulOverlayHandle, ref VROverlayIntersectionMaskPrimitive_t pMaskPrimitives, uint unNumMaskPrimitives, uint unPrimitiveSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _TriggerLaserMouseHapticVibration(ulong ulOverlayHandle, float fDurationSeconds, float fFrequency, float fAmplitude);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayCursor(ulong ulOverlayHandle, ulong ulCursorHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayCursorPositionOverride(ulong ulOverlayHandle, ref HmdVector2_t pvCursor);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ClearOverlayCursorPositionOverride(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ClearOverlayTexture(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unBytesPerPixel);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetOverlayFromFile(ulong ulOverlayHandle, IntPtr pchFilePath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTexture(ulong ulOverlayHandle, ref IntPtr pNativeTextureHandle, IntPtr pNativeTextureRef, ref uint pWidth, ref uint pHeight, ref uint pNativeFormat, ref ETextureType pAPIType, ref EColorSpace pColorSpace, ref VRTextureBounds_t pTextureBounds);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ReleaseNativeOverlayHandle(ulong ulOverlayHandle, IntPtr pNativeTextureHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetOverlayTextureSize(ulong ulOverlayHandle, ref uint pWidth, ref uint pHeight);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _CreateDashboardOverlay(IntPtr pchOverlayKey, IntPtr pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsDashboardVisible();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsActiveDashboardOverlay(ulong ulOverlayHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _ShowDashboard(IntPtr pchOverlayToShow);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetPrimaryDashboardDevice();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ShowKeyboard(int eInputMode, int eLineInputMode, uint unFlags, IntPtr pchDescription, uint unCharMax, IntPtr pchExistingText, ulong uUserValue);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVROverlayError _ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, uint unFlags, IntPtr pchDescription, uint unCharMax, IntPtr pchExistingText, ulong uUserValue);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetKeyboardText(StringBuilder pchText, uint cchText);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _HideKeyboard();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate VRMessageOverlayResponse _ShowMessageOverlay(IntPtr pchText, IntPtr pchCaption, IntPtr pchButton0Text, IntPtr pchButton1Text, IntPtr pchButton2Text, IntPtr pchButton3Text);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _CloseMessageOverlay();

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _FindOverlay FindOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CreateOverlay CreateOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CreateSubviewOverlay CreateSubviewOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _DestroyOverlay DestroyOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayKey GetOverlayKey;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayName GetOverlayName;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayName SetOverlayName;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayImageData GetOverlayImageData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayErrorNameFromEnum GetOverlayErrorNameFromEnum;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayRenderingPid SetOverlayRenderingPid;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayRenderingPid GetOverlayRenderingPid;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayFlag SetOverlayFlag;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayFlag GetOverlayFlag;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayFlags GetOverlayFlags;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayColor SetOverlayColor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayColor GetOverlayColor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayAlpha SetOverlayAlpha;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayAlpha GetOverlayAlpha;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTexelAspect SetOverlayTexelAspect;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTexelAspect GetOverlayTexelAspect;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlaySortOrder SetOverlaySortOrder;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlaySortOrder GetOverlaySortOrder;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayWidthInMeters SetOverlayWidthInMeters;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayWidthInMeters GetOverlayWidthInMeters;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayCurvature SetOverlayCurvature;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayCurvature GetOverlayCurvature;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayPreCurvePitch SetOverlayPreCurvePitch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayPreCurvePitch GetOverlayPreCurvePitch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTextureColorSpace SetOverlayTextureColorSpace;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTextureColorSpace GetOverlayTextureColorSpace;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTextureBounds SetOverlayTextureBounds;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTextureBounds GetOverlayTextureBounds;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTransformType GetOverlayTransformType;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTransformAbsolute SetOverlayTransformAbsolute;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTransformAbsolute GetOverlayTransformAbsolute;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTransformTrackedDeviceRelative SetOverlayTransformTrackedDeviceRelative;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTransformTrackedDeviceRelative GetOverlayTransformTrackedDeviceRelative;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTransformTrackedDeviceComponent SetOverlayTransformTrackedDeviceComponent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTransformTrackedDeviceComponent GetOverlayTransformTrackedDeviceComponent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTransformCursor SetOverlayTransformCursor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTransformCursor GetOverlayTransformCursor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTransformProjection SetOverlayTransformProjection;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetSubviewPosition SetSubviewPosition;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowOverlay ShowOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _HideOverlay HideOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsOverlayVisible IsOverlayVisible;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetTransformForOverlayCoordinates GetTransformForOverlayCoordinates;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _WaitFrameSync WaitFrameSync;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _PollNextOverlayEvent PollNextOverlayEvent;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayInputMethod GetOverlayInputMethod;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayInputMethod SetOverlayInputMethod;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayMouseScale GetOverlayMouseScale;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayMouseScale SetOverlayMouseScale;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ComputeOverlayIntersection ComputeOverlayIntersection;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsHoverTargetOverlay IsHoverTargetOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayIntersectionMask SetOverlayIntersectionMask;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _TriggerLaserMouseHapticVibration TriggerLaserMouseHapticVibration;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayCursor SetOverlayCursor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayCursorPositionOverride SetOverlayCursorPositionOverride;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ClearOverlayCursorPositionOverride ClearOverlayCursorPositionOverride;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayTexture SetOverlayTexture;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ClearOverlayTexture ClearOverlayTexture;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayRaw SetOverlayRaw;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetOverlayFromFile SetOverlayFromFile;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTexture GetOverlayTexture;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReleaseNativeOverlayHandle ReleaseNativeOverlayHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOverlayTextureSize GetOverlayTextureSize;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CreateDashboardOverlay CreateDashboardOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsDashboardVisible IsDashboardVisible;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsActiveDashboardOverlay IsActiveDashboardOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetDashboardOverlaySceneProcess SetDashboardOverlaySceneProcess;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDashboardOverlaySceneProcess GetDashboardOverlaySceneProcess;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowDashboard ShowDashboard;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetPrimaryDashboardDevice GetPrimaryDashboardDevice;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowKeyboard ShowKeyboard;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowKeyboardForOverlay ShowKeyboardForOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetKeyboardText GetKeyboardText;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _HideKeyboard HideKeyboard;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetKeyboardTransformAbsolute SetKeyboardTransformAbsolute;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetKeyboardPositionForOverlay SetKeyboardPositionForOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowMessageOverlay ShowMessageOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CloseMessageOverlay CloseMessageOverlay;
}
