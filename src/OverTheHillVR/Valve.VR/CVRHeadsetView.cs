using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVRHeadsetView
{
	private IVRHeadsetView FnTable;

	internal CVRHeadsetView(IntPtr pInterface)
	{
		FnTable = (IVRHeadsetView)Marshal.PtrToStructure(pInterface, typeof(IVRHeadsetView));
	}

	public void SetHeadsetViewSize(uint nWidth, uint nHeight)
	{
		FnTable.SetHeadsetViewSize(nWidth, nHeight);
	}

	public void GetHeadsetViewSize(ref uint pnWidth, ref uint pnHeight)
	{
		pnWidth = 0u;
		pnHeight = 0u;
		FnTable.GetHeadsetViewSize(ref pnWidth, ref pnHeight);
	}

	public void SetHeadsetViewMode(uint eHeadsetViewMode)
	{
		FnTable.SetHeadsetViewMode(eHeadsetViewMode);
	}

	public uint GetHeadsetViewMode()
	{
		return FnTable.GetHeadsetViewMode();
	}

	public void SetHeadsetViewCropped(bool bCropped)
	{
		FnTable.SetHeadsetViewCropped(bCropped);
	}

	public bool GetHeadsetViewCropped()
	{
		return FnTable.GetHeadsetViewCropped();
	}

	public float GetHeadsetViewAspectRatio()
	{
		return FnTable.GetHeadsetViewAspectRatio();
	}

	public void SetHeadsetViewBlendRange(float flStartPct, float flEndPct)
	{
		FnTable.SetHeadsetViewBlendRange(flStartPct, flEndPct);
	}

	public void GetHeadsetViewBlendRange(ref float pStartPct, ref float pEndPct)
	{
		pStartPct = 0f;
		pEndPct = 0f;
		FnTable.GetHeadsetViewBlendRange(ref pStartPct, ref pEndPct);
	}
}
