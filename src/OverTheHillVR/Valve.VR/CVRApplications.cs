using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRApplications
{
	private IVRApplications FnTable;

	internal CVRApplications(IntPtr pInterface)
	{
		FnTable = (IVRApplications)Marshal.PtrToStructure(pInterface, typeof(IVRApplications));
	}

	public EVRApplicationError AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary)
	{
		IntPtr intPtr = Utils.ToUtf8(pchApplicationManifestFullPath);
		EVRApplicationError result = FnTable.AddApplicationManifest(intPtr, bTemporary);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRApplicationError RemoveApplicationManifest(string pchApplicationManifestFullPath)
	{
		IntPtr intPtr = Utils.ToUtf8(pchApplicationManifestFullPath);
		EVRApplicationError result = FnTable.RemoveApplicationManifest(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool IsApplicationInstalled(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		bool result = FnTable.IsApplicationInstalled(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetApplicationCount()
	{
		return FnTable.GetApplicationCount();
	}

	public EVRApplicationError GetApplicationKeyByIndex(uint unApplicationIndex, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
	{
		return FnTable.GetApplicationKeyByIndex(unApplicationIndex, pchAppKeyBuffer, unAppKeyBufferLen);
	}

	public EVRApplicationError GetApplicationKeyByProcessId(uint unProcessId, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
	{
		return FnTable.GetApplicationKeyByProcessId(unProcessId, pchAppKeyBuffer, unAppKeyBufferLen);
	}

	public EVRApplicationError LaunchApplication(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		EVRApplicationError result = FnTable.LaunchApplication(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRApplicationError LaunchTemplateApplication(string pchTemplateAppKey, string pchNewAppKey, AppOverrideKeys_t[] pKeys)
	{
		IntPtr intPtr = Utils.ToUtf8(pchTemplateAppKey);
		IntPtr intPtr2 = Utils.ToUtf8(pchNewAppKey);
		EVRApplicationError result = FnTable.LaunchTemplateApplication(intPtr, intPtr2, pKeys, (uint)pKeys.Length);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVRApplicationError LaunchApplicationFromMimeType(string pchMimeType, string pchArgs)
	{
		IntPtr intPtr = Utils.ToUtf8(pchMimeType);
		IntPtr intPtr2 = Utils.ToUtf8(pchArgs);
		EVRApplicationError result = FnTable.LaunchApplicationFromMimeType(intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVRApplicationError LaunchDashboardOverlay(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		EVRApplicationError result = FnTable.LaunchDashboardOverlay(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool CancelApplicationLaunch(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		bool result = FnTable.CancelApplicationLaunch(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRApplicationError IdentifyApplication(uint unProcessId, string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		EVRApplicationError result = FnTable.IdentifyApplication(unProcessId, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetApplicationProcessId(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		uint result = FnTable.GetApplicationProcessId(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public string GetApplicationsErrorNameFromEnum(EVRApplicationError error)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetApplicationsErrorNameFromEnum(error));
	}

	public uint GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, StringBuilder pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		uint result = FnTable.GetApplicationPropertyString(intPtr, eProperty, pchPropertyValueBuffer, unPropertyValueBufferLen, ref peError);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		bool result = FnTable.GetApplicationPropertyBool(intPtr, eProperty, ref peError);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public ulong GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		ulong result = FnTable.GetApplicationPropertyUint64(intPtr, eProperty, ref peError);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRApplicationError SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		EVRApplicationError result = FnTable.SetApplicationAutoLaunch(intPtr, bAutoLaunch);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool GetApplicationAutoLaunch(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		bool result = FnTable.GetApplicationAutoLaunch(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRApplicationError SetDefaultApplicationForMimeType(string pchAppKey, string pchMimeType)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		IntPtr intPtr2 = Utils.ToUtf8(pchMimeType);
		EVRApplicationError result = FnTable.SetDefaultApplicationForMimeType(intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public bool GetDefaultApplicationForMimeType(string pchMimeType, StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
	{
		IntPtr intPtr = Utils.ToUtf8(pchMimeType);
		bool result = FnTable.GetDefaultApplicationForMimeType(intPtr, pchAppKeyBuffer, unAppKeyBufferLen);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool GetApplicationSupportedMimeTypes(string pchAppKey, StringBuilder pchMimeTypesBuffer, uint unMimeTypesBuffer)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		bool result = FnTable.GetApplicationSupportedMimeTypes(intPtr, pchMimeTypesBuffer, unMimeTypesBuffer);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetApplicationsThatSupportMimeType(string pchMimeType, StringBuilder pchAppKeysThatSupportBuffer, uint unAppKeysThatSupportBuffer)
	{
		IntPtr intPtr = Utils.ToUtf8(pchMimeType);
		uint result = FnTable.GetApplicationsThatSupportMimeType(intPtr, pchAppKeysThatSupportBuffer, unAppKeysThatSupportBuffer);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetApplicationLaunchArguments(uint unHandle, StringBuilder pchArgs, uint unArgs)
	{
		return FnTable.GetApplicationLaunchArguments(unHandle, pchArgs, unArgs);
	}

	public EVRApplicationError GetStartingApplication(StringBuilder pchAppKeyBuffer, uint unAppKeyBufferLen)
	{
		return FnTable.GetStartingApplication(pchAppKeyBuffer, unAppKeyBufferLen);
	}

	public EVRSceneApplicationState GetSceneApplicationState()
	{
		return FnTable.GetSceneApplicationState();
	}

	public EVRApplicationError PerformApplicationPrelaunchCheck(string pchAppKey)
	{
		IntPtr intPtr = Utils.ToUtf8(pchAppKey);
		EVRApplicationError result = FnTable.PerformApplicationPrelaunchCheck(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public string GetSceneApplicationStateNameFromEnum(EVRSceneApplicationState state)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetSceneApplicationStateNameFromEnum(state));
	}

	public EVRApplicationError LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory)
	{
		IntPtr intPtr = Utils.ToUtf8(pchBinaryPath);
		IntPtr intPtr2 = Utils.ToUtf8(pchArguments);
		IntPtr intPtr3 = Utils.ToUtf8(pchWorkingDirectory);
		EVRApplicationError result = FnTable.LaunchInternalProcess(intPtr, intPtr2, intPtr3);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		Marshal.FreeHGlobal(intPtr3);
		return result;
	}

	public EVRApplicationError RegisterSubprocess(uint nPid)
	{
		return FnTable.RegisterSubprocess(nPid);
	}

	public uint GetCurrentSceneProcessId()
	{
		return FnTable.GetCurrentSceneProcessId();
	}
}
