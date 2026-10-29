using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVRProperties
{
	private IVRProperties FnTable;

	internal CVRProperties(IntPtr pInterface)
	{
		FnTable = (IVRProperties)Marshal.PtrToStructure(pInterface, typeof(IVRProperties));
	}

	public ETrackedPropertyError ReadPropertyBatch(ulong ulContainerHandle, ref PropertyRead_t pBatch, uint unBatchEntryCount)
	{
		return FnTable.ReadPropertyBatch(ulContainerHandle, ref pBatch, unBatchEntryCount);
	}

	public ETrackedPropertyError WritePropertyBatch(ulong ulContainerHandle, ref PropertyWrite_t pBatch, uint unBatchEntryCount)
	{
		return FnTable.WritePropertyBatch(ulContainerHandle, ref pBatch, unBatchEntryCount);
	}

	public string GetPropErrorNameFromEnum(ETrackedPropertyError error)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetPropErrorNameFromEnum(error));
	}

	public ulong TrackedDeviceToPropertyContainer(uint nDevice)
	{
		return FnTable.TrackedDeviceToPropertyContainer(nDevice);
	}
}
