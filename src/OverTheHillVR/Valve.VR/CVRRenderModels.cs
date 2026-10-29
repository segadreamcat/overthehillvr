using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public class CVRRenderModels
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate bool _GetComponentStatePacked(IntPtr pchRenderModelName, IntPtr pchComponentName, ref VRControllerState_t_Packed pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState);

	[StructLayout(LayoutKind.Explicit)]
	private struct GetComponentStateUnion
	{
		[FieldOffset(0)]
		public IVRRenderModels._GetComponentState pGetComponentState;

		[FieldOffset(0)]
		public _GetComponentStatePacked pGetComponentStatePacked;
	}

	private IVRRenderModels FnTable;

	internal CVRRenderModels(IntPtr pInterface)
	{
		FnTable = (IVRRenderModels)Marshal.PtrToStructure(pInterface, typeof(IVRRenderModels));
	}

	public EVRRenderModelError LoadRenderModel_Async(string pchRenderModelName, ref IntPtr ppRenderModel)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		EVRRenderModelError result = FnTable.LoadRenderModel_Async(intPtr, ref ppRenderModel);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public void FreeRenderModel(IntPtr pRenderModel)
	{
		FnTable.FreeRenderModel(pRenderModel);
	}

	public EVRRenderModelError LoadTexture_Async(int textureId, ref IntPtr ppTexture)
	{
		return FnTable.LoadTexture_Async(textureId, ref ppTexture);
	}

	public void FreeTexture(IntPtr pTexture)
	{
		FnTable.FreeTexture(pTexture);
	}

	public EVRRenderModelError LoadTextureD3D11_Async(int textureId, IntPtr pD3D11Device, ref IntPtr ppD3D11Texture2D)
	{
		return FnTable.LoadTextureD3D11_Async(textureId, pD3D11Device, ref ppD3D11Texture2D);
	}

	public EVRRenderModelError LoadIntoTextureD3D11_Async(int textureId, IntPtr pDstTexture)
	{
		return FnTable.LoadIntoTextureD3D11_Async(textureId, pDstTexture);
	}

	public void FreeTextureD3D11(IntPtr pD3D11Texture2D)
	{
		FnTable.FreeTextureD3D11(pD3D11Texture2D);
	}

	public uint GetRenderModelName(uint unRenderModelIndex, StringBuilder pchRenderModelName, uint unRenderModelNameLen)
	{
		return FnTable.GetRenderModelName(unRenderModelIndex, pchRenderModelName, unRenderModelNameLen);
	}

	public uint GetRenderModelCount()
	{
		return FnTable.GetRenderModelCount();
	}

	public uint GetComponentCount(string pchRenderModelName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		uint result = FnTable.GetComponentCount(intPtr);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetComponentName(string pchRenderModelName, uint unComponentIndex, StringBuilder pchComponentName, uint unComponentNameLen)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		uint result = FnTable.GetComponentName(intPtr, unComponentIndex, pchComponentName, unComponentNameLen);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public ulong GetComponentButtonMask(string pchRenderModelName, string pchComponentName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
		ulong result = FnTable.GetComponentButtonMask(intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public uint GetComponentRenderModelName(string pchRenderModelName, string pchComponentName, StringBuilder pchComponentRenderModelName, uint unComponentRenderModelNameLen)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
		uint result = FnTable.GetComponentRenderModelName(intPtr, intPtr2, pchComponentRenderModelName, unComponentRenderModelNameLen);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public bool GetComponentStateForDevicePath(string pchRenderModelName, string pchComponentName, ulong devicePath, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
		bool result = FnTable.GetComponentStateForDevicePath(intPtr, intPtr2, devicePath, ref pState, ref pComponentState);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public bool GetComponentState(string pchRenderModelName, string pchComponentName, ref VRControllerState_t pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
		if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
		{
			VRControllerState_t_Packed pControllerState2 = new VRControllerState_t_Packed(pControllerState);
			GetComponentStateUnion getComponentStateUnion = default;
			getComponentStateUnion.pGetComponentStatePacked = null;
			getComponentStateUnion.pGetComponentState = FnTable.GetComponentState;
			bool result = getComponentStateUnion.pGetComponentStatePacked(intPtr, intPtr2, ref pControllerState2, ref pState, ref pComponentState);
			pControllerState2.Unpack(ref pControllerState);
			return result;
		}
		bool result2 = FnTable.GetComponentState(intPtr, intPtr2, ref pControllerState, ref pState, ref pComponentState);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result2;
	}

	public bool RenderModelHasComponent(string pchRenderModelName, string pchComponentName)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		IntPtr intPtr2 = Utils.ToUtf8(pchComponentName);
		bool result = FnTable.RenderModelHasComponent(intPtr, intPtr2);
		Marshal.FreeHGlobal(intPtr);
		Marshal.FreeHGlobal(intPtr2);
		return result;
	}

	public uint GetRenderModelThumbnailURL(string pchRenderModelName, StringBuilder pchThumbnailURL, uint unThumbnailURLLen, ref EVRRenderModelError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		uint result = FnTable.GetRenderModelThumbnailURL(intPtr, pchThumbnailURL, unThumbnailURLLen, ref peError);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public uint GetRenderModelOriginalPath(string pchRenderModelName, StringBuilder pchOriginalPath, uint unOriginalPathLen, ref EVRRenderModelError peError)
	{
		IntPtr intPtr = Utils.ToUtf8(pchRenderModelName);
		uint result = FnTable.GetRenderModelOriginalPath(intPtr, pchOriginalPath, unOriginalPathLen, ref peError);
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public string GetRenderModelErrorNameFromEnum(EVRRenderModelError error)
	{
		return Marshal.PtrToStringAnsi(FnTable.GetRenderModelErrorNameFromEnum(error));
	}
}
