using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRInput
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _SetActionManifestPath(IntPtr pchActionManifestPath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetActionSetHandle(IntPtr pchActionSetName, ref ulong pHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetActionHandle(IntPtr pchActionName, ref ulong pHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetInputSourceHandle(IntPtr pchInputSourcePath, ref ulong pHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _UpdateActionState([In][Out] VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, uint unSetCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetDigitalActionData(ulong action, ref InputDigitalActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetAnalogActionData(ulong action, ref InputAnalogActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetPoseActionDataRelativeToNow(ulong action, ETrackingUniverseOrigin eOrigin, float fPredictedSecondsFromNow, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetPoseActionDataForNextFrame(ulong action, ETrackingUniverseOrigin eOrigin, ref InputPoseActionData_t pActionData, uint unActionDataSize, ulong ulRestrictToDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalActionData(ulong action, ref InputSkeletalActionData_t pActionData, uint unActionDataSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetDominantHand(ref ETrackedControllerRole peDominantHand);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _SetDominantHand(ETrackedControllerRole eDominantHand);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetEyeTrackingDataRelativeToNow(ulong action, ETrackingUniverseOrigin eOrigin, float fPredictedSecondsFromNow, ref VREyeTrackingData_t pEyeTrackingData, uint ulEyeTrackingDataSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetEyeTrackingDataForNextFrame(ulong action, ETrackingUniverseOrigin eOrigin, ref VREyeTrackingData_t pEyeTrackingData, uint ulEyeTrackingDataSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetBoneCount(ulong action, ref uint pBoneCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetBoneHierarchy(ulong action, [In][Out] int[] pParentIndices, uint unIndexArayCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetBoneName(ulong action, int nBoneIndex, StringBuilder pchBoneName, uint unNameBufferSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalReferenceTransforms(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalReferencePose eReferencePose, [In][Out] VRBoneTransform_t[] pTransformArray, uint unTransformArrayCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalTrackingLevel(ulong action, ref EVRSkeletalTrackingLevel pSkeletalTrackingLevel);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalBoneData(ulong action, EVRSkeletalTransformSpace eTransformSpace, EVRSkeletalMotionRange eMotionRange, [In][Out] VRBoneTransform_t[] pTransformArray, uint unTransformArrayCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalSummaryData(ulong action, EVRSummaryType eSummaryType, ref VRSkeletalSummaryData_t pSkeletalSummaryData);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetSkeletalBoneDataCompressed(ulong action, EVRSkeletalMotionRange eMotionRange, IntPtr pvCompressedData, uint unCompressedSize, ref uint punRequiredCompressedSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _DecompressSkeletalBoneData(IntPtr pvCompressedBuffer, uint unCompressedBufferSize, EVRSkeletalTransformSpace eTransformSpace, [In][Out] VRBoneTransform_t[] pTransformArray, uint unTransformArrayCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _TriggerHapticVibrationAction(ulong action, float fStartSecondsFromNow, float fDurationSeconds, float fFrequency, float fAmplitude, ulong ulRestrictToDevice);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetActionOrigins(ulong actionSetHandle, ulong digitalActionHandle, [In][Out] ulong[] originsOut, uint originOutCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetOriginLocalizedName(ulong origin, StringBuilder pchNameArray, uint unNameArraySize, int unStringSectionsToInclude);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetOriginTrackedDeviceInfo(ulong origin, ref InputOriginInfo_t pOriginInfo, uint unOriginInfoSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetActionBindingInfo(ulong action, [In][Out] InputBindingInfo_t[] pOriginInfo, uint unBindingInfoSize, uint unBindingInfoCount, ref uint punReturnedBindingInfoCount);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _ShowActionOrigins(ulong actionSetHandle, ulong ulActionHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _ShowBindingsForActionSet([In][Out] VRActiveActionSet_t[] pSets, uint unSizeOfVRSelectedActionSet_t, uint unSetCount, ulong originToHighlight);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetComponentStateForBinding(IntPtr pchRenderModelName, IntPtr pchComponentName, ref InputBindingInfo_t pOriginInfo, uint unBindingInfoSize, uint unBindingInfoCount, ref RenderModel_ComponentState_t pComponentState);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsUsingLegacyInput();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _OpenBindingUI(IntPtr pchAppKey, ulong ulActionSetHandle, ulong ulDeviceHandle, [MarshalAs(UnmanagedType.I1)] bool bShowOnDesktop);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRInputError _GetBindingVariant(ulong ulDevicePath, StringBuilder pchVariantArray, uint unVariantArraySize);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetActionManifestPath SetActionManifestPath;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetActionSetHandle GetActionSetHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetActionHandle GetActionHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetInputSourceHandle GetInputSourceHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _UpdateActionState UpdateActionState;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDigitalActionData GetDigitalActionData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetAnalogActionData GetAnalogActionData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetPoseActionDataRelativeToNow GetPoseActionDataRelativeToNow;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetPoseActionDataForNextFrame GetPoseActionDataForNextFrame;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalActionData GetSkeletalActionData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDominantHand GetDominantHand;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetDominantHand SetDominantHand;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetEyeTrackingDataRelativeToNow GetEyeTrackingDataRelativeToNow;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetEyeTrackingDataForNextFrame GetEyeTrackingDataForNextFrame;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetBoneCount GetBoneCount;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetBoneHierarchy GetBoneHierarchy;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetBoneName GetBoneName;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalReferenceTransforms GetSkeletalReferenceTransforms;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalTrackingLevel GetSkeletalTrackingLevel;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalBoneData GetSkeletalBoneData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalSummaryData GetSkeletalSummaryData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSkeletalBoneDataCompressed GetSkeletalBoneDataCompressed;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _DecompressSkeletalBoneData DecompressSkeletalBoneData;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _TriggerHapticVibrationAction TriggerHapticVibrationAction;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetActionOrigins GetActionOrigins;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOriginLocalizedName GetOriginLocalizedName;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetOriginTrackedDeviceInfo GetOriginTrackedDeviceInfo;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetActionBindingInfo GetActionBindingInfo;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowActionOrigins ShowActionOrigins;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ShowBindingsForActionSet ShowBindingsForActionSet;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetComponentStateForBinding GetComponentStateForBinding;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsUsingLegacyInput IsUsingLegacyInput;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _OpenBindingUI OpenBindingUI;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetBindingVariant GetBindingVariant;
}
