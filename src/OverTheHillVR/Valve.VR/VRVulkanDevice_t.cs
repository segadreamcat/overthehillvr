using System;

namespace Valve.VR;

public struct VRVulkanDevice_t
{
	public IntPtr m_pInstance;

	public IntPtr m_pDevice;

	public IntPtr m_pPhysicalDevice;

	public IntPtr m_pQueue;

	public uint m_uQueueFamilyIndex;
}
