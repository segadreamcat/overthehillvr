using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRApplications
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _AddApplicationManifest(IntPtr pchApplicationManifestFullPath, [MarshalAs(UnmanagedType.I1)] bool bTemporary);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _RemoveApplicationManifest(IntPtr pchApplicationManifestFullPath);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsApplicationInstalled(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetApplicationCount();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _GetApplicationKeyByIndex(uint unApplicationIndex, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _GetApplicationKeyByProcessId(uint unProcessId, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _LaunchApplication(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _LaunchTemplateApplication(IntPtr pchTemplateAppKey, IntPtr pchNewAppKey, [In][Out] AppOverrideKeys_t[] pKeys, uint unKeys);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _LaunchApplicationFromMimeType(IntPtr pchMimeType, IntPtr pchArgs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _LaunchDashboardOverlay(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _CancelApplicationLaunch(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _IdentifyApplication(uint unProcessId, IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetApplicationProcessId(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate IntPtr _GetApplicationsErrorNameFromEnum(EVRApplicationError error);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetApplicationPropertyString(IntPtr pchAppKey, EVRApplicationProperty eProperty, StringBuilder pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetApplicationPropertyBool(IntPtr pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ulong _GetApplicationPropertyUint64(IntPtr pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _SetApplicationAutoLaunch(IntPtr pchAppKey, [MarshalAs(UnmanagedType.I1)] bool bAutoLaunch);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetApplicationAutoLaunch(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _SetDefaultApplicationForMimeType(IntPtr pchAppKey, IntPtr pchMimeType);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetDefaultApplicationForMimeType(IntPtr pchMimeType, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetApplicationSupportedMimeTypes(IntPtr pchAppKey, StringBuilder pchMimeTypesBuffer, uint unMimeTypesBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetApplicationsThatSupportMimeType(IntPtr pchMimeType, StringBuilder pchAppKeysThatSupportBuffer, uint unAppKeysThatSupportBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetApplicationLaunchArguments(uint unHandle, StringBuilder pchArgs, uint unArgs);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _GetStartingApplication(StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRSceneApplicationState _GetSceneApplicationState();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _PerformApplicationPrelaunchCheck(IntPtr pchAppKey);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate IntPtr _GetSceneApplicationStateNameFromEnum(EVRSceneApplicationState state);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _LaunchInternalProcess(IntPtr pchBinaryPath, IntPtr pchArguments, IntPtr pchWorkingDirectory);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRApplicationError _RegisterSubprocess(uint nPid);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetCurrentSceneProcessId();

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _AddApplicationManifest AddApplicationManifest;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RemoveApplicationManifest RemoveApplicationManifest;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsApplicationInstalled IsApplicationInstalled;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationCount GetApplicationCount;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationKeyByIndex GetApplicationKeyByIndex;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationKeyByProcessId GetApplicationKeyByProcessId;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LaunchApplication LaunchApplication;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LaunchTemplateApplication LaunchTemplateApplication;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LaunchApplicationFromMimeType LaunchApplicationFromMimeType;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LaunchDashboardOverlay LaunchDashboardOverlay;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CancelApplicationLaunch CancelApplicationLaunch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IdentifyApplication IdentifyApplication;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationProcessId GetApplicationProcessId;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationsErrorNameFromEnum GetApplicationsErrorNameFromEnum;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationPropertyString GetApplicationPropertyString;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationPropertyBool GetApplicationPropertyBool;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationPropertyUint64 GetApplicationPropertyUint64;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetApplicationAutoLaunch SetApplicationAutoLaunch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationAutoLaunch GetApplicationAutoLaunch;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetDefaultApplicationForMimeType SetDefaultApplicationForMimeType;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDefaultApplicationForMimeType GetDefaultApplicationForMimeType;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationSupportedMimeTypes GetApplicationSupportedMimeTypes;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationsThatSupportMimeType GetApplicationsThatSupportMimeType;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetApplicationLaunchArguments GetApplicationLaunchArguments;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetStartingApplication GetStartingApplication;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSceneApplicationState GetSceneApplicationState;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _PerformApplicationPrelaunchCheck PerformApplicationPrelaunchCheck;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSceneApplicationStateNameFromEnum GetSceneApplicationStateNameFromEnum;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LaunchInternalProcess LaunchInternalProcess;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RegisterSubprocess RegisterSubprocess;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetCurrentSceneProcessId GetCurrentSceneProcessId;
}
