using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

public class CVRBlockQueue
{
	private IVRBlockQueue FnTable;

	internal CVRBlockQueue(IntPtr pInterface)
	{
		FnTable = (IVRBlockQueue)Marshal.PtrToStructure(pInterface, typeof(IVRBlockQueue));
	}

	public EBlockQueueError Create(ref ulong pulQueueHandle, string pchPath, uint unBlockDataSize, uint unBlockHeaderSize, uint unBlockCount, uint unFlags)
	{
		pulQueueHandle = 0uL;
		IntPtr intPtr = Utils.ToUtf8(pchPath);
		EBlockQueueError result = FnTable.Create(ref pulQueueHandle, intPtr, unBlockDataSize, unBlockHeaderSize, unBlockCount, unFlags);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EBlockQueueError Connect(ref ulong pulQueueHandle, string pchPath)
	{
		pulQueueHandle = 0uL;
		IntPtr intPtr = Utils.ToUtf8(pchPath);
		EBlockQueueError result = FnTable.Connect(ref pulQueueHandle, intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public EBlockQueueError Destroy(ulong ulQueueHandle)
	{
		return FnTable.Destroy(ulQueueHandle);
	}

	public EBlockQueueError AcquireWriteOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer)
	{
		pulBlockHandle = 0uL;
		return FnTable.AcquireWriteOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer);
	}

	public EBlockQueueError ReleaseWriteOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle)
	{
		return FnTable.ReleaseWriteOnlyBlock(ulQueueHandle, ulBlockHandle);
	}

	public EBlockQueueError WaitAndAcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType, uint unTimeoutMs)
	{
		pulBlockHandle = 0uL;
		return FnTable.WaitAndAcquireReadOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer, eReadType, unTimeoutMs);
	}

	public EBlockQueueError AcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType)
	{
		pulBlockHandle = 0uL;
		return FnTable.AcquireReadOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer, eReadType);
	}

	public EBlockQueueError ReleaseReadOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle)
	{
		return FnTable.ReleaseReadOnlyBlock(ulQueueHandle, ulBlockHandle);
	}

	public EBlockQueueError QueueHasReader(ulong ulQueueHandle, ref bool pbHasReaders)
	{
		pbHasReaders = false;
		return FnTable.QueueHasReader(ulQueueHandle, ref pbHasReaders);
	}
}
