using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRDriverManager
{
	private IVRDriverManager FnTable;

	internal CVRDriverManager(IntPtr pInterface)
	{
		FnTable = (IVRDriverManager)Marshal.PtrToStructure(pInterface, typeof(IVRDriverManager));
	}

	public uint GetDriverCount()
	{
		return FnTable.GetDriverCount();
	}

	public uint GetDriverName(uint nDriver, StringBuilder pchValue, uint unBufferSize)
	{
		return FnTable.GetDriverName(nDriver, pchValue, unBufferSize);
	}

	public ulong GetDriverHandle(string pchDriverName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchDriverName);
		ulong result = FnTable.GetDriverHandle(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public bool IsEnabled(uint nDriver)
	{
		return FnTable.IsEnabled(nDriver);
	}
}
