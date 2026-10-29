using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRSettings
{
	private IVRSettings FnTable;

	internal CVRSettings(IntPtr pInterface)
	{
		FnTable = (IVRSettings)Marshal.PtrToStructure(pInterface, typeof(IVRSettings));
	}

	public string GetSettingsErrorNameFromEnum(EVRSettingsError eError)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetSettingsErrorNameFromEnum(eError));
	}

	public void SetBool(string pchSection, string pchSettingsKey, bool bValue, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		FnTable.SetBool(intPtr, intPtr2, bValue, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
	}

	public void SetInt32(string pchSection, string pchSettingsKey, int nValue, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		FnTable.SetInt32(intPtr, intPtr2, nValue, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
	}

	public void SetFloat(string pchSection, string pchSettingsKey, float flValue, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		FnTable.SetFloat(intPtr, intPtr2, flValue, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
	}

	public void SetString(string pchSection, string pchSettingsKey, string pchValue, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		IntPtr intPtr3 = Utils.ToUtf8(pchValue);
		FnTable.SetString(intPtr, intPtr2, intPtr3, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		Marshal.FreeHGlobal(intPtr3);
	}

	public bool GetBool(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		bool result = FnTable.GetBool(intPtr, intPtr2, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public int GetInt32(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		int result = FnTable.GetInt32(intPtr, intPtr2, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public float GetFloat(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		float result = FnTable.GetFloat(intPtr, intPtr2, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public void GetString(string pchSection, string pchSettingsKey, StringBuilder pchValue, uint unValueLen, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		FnTable.GetString(intPtr, intPtr2, pchValue, unValueLen, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
	}

	public void RemoveSection(string pchSection, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		FnTable.RemoveSection(intPtr, ref peError);
		Marshal.FreeHGlobal(intPtr);
	}

	public void RemoveKeyInSection(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSection);
		IntPtr intPtr2 = Utils.ToUtf8(pchSettingsKey);
		FnTable.RemoveKeyInSection(intPtr, intPtr2, ref peError);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
	}
}
