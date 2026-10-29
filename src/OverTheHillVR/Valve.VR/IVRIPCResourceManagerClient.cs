using System.Runtime.InteropServices;

namespace Valve.VR;

public struct IVRIPCResourceManagerClient
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _NewSharedVulkanImage(uint nImageFormat, uint nWidth, uint nHeight, [MarshalAs(UnmanagedType.I1)] bool bRenderable, [MarshalAs(UnmanagedType.I1)] bool bMappable, [MarshalAs(UnmanagedType.I1)] bool bComputeAccess, uint unMipLevels, uint unArrayLayerCount, uint unAdditionalVkCreateFlags, uint unAdditionalVkUsageFlags, ref ulong pSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _NewSharedVulkanBuffer(uint nSize, uint nUsageFlags, ref ulong pSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _NewSharedVulkanSemaphore([MarshalAs(UnmanagedType.I1)] bool bCounting, ref ulong pSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _RefResource(ulong hSharedHandle, ref ulong pNewIpcHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _UnrefResource(ulong hSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetDmabufFormats(ref uint pOutFormatCount, ref uint pOutFormats);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _GetDmabufModifiers(EVRApplicationType eApplicationType, uint unDRMFormat, ref uint pOutModifierCount, ref ulong pOutModifiers);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _ImportDmabuf(EVRApplicationType eApplicationType, ref DmabufAttributes_t pDmabufAttributes, ref ulong pSharedHandle);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	[return: MarshalAs(UnmanagedType.I1)]
	internal delegate bool _ReceiveSharedFd(ulong ulIpcHandle, ref int pOutFd);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate void _DestructIVRIPCResourceManagerClient();

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _NewSharedVulkanImage NewSharedVulkanImage;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _NewSharedVulkanBuffer NewSharedVulkanBuffer;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _NewSharedVulkanSemaphore NewSharedVulkanSemaphore;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _RefResource RefResource;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _UnrefResource UnrefResource;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDmabufFormats GetDmabufFormats;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetDmabufModifiers GetDmabufModifiers;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ImportDmabuf ImportDmabuf;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _ReceiveSharedFd ReceiveSharedFd;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _DestructIVRIPCResourceManagerClient DestructIVRIPCResourceManagerClient;
}
