using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRDriverManager
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetDriverCount();

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetDriverName(uint nDriver, StringBuilder pchValue, uint unBufferSize);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ulong _GetDriverHandle(IntPtr pchDriverName);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _IsEnabled(uint nDriver);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDriverCount GetDriverCount;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDriverName GetDriverName;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDriverHandle GetDriverHandle;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _IsEnabled IsEnabled;
}
