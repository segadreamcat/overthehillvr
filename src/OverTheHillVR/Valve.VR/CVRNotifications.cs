using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVRNotifications
{
	private IVRNotifications FnTable;

	internal CVRNotifications(IntPtr pInterface)
	{
		FnTable = (IVRNotifications)Marshal.PtrToStructure(pInterface, typeof(IVRNotifications));
	}

	public EVRNotificationError CreateNotification(ulong ulOverlayHandle, ulong ulUserValue, EVRNotificationType type, string pchText, EVRNotificationStyle style, ref NotificationBitmap_t pImage, ref uint pNotificationId)
	{
		IntPtr intPtr = Utils.ToUtf8(pchText);
		pNotificationId = 0u;
		EVRNotificationError result = FnTable.CreateNotification(ulOverlayHandle, ulUserValue, type, intPtr, style, ref pImage, ref pNotificationId);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EVRNotificationError RemoveNotification(uint notificationId)
	{
		return FnTable.RemoveNotification(notificationId);
	}
}
