using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRSettings
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate IntPtr _GetSettingsErrorNameFromEnum(EVRSettingsError eError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetBool(IntPtr pchSection, IntPtr pchSettingsKey, [MarshalAs(UnmanagedType.I1)] bool bValue, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetInt32(IntPtr pchSection, IntPtr pchSettingsKey, int nValue, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetFloat(IntPtr pchSection, IntPtr pchSettingsKey, float flValue, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _SetString(IntPtr pchSection, IntPtr pchSettingsKey, IntPtr pchValue, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetBool(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate int _GetInt32(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate float _GetFloat(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _GetString(IntPtr pchSection, IntPtr pchSettingsKey, StringBuilder pchValue, uint unValueLen, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _RemoveSection(IntPtr pchSection, ref EVRSettingsError peError);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _RemoveKeyInSection(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSettingsErrorNameFromEnum GetSettingsErrorNameFromEnum;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetBool SetBool;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetInt32 SetInt32;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetFloat SetFloat;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _SetString SetString;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetBool GetBool;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetInt32 GetInt32;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetFloat GetFloat;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetString GetString;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RemoveSection RemoveSection;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RemoveKeyInSection RemoveKeyInSection;
}
