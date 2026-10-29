using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVROverlay
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _PollNextOverlayEventPacked(ulong ulOverlayHandle, ref VREvent_t_Packed pEvent, uint uncbVREvent);

	[StructLayout(LayoutKind.Explicit)]
	private struct PollNextOverlayEventUnion
	{
		[FieldOffset(0)]
		public IVROverlay._PollNextOverlayEvent pPollNextOverlayEvent;

		[FieldOffset(0)]
		public _PollNextOverlayEventPacked pPollNextOverlayEventPacked;
	}

	private IVROverlay FnTable;

	internal CVROverlay(IntPtr pInterface)
	{
		FnTable = (IVROverlay)Marshal.PtrToStructure(pInterface, typeof(IVROverlay));
	}

	public EVROverlayError FindOverlay(string pchOverlayKey, ref ulong pOverlayHandle)
	{
		IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
		pOverlayHandle = 0uL;
		EVROverlayError result = FnTable.FindOverlay(intPtr, ref pOverlayHandle);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVROverlayError CreateOverlay(string pchOverlayKey, string pchOverlayName, ref ulong pOverlayHandle)
	{
		IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
		IntPtr intPtr2 = Utils.ToUtf8(pchOverlayName);
		pOverlayHandle = 0uL;
		EVROverlayError result = FnTable.CreateOverlay(intPtr, intPtr2, ref pOverlayHandle);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVROverlayError CreateSubviewOverlay(ulong parentOverlayHandle, string pchSubviewOverlayKey, string pchSubviewOverlayName, ref ulong pSubviewOverlayHandle)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSubviewOverlayKey);
		IntPtr intPtr2 = Utils.ToUtf8(pchSubviewOverlayName);
		pSubviewOverlayHandle = 0uL;
		EVROverlayError result = FnTable.CreateSubviewOverlay(parentOverlayHandle, intPtr, intPtr2, ref pSubviewOverlayHandle);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVROverlayError DestroyOverlay(ulong ulOverlayHandle)
	{
		return FnTable.DestroyOverlay(ulOverlayHandle);
	}

	public uint GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
	{
		return FnTable.GetOverlayKey(ulOverlayHandle, pchValue, unBufferSize, ref pError);
	}

	public uint GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
	{
		return FnTable.GetOverlayName(ulOverlayHandle, pchValue, unBufferSize, ref pError);
	}

	public EVROverlayError SetOverlayName(ulong ulOverlayHandle, string pchName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchName);
		EVROverlayError result = FnTable.SetOverlayName(ulOverlayHandle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVROverlayError GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight)
	{
		punWidth = 0u;
		punHeight = 0u;
		return FnTable.GetOverlayImageData(ulOverlayHandle, pvBuffer, unBufferSize, ref punWidth, ref punHeight);
	}

	public string GetOverlayErrorNameFromEnum(EVROverlayError error)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetOverlayErrorNameFromEnum(error));
	}

	public EVROverlayError SetOverlayRenderingPid(ulong ulOverlayHandle, uint unPID)
	{
		return FnTable.SetOverlayRenderingPid(ulOverlayHandle, unPID);
	}

	public uint GetOverlayRenderingPid(ulong ulOverlayHandle)
	{
		return FnTable.GetOverlayRenderingPid(ulOverlayHandle);
	}

	public EVROverlayError SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, bool bEnabled)
	{
		return FnTable.SetOverlayFlag(ulOverlayHandle, eOverlayFlag, bEnabled);
	}

	public EVROverlayError GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, ref bool pbEnabled)
	{
		pbEnabled = false;
		return FnTable.GetOverlayFlag(ulOverlayHandle, eOverlayFlag, ref pbEnabled);
	}

	public EVROverlayError GetOverlayFlags(ulong ulOverlayHandle, ref uint pFlags)
	{
		pFlags = 0u;
		return FnTable.GetOverlayFlags(ulOverlayHandle, ref pFlags);
	}

	public EVROverlayError SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue)
	{
		return FnTable.SetOverlayColor(ulOverlayHandle, fRed, fGreen, fBlue);
	}

	public EVROverlayError GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue)
	{
		pfRed = 0f;
		pfGreen = 0f;
		pfBlue = 0f;
		return FnTable.GetOverlayColor(ulOverlayHandle, ref pfRed, ref pfGreen, ref pfBlue);
	}

	public EVROverlayError SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha)
	{
		return FnTable.SetOverlayAlpha(ulOverlayHandle, fAlpha);
	}

	public EVROverlayError GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha)
	{
		pfAlpha = 0f;
		return FnTable.GetOverlayAlpha(ulOverlayHandle, ref pfAlpha);
	}

	public EVROverlayError SetOverlayTexelAspect(ulong ulOverlayHandle, float fTexelAspect)
	{
		return FnTable.SetOverlayTexelAspect(ulOverlayHandle, fTexelAspect);
	}

	public EVROverlayError GetOverlayTexelAspect(ulong ulOverlayHandle, ref float pfTexelAspect)
	{
		pfTexelAspect = 0f;
		return FnTable.GetOverlayTexelAspect(ulOverlayHandle, ref pfTexelAspect);
	}

	public EVROverlayError SetOverlaySortOrder(ulong ulOverlayHandle, uint unSortOrder)
	{
		return FnTable.SetOverlaySortOrder(ulOverlayHandle, unSortOrder);
	}

	public EVROverlayError GetOverlaySortOrder(ulong ulOverlayHandle, ref uint punSortOrder)
	{
		punSortOrder = 0u;
		return FnTable.GetOverlaySortOrder(ulOverlayHandle, ref punSortOrder);
	}

	public EVROverlayError SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters)
	{
		return FnTable.SetOverlayWidthInMeters(ulOverlayHandle, fWidthInMeters);
	}

	public EVROverlayError GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters)
	{
		pfWidthInMeters = 0f;
		return FnTable.GetOverlayWidthInMeters(ulOverlayHandle, ref pfWidthInMeters);
	}

	public EVROverlayError SetOverlayCurvature(ulong ulOverlayHandle, float fCurvature)
	{
		return FnTable.SetOverlayCurvature(ulOverlayHandle, fCurvature);
	}

	public EVROverlayError GetOverlayCurvature(ulong ulOverlayHandle, ref float pfCurvature)
	{
		pfCurvature = 0f;
		return FnTable.GetOverlayCurvature(ulOverlayHandle, ref pfCurvature);
	}

	public EVROverlayError SetOverlayPreCurvePitch(ulong ulOverlayHandle, float fRadians)
	{
		return FnTable.SetOverlayPreCurvePitch(ulOverlayHandle, fRadians);
	}

	public EVROverlayError GetOverlayPreCurvePitch(ulong ulOverlayHandle, ref float pfRadians)
	{
		pfRadians = 0f;
		return FnTable.GetOverlayPreCurvePitch(ulOverlayHandle, ref pfRadians);
	}

	public EVROverlayError SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace)
	{
		return FnTable.SetOverlayTextureColorSpace(ulOverlayHandle, eTextureColorSpace);
	}

	public EVROverlayError GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace)
	{
		return FnTable.GetOverlayTextureColorSpace(ulOverlayHandle, ref peTextureColorSpace);
	}

	public EVROverlayError SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
	{
		return FnTable.SetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
	}

	public EVROverlayError GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
	{
		return FnTable.GetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
	}

	public EVROverlayError GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType)
	{
		return FnTable.GetOverlayTransformType(ulOverlayHandle, ref peTransformType);
	}

	public EVROverlayError SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
	{
		return FnTable.SetOverlayTransformAbsolute(ulOverlayHandle, eTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
	}

	public EVROverlayError GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
	{
		return FnTable.GetOverlayTransformAbsolute(ulOverlayHandle, ref peTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
	}

	public EVROverlayError SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
	{
		return FnTable.SetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, unTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
	}

	public EVROverlayError GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
	{
		punTrackedDevice = 0u;
		return FnTable.GetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, ref punTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
	}

	public EVROverlayError SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, string pchComponentName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchComponentName);
		EVROverlayError result = FnTable.SetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, unDeviceIndex, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVROverlayError GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, StringBuilder pchComponentName, uint unComponentNameSize)
	{
		punDeviceIndex = 0u;
		return FnTable.GetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, ref punDeviceIndex, pchComponentName, unComponentNameSize);
	}

	public EVROverlayError SetOverlayTransformCursor(ulong ulCursorOverlayHandle, ref HmdVector2_t pvHotspot)
	{
		return FnTable.SetOverlayTransformCursor(ulCursorOverlayHandle, ref pvHotspot);
	}

	public EVROverlayError GetOverlayTransformCursor(ulong ulOverlayHandle, ref HmdVector2_t pvHotspot)
	{
		return FnTable.GetOverlayTransformCursor(ulOverlayHandle, ref pvHotspot);
	}

	public EVROverlayError SetOverlayTransformProjection(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform, ref VROverlayProjection_t pProjection, EVREye eEye)
	{
		return FnTable.SetOverlayTransformProjection(ulOverlayHandle, eTrackingOrigin, ref pmatTrackingOriginToOverlayTransform, ref pProjection, eEye);
	}

	public EVROverlayError SetSubviewPosition(ulong ulOverlayHandle, float fX, float fY)
	{
		return FnTable.SetSubviewPosition(ulOverlayHandle, fX, fY);
	}

	public EVROverlayError ShowOverlay(ulong ulOverlayHandle)
	{
		return FnTable.ShowOverlay(ulOverlayHandle);
	}

	public EVROverlayError HideOverlay(ulong ulOverlayHandle)
	{
		return FnTable.HideOverlay(ulOverlayHandle);
	}

	public bool IsOverlayVisible(ulong ulOverlayHandle)
	{
		return FnTable.IsOverlayVisible(ulOverlayHandle);
	}

	public EVROverlayError GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform)
	{
		return FnTable.GetTransformForOverlayCoordinates(ulOverlayHandle, eTrackingOrigin, coordinatesInOverlay, ref pmatTransform);
	}

	public EVROverlayError WaitFrameSync(uint nTimeoutMs)
	{
		return FnTable.WaitFrameSync(nTimeoutMs);
	}

	public bool PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent)
	{
		if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
		{
			VREvent_t_Packed pEvent2 = default;
			PollNextOverlayEventUnion pollNextOverlayEventUnion = default;
			pollNextOverlayEventUnion.pPollNextOverlayEventPacked = null;
			pollNextOverlayEventUnion.pPollNextOverlayEvent = FnTable.PollNextOverlayEvent;
			bool result = pollNextOverlayEventUnion.pPollNextOverlayEventPacked(ulOverlayHandle, ref pEvent2, (uint)Marshal.SizeOf(typeof(VREvent_t_Packed)));
			pEvent2.Unpack(ref pEvent);
			return result;
		}
		return FnTable.PollNextOverlayEvent(ulOverlayHandle, ref pEvent, uncbVREvent);
	}

	public EVROverlayError GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod)
	{
		return FnTable.GetOverlayInputMethod(ulOverlayHandle, ref peInputMethod);
	}

	public EVROverlayError SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod)
	{
		return FnTable.SetOverlayInputMethod(ulOverlayHandle, eInputMethod);
	}

	public EVROverlayError GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
	{
		return FnTable.GetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
	}

	public EVROverlayError SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
	{
		return FnTable.SetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
	}

	public bool ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults)
	{
		return FnTable.ComputeOverlayIntersection(ulOverlayHandle, ref pParams, ref pResults);
	}

	public bool IsHoverTargetOverlay(ulong ulOverlayHandle)
	{
		return FnTable.IsHoverTargetOverlay(ulOverlayHandle);
	}

	public EVROverlayError SetOverlayIntersectionMask(ulong ulOverlayHandle, ref VROverlayIntersectionMaskPrimitive_t pMaskPrimitives, uint unNumMaskPrimitives, uint unPrimitiveSize)
	{
		return FnTable.SetOverlayIntersectionMask(ulOverlayHandle, ref pMaskPrimitives, unNumMaskPrimitives, unPrimitiveSize);
	}

	public EVROverlayError TriggerLaserMouseHapticVibration(ulong ulOverlayHandle, float fDurationSeconds, float fFrequency, float fAmplitude)
	{
		return FnTable.TriggerLaserMouseHapticVibration(ulOverlayHandle, fDurationSeconds, fFrequency, fAmplitude);
	}

	public EVROverlayError SetOverlayCursor(ulong ulOverlayHandle, ulong ulCursorHandle)
	{
		return FnTable.SetOverlayCursor(ulOverlayHandle, ulCursorHandle);
	}

	public EVROverlayError SetOverlayCursorPositionOverride(ulong ulOverlayHandle, ref HmdVector2_t pvCursor)
	{
		return FnTable.SetOverlayCursorPositionOverride(ulOverlayHandle, ref pvCursor);
	}

	public EVROverlayError ClearOverlayCursorPositionOverride(ulong ulOverlayHandle)
	{
		return FnTable.ClearOverlayCursorPositionOverride(ulOverlayHandle);
	}

	public EVROverlayError SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture)
	{
		return FnTable.SetOverlayTexture(ulOverlayHandle, ref pTexture);
	}

	public EVROverlayError ClearOverlayTexture(ulong ulOverlayHandle)
	{
		return FnTable.ClearOverlayTexture(ulOverlayHandle);
	}

	public EVROverlayError SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unBytesPerPixel)
	{
		return FnTable.SetOverlayRaw(ulOverlayHandle, pvBuffer, unWidth, unHeight, unBytesPerPixel);
	}

	public EVROverlayError SetOverlayFromFile(ulong ulOverlayHandle, string pchFilePath)
	{
		IntPtr intPtr = Utils.ToUtf8(pchFilePath);
		EVROverlayError result = FnTable.SetOverlayFromFile(ulOverlayHandle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVROverlayError GetOverlayTexture(ulong ulOverlayHandle, ref IntPtr pNativeTextureHandle, IntPtr pNativeTextureRef, ref uint pWidth, ref uint pHeight, ref uint pNativeFormat, ref ETextureType pAPIType, ref EColorSpace pColorSpace, ref VRTextureBounds_t pTextureBounds)
	{
		pWidth = 0u;
		pHeight = 0u;
		pNativeFormat = 0u;
		return FnTable.GetOverlayTexture(ulOverlayHandle, ref pNativeTextureHandle, pNativeTextureRef, ref pWidth, ref pHeight, ref pNativeFormat, ref pAPIType, ref pColorSpace, ref pTextureBounds);
	}

	public EVROverlayError ReleaseNativeOverlayHandle(ulong ulOverlayHandle, IntPtr pNativeTextureHandle)
	{
		return FnTable.ReleaseNativeOverlayHandle(ulOverlayHandle, pNativeTextureHandle);
	}

	public EVROverlayError GetOverlayTextureSize(ulong ulOverlayHandle, ref uint pWidth, ref uint pHeight)
	{
		pWidth = 0u;
		pHeight = 0u;
		return FnTable.GetOverlayTextureSize(ulOverlayHandle, ref pWidth, ref pHeight);
	}

	public EVROverlayError CreateDashboardOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle)
	{
		IntPtr intPtr = Utils.ToUtf8(pchOverlayKey);
		IntPtr intPtr2 = Utils.ToUtf8(pchOverlayFriendlyName);
		pMainHandle = 0uL;
		pThumbnailHandle = 0uL;
		EVROverlayError result = FnTable.CreateDashboardOverlay(intPtr, intPtr2, ref pMainHandle, ref pThumbnailHandle);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public bool IsDashboardVisible()
	{
		return FnTable.IsDashboardVisible();
	}

	public bool IsActiveDashboardOverlay(ulong ulOverlayHandle)
	{
		return FnTable.IsActiveDashboardOverlay(ulOverlayHandle);
	}

	public EVROverlayError SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId)
	{
		return FnTable.SetDashboardOverlaySceneProcess(ulOverlayHandle, unProcessId);
	}

	public EVROverlayError GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId)
	{
		punProcessId = 0u;
		return FnTable.GetDashboardOverlaySceneProcess(ulOverlayHandle, ref punProcessId);
	}

	public void ShowDashboard(string pchOverlayToShow)
	{
		IntPtr intPtr = Utils.ToUtf8(pchOverlayToShow);
		FnTable.ShowDashboard(intPtr);
		Marshal.FreeHGlobal(intPtr);
	}

	public uint GetPrimaryDashboardDevice()
	{
		return FnTable.GetPrimaryDashboardDevice();
	}

	public EVROverlayError ShowKeyboard(int eInputMode, int eLineInputMode, uint unFlags, string pchDescription, uint unCharMax, string pchExistingText, ulong uUserValue)
	{
		IntPtr intPtr = Utils.ToUtf8(pchDescription);
		IntPtr intPtr2 = Utils.ToUtf8(pchExistingText);
		EVROverlayError result = FnTable.ShowKeyboard(eInputMode, eLineInputMode, unFlags, intPtr, unCharMax, intPtr2, uUserValue);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVROverlayError ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, uint unFlags, string pchDescription, uint unCharMax, string pchExistingText, ulong uUserValue)
	{
		IntPtr intPtr = Utils.ToUtf8(pchDescription);
		IntPtr intPtr2 = Utils.ToUtf8(pchExistingText);
		EVROverlayError result = FnTable.ShowKeyboardForOverlay(ulOverlayHandle, eInputMode, eLineInputMode, unFlags, intPtr, unCharMax, intPtr2, uUserValue);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public uint GetKeyboardText(StringBuilder pchText, uint cchText)
	{
		return FnTable.GetKeyboardText(pchText, cchText);
	}

	public void HideKeyboard()
	{
		FnTable.HideKeyboard();
	}

	public void SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform)
	{
		FnTable.SetKeyboardTransformAbsolute(eTrackingOrigin, ref pmatTrackingOriginToKeyboardTransform);
	}

	public void SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect)
	{
		FnTable.SetKeyboardPositionForOverlay(ulOverlayHandle, avoidRect);
	}

	public VRMessageOverlayResponse ShowMessageOverlay(string pchText, string pchCaption, string pchButton0Text, string pchButton1Text, string pchButton2Text, string pchButton3Text)
	{
		IntPtr intPtr = Utils.ToUtf8(pchText);
		IntPtr intPtr2 = Utils.ToUtf8(pchCaption);
		IntPtr intPtr3 = Utils.ToUtf8(pchButton0Text);
		IntPtr intPtr4 = Utils.ToUtf8(pchButton1Text);
		IntPtr intPtr5 = Utils.ToUtf8(pchButton2Text);
		IntPtr intPtr6 = Utils.ToUtf8(pchButton3Text);
		VRMessageOverlayResponse result = FnTable.ShowMessageOverlay(intPtr, intPtr2, intPtr3, intPtr4, intPtr5, intPtr6);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		Marshal.FreeHGlobal(intPtr3);
		Marshal.FreeHGlobal(intPtr4);
		Marshal.FreeHGlobal(intPtr5);
		Marshal.FreeHGlobal(intPtr6);
		return result;
	}

	public void CloseMessageOverlay()
	{
		FnTable.CloseMessageOverlay();
	}
}
