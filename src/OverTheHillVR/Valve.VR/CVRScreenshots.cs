using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRScreenshots
{
	private IVRScreenshots FnTable;

	internal CVRScreenshots(IntPtr pInterface)
	{
		FnTable = (IVRScreenshots)Marshal.PtrToStructure(pInterface, typeof(IVRScreenshots));
	}

	public EVRScreenshotError RequestScreenshot(ref uint pOutScreenshotHandle, EVRScreenshotType type, string pchPreviewFilename, string pchVRFilename)
	{
		pOutScreenshotHandle = 0u;
		IntPtr intPtr = Utils.ToUtf8(pchPreviewFilename);
		IntPtr intPtr2 = Utils.ToUtf8(pchVRFilename);
		EVRScreenshotError result = FnTable.RequestScreenshot(ref pOutScreenshotHandle, type, intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVRScreenshotError HookScreenshot(EVRScreenshotType[] pSupportedTypes)
	{
		return FnTable.HookScreenshot(pSupportedTypes, pSupportedTypes.Length);
	}

	public EVRScreenshotType GetScreenshotPropertyType(uint screenshotHandle, ref EVRScreenshotError pError)
	{
		return FnTable.GetScreenshotPropertyType(screenshotHandle, ref pError);
	}

	public uint GetScreenshotPropertyFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames filenameType, StringBuilder pchFilename, uint cchFilename, ref EVRScreenshotError pError)
	{
		return FnTable.GetScreenshotPropertyFilename(screenshotHandle, filenameType, pchFilename, cchFilename, ref pError);
	}

	public EVRScreenshotError UpdateScreenshotProgress(uint screenshotHandle, float flProgress)
	{
		return FnTable.UpdateScreenshotProgress(screenshotHandle, flProgress);
	}

	public EVRScreenshotError TakeStereoScreenshot(ref uint pOutScreenshotHandle, string pchPreviewFilename, string pchVRFilename)
	{
		pOutScreenshotHandle = 0u;
		IntPtr intPtr = Utils.ToUtf8(pchPreviewFilename);
		IntPtr intPtr2 = Utils.ToUtf8(pchVRFilename);
		EVRScreenshotError result = FnTable.TakeStereoScreenshot(ref pOutScreenshotHandle, intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public EVRScreenshotError SubmitScreenshot(uint screenshotHandle, EVRScreenshotType type, string pchSourcePreviewFilename, string pchSourceVRFilename)
	{
		IntPtr intPtr = Utils.ToUtf8(pchSourcePreviewFilename);
		IntPtr intPtr2 = Utils.ToUtf8(pchSourceVRFilename);
		EVRScreenshotError result = FnTable.SubmitScreenshot(screenshotHandle, type, intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}
}
