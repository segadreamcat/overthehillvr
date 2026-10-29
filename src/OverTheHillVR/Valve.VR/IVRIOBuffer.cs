using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVRIOBuffer
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EIOBufferError _Open(IntPtr pchPath, EIOBufferMode mode, uint unElementSize, uint unElements, ref ulong pulBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EIOBufferError _Close(ulong ulBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EIOBufferError _Read(ulong ulBuffer, IntPtr pDst, uint unBytes, ref uint punRead);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EIOBufferError _Write(ulong ulBuffer, IntPtr pSrc, uint unBytes);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate ulong _PropertyContainer(ulong ulBuffer);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _HasReaders(ulong ulBuffer);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Open Open;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Close Close;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Read Read;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _Write Write;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _PropertyContainer PropertyContainer;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _HasReaders HasReaders;
}
