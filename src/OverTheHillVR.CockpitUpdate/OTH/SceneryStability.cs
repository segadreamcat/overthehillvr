using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppGPUInstancerPro;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

namespace OTH;

internal sealed class SceneryStability
{
	private sealed class State
	{
		internal Camera camera;

		internal bool unity;

		internal bool gpuEnabled;

		internal bool autoInitialize;

		internal GPUICamera gpu;

		internal GPUICameraData data;
	}

	internal static MelonPreferences_Entry<bool> Enabled;

	private readonly Dictionary<IntPtr, State> saved = new Dictionary<IntPtr, State>();

	internal void Initialize(HarmonyLib.Harmony harmony)
	{
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("FrameUpdate", bindingAttr), null, new HarmonyMethod(typeof(SceneryStability).GetMethod("AfterFrame", BindingFlags.Static | BindingFlags.NonPublic)));
		harmony.Patch(CockpitUpdate.DriverType.GetMethod("StopVr", bindingAttr), null, new HarmonyMethod(typeof(SceneryStability).GetMethod("AfterStop", BindingFlags.Static | BindingFlags.NonPublic)));
	}

	private static void AfterFrame()
	{
		try
		{
			CockpitUpdate.Self.scenery.Update();
		}
		catch (Exception ex)
		{
			CockpitUpdate.Self.Warn("Scenery culling: " + ex.Message);
		}
	}

	private static void AfterStop()
	{
		CockpitUpdate.Self?.scenery.Restore();
	}

	internal void Update()
	{
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if (!Enabled.Value || !CockpitUpdate.Read<bool>("vrOn") || !CockpitUpdate.Read<bool>("stereoView"))
		{
			Restore();
			return;
		}
		Camera val = CockpitUpdate.Read<Camera>("src");
		Camera[] array = CockpitUpdate.Read<Camera[]>("eyes");
		List<IntPtr> list = new List<IntPtr>();
		Camera[] array2;
		foreach (KeyValuePair<IntPtr, State> item in saved)
		{
			Camera camera = item.Value.camera;
			bool flag = (Object)(object)camera != (Object)null && (Object)(object)val != (Object)null && ((Il2CppObjectBase)(object)camera).Pointer == ((Il2CppObjectBase)(object)val).Pointer;
			if (!flag && (Object)(object)camera != (Object)null && array != null)
			{
				array2 = array;
				foreach (Camera val2 in array2)
				{
					if ((Object)(object)val2 != (Object)null && ((Il2CppObjectBase)(object)val2).Pointer == ((Il2CppObjectBase)(object)camera).Pointer)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add(item.Key);
			}
		}
		foreach (IntPtr item2 in list)
		{
			RestoreOne(item2);
		}
		Apply(val);
		if (array == null)
		{
			return;
		}
		array2 = array;
		foreach (Camera val3 in array2)
		{
			if (!((Object)(object)val3 == (Object)null))
			{
				val3.ResetWorldToCameraMatrix();
				val3.ResetCullingMatrix();
				val3.rect = new Rect(0f, 0f, 1f, 1f);
				Apply(val3);
			}
		}
	}

	internal void Apply(Camera camera)
	{
		if ((Object)(object)camera == (Object)null)
		{
			return;
		}
		if (!saved.TryGetValue(((Il2CppObjectBase)(object)camera).Pointer, out var value))
		{
			value = new State
			{
				camera = camera,
				unity = camera.useOcclusionCulling
			};
			saved.Add(((Il2CppObjectBase)(object)camera).Pointer, value);
			CockpitUpdate.Self.Log("Stable scenery: depth occlusion disabled on '" + ((Object)camera).name + "'; frustum culling retained.");
		}
		GPUICamera component = ((Component)camera).GetComponent<GPUICamera>();
		if ((Object)(object)component != (Object)null && ((Object)(object)value.gpu == (Object)null || ((Il2CppObjectBase)(object)value.gpu).Pointer != ((Il2CppObjectBase)(object)component).Pointer))
		{
			value.gpu = component;
			value.gpuEnabled = component._enableOcclusionCulling;
		}
		camera.useOcclusionCulling = false;
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		if (component._enableOcclusionCulling)
		{
			component.SetOcclusionCullingEnabled(false);
		}
		GPUICameraData cameraData = component.GetCameraData();
		if (cameraData != null)
		{
			if (value.data == null || ((Il2CppObjectBase)(object)value.data).Pointer != ((Il2CppObjectBase)(object)cameraData).Pointer)
			{
				value.data = cameraData;
				value.autoInitialize = cameraData.autoInitializeOcclusionCulling;
			}
			cameraData.autoInitializeOcclusionCulling = false;
			if (cameraData.OcclusionCullingData != null)
			{
				cameraData.DisableOcclusionCulling();
			}
		}
	}

	private void RestoreOne(IntPtr pointer)
	{
		State state = saved[pointer];
		saved.Remove(pointer);
		if ((Object)(object)state.camera != (Object)null)
		{
			state.camera.useOcclusionCulling = state.unity;
		}
		if (state.data != null)
		{
			state.data.autoInitializeOcclusionCulling = state.autoInitialize;
		}
		if ((Object)(object)state.gpu != (Object)null)
		{
			state.gpu.SetOcclusionCullingEnabled(state.gpuEnabled);
		}
	}

	internal void Restore()
	{
		foreach (IntPtr item in new List<IntPtr>(saved.Keys))
		{
			RestoreOne(item);
		}
	}
}
