using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVRIPCResourceManagerClient
{
	private IVRIPCResourceManagerClient FnTable;

	internal CVRIPCResourceManagerClient(IntPtr pInterface)
	{
		FnTable = (IVRIPCResourceManagerClient)Marshal.PtrToStructure(pInterface, typeof(IVRIPCResourceManagerClient));
	}

	public bool NewSharedVulkanImage(uint nImageFormat, uint nWidth, uint nHeight, bool bRenderable, bool bMappable, bool bComputeAccess, uint unMipLevels, uint unArrayLayerCount, uint unAdditionalVkCreateFlags, uint unAdditionalVkUsageFlags, ref ulong pSharedHandle)
	{
		pSharedHandle = 0uL;
		return FnTable.NewSharedVulkanImage(nImageFormat, nWidth, nHeight, bRenderable, bMappable, bComputeAccess, unMipLevels, unArrayLayerCount, unAdditionalVkCreateFlags, unAdditionalVkUsageFlags, ref pSharedHandle);
	}

	public bool NewSharedVulkanBuffer(uint nSize, uint nUsageFlags, ref ulong pSharedHandle)
	{
		pSharedHandle = 0uL;
		return FnTable.NewSharedVulkanBuffer(nSize, nUsageFlags, ref pSharedHandle);
	}

	public bool NewSharedVulkanSemaphore(bool bCounting, ref ulong pSharedHandle)
	{
		pSharedHandle = 0uL;
		return FnTable.NewSharedVulkanSemaphore(bCounting, ref pSharedHandle);
	}

	public bool RefResource(ulong hSharedHandle, ref ulong pNewIpcHandle)
	{
		pNewIpcHandle = 0uL;
		return FnTable.RefResource(hSharedHandle, ref pNewIpcHandle);
	}

	public bool UnrefResource(ulong hSharedHandle)
	{
		return FnTable.UnrefResource(hSharedHandle);
	}

	public bool GetDmabufFormats(ref uint pOutFormatCount, ref uint pOutFormats)
	{
		pOutFormatCount = 0u;
		pOutFormats = 0u;
		return FnTable.GetDmabufFormats(ref pOutFormatCount, ref pOutFormats);
	}

	public bool GetDmabufModifiers(EVRApplicationType eApplicationType, uint unDRMFormat, ref uint pOutModifierCount, ref ulong pOutModifiers)
	{
		pOutModifierCount = 0u;
		pOutModifiers = 0uL;
		return FnTable.GetDmabufModifiers(eApplicationType, unDRMFormat, ref pOutModifierCount, ref pOutModifiers);
	}

	public bool ImportDmabuf(EVRApplicationType eApplicationType, ref DmabufAttributes_t pDmabufAttributes, ref ulong pSharedHandle)
	{
		pSharedHandle = 0uL;
		return FnTable.ImportDmabuf(eApplicationType, ref pDmabufAttributes, ref pSharedHandle);
	}

	public bool ReceiveSharedFd(ulong ulIpcHandle, ref int pOutFd)
	{
		pOutFd = 0;
		return FnTable.ReceiveSharedFd(ulIpcHandle, ref pOutFd);
	}

	public void DestructIVRIPCResourceManagerClient()
	{
		FnTable.DestructIVRIPCResourceManagerClient();
	}
}
