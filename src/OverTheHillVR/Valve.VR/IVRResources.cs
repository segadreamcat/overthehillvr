using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRResources
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _LoadSharedResource(IntPtr pchResourceName, string pchBuffer, uint unBufferLen);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate uint _GetResourceFullPath(IntPtr pchResourceName, IntPtr pchResourceTypeDirectory, StringBuilder pchPathBuffer, uint unBufferLen);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _LoadSharedResource LoadSharedResource;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetResourceFullPath GetResourceFullPath;
}
