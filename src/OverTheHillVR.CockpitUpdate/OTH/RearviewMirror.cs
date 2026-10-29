using System.Collections.Generic;
using Il2CppGPUInstancerPro;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OTH;

internal sealed class RearviewMirror
{
	internal static MelonPreferences_Entry<bool> Enabled;

	private Vector3 viewPosition;

	private GameObject root;

	private GameObject cameraObject;

	private Transform body;

	private Camera rear;

	private RenderTexture texture;

	private Mesh surface;

	private readonly List<Material> materials = new List<Material>();

	private float nextFrame;

	internal void Update()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		Transform val = CockpitUpdate.Read<Transform>("body");
		if ((Object)(object)body != (Object)null && ((Object)(object)val == (Object)null || ((Il2CppObjectBase)(object)val).Pointer != ((Il2CppObjectBase)(object)body).Pointer))
		{
			Dispose();
		}
		if (!Enabled.Value || !((Object)(object)val != (Object)null) || !CockpitUpdate.Read<bool>("inCockpit"))
		{
			if ((Object)(object)root != (Object)null)
			{
				root.SetActive(false);
			}
			if ((Object)(object)rear != (Object)null)
			{
				((Behaviour)rear).enabled = false;
			}
			if ((Object)(object)val == (Object)null && (Object)(object)texture != (Object)null)
			{
				Dispose();
			}
			return;
		}
		if ((Object)(object)root == (Object)null)
		{
			Build(val);
		}
		root.SetActive(true);
		((Component)rear).transform.localPosition = viewPosition;
		((Component)rear).transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		bool flag = Time.unscaledTime >= nextFrame;
		((Behaviour)rear).enabled = flag;
		if (flag)
		{
			nextFrame = Time.unscaledTime + 1f / 30f;
		}
	}

	private void Build(Transform current)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected Obj, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected Obj, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected Obj, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected Obj, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected Obj, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		Dispose();
		body = current;
		viewPosition = RearPosition(body);
		root = new GameObject("OTHVR Rearview Mirror");
		root.transform.SetParent(body, false);
		root.transform.localPosition = new Vector3(0.005f, 1.66f, 0.43f);
		root.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
		Material val = Material("Universal Render Pipeline/Lit", new Color(0.055f, 0.062f, 0.065f));
		val.SetFloat("_Smoothness", 0.28f);
		Box("Mirror housing", new Vector3(0f, 0f, 0.01f), new Vector3(0.28f, 0.094f, 0.018f), val);
		Box("Mirror stem", new Vector3(0f, 0.087f, 0.024f), new Vector3(0.014f, 0.166f, 0.014f), val);
		texture = new RenderTexture(640, 200, 24, (RenderTextureFormat)0);
		((Object)texture).name = "OTHVR Live Rearview";
		texture.antiAliasing = 1;
		((Texture)texture).filterMode = (FilterMode)1;
		((Texture)texture).wrapMode = (TextureWrapMode)1;
		texture.Create();
		Material val2 = Material("Universal Render Pipeline/Unlit", Color.white);
		val2.SetTexture("_BaseMap", (Texture)(object)texture);
		GameObject val3 = new GameObject("Live mirrored rear view");
		val3.transform.SetParent(root.transform, false);
		val3.transform.localPosition = new Vector3(0f, 0f, -0.001f);
		surface = new Mesh();
		((Object)surface).name = "OTHVR Rearview Face";
		surface.vertices = new Vector3[4]
		{
			new Vector3(-0.132f, -0.04125f, 0f),
			new Vector3(0.132f, -0.04125f, 0f),
			new Vector3(0.132f, 0.04125f, 0f),
			new Vector3(-0.132f, 0.04125f, 0f)
		};
		surface.uv = new Vector2[4]
		{
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		surface.triangles = new int[6] { 0, 2, 1, 0, 3, 2 };
		surface.RecalculateNormals();
		surface.RecalculateBounds();
		val3.AddComponent<MeshFilter>().sharedMesh = surface;
		MeshRenderer val4 = val3.AddComponent<MeshRenderer>();
		((Renderer)val4).sharedMaterial = val2;
		((Renderer)val4).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)val4).receiveShadows = false;
		cameraObject = new GameObject("OTHVR Rearview Camera");
		cameraObject.transform.SetParent(body, false);
		cameraObject.transform.localPosition = viewPosition;
		cameraObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		cameraObject.tag = "Untagged";
		rear = cameraObject.AddComponent<Camera>();
		((Behaviour)rear).enabled = false;
		Camera val5 = CockpitUpdate.Read<Camera>("src") ?? Camera.main;
		rear.orthographic = false;
		rear.usePhysicalProperties = false;
		((Behaviour)rear).enabled = false;
		rear.targetTexture = texture;
		rear.stereoTargetEye = (StereoTargetEyeMask)0;
		rear.rect = new Rect(0f, 0f, 1f, 1f);
		rear.ResetProjectionMatrix();
		rear.ResetWorldToCameraMatrix();
		rear.ResetCullingMatrix();
		rear.aspect = 3.2f;
		rear.fieldOfView = 24f;
		rear.nearClipPlane = 0.08f;
		rear.farClipPlane = 350f;
		rear.depth = (((Object)(object)val5 != (Object)null) ? val5.depth : 0f) - 40f;
		rear.cullingMask = (((Object)(object)val5 != (Object)null) ? val5.cullingMask : (-1)) & -33;
		rear.clearFlags = (CameraClearFlags)1;
		rear.allowMSAA = false;
		rear.allowDynamicResolution = false;
		rear.useOcclusionCulling = false;
		UniversalAdditionalCameraData universalAdditionalCameraData = CameraExtensions.GetUniversalAdditionalCameraData(rear);
		universalAdditionalCameraData.renderType = (CameraRenderType)0;
		universalAdditionalCameraData.renderPostProcessing = false;
		universalAdditionalCameraData.renderShadows = false;
		universalAdditionalCameraData.antialiasing = (AntialiasingMode)1;
		universalAdditionalCameraData.requiresColorTexture = false;
		universalAdditionalCameraData.requiresDepthTexture = true;
		universalAdditionalCameraData.cameraStack.Clear();
		if ((Object)(object)val5 != (Object)null)
		{
			universalAdditionalCameraData.volumeLayerMask = CameraExtensions.GetUniversalAdditionalCameraData(val5).volumeLayerMask;
		}
		cameraObject.AddComponent<GPUICamera>().SetOcclusionCullingEnabled(false);
		cameraObject.transform.SetParent(body, false);
		cameraObject.transform.localPosition = viewPosition;
		cameraObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		nextFrame = 0f;
		CockpitUpdate self = CockpitUpdate.Self;
		Vector3 val6 = viewPosition;
		self.Log("Live rearview mirror built: camera at car-local " + ((object)val6/*cast due to constrained. prefix*/).ToString() + ", level with dash, beyond rear bodywork; 640x200, up to 30 Hz.");
	}

	private static Vector3 RearPosition(Transform vehicle)
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		float num = -2.65f;
		foreach (Renderer componentsInChild in ((Component)vehicle).GetComponentsInChildren<Renderer>(true))
		{
			if ((Object)(object)((Il2CppObjectBase)(object)componentsInChild).TryCast<MeshRenderer>() == (Object)null && (Object)(object)((Il2CppObjectBase)(object)componentsInChild).TryCast<SkinnedMeshRenderer>() == (Object)null)
			{
				continue;
			}
			bool flag = false;
			Transform val = ((Component)componentsInChild).transform;
			while ((Object)(object)val != (Object)null && ((Il2CppObjectBase)(object)val).Pointer != ((Il2CppObjectBase)(object)vehicle).Pointer)
			{
				if (((Object)val).name.StartsWith("OTHVR"))
				{
					flag = true;
					break;
				}
				val = val.parent;
			}
			if (flag)
			{
				continue;
			}
			Bounds localBounds = componentsInChild.localBounds;
			Vector3 extents = localBounds.extents;
			if (extents.magnitude > 15f)
			{
				continue;
			}
			for (int i = 0; i < 8; i++)
			{
				Vector3 val2 = localBounds.center + Vector3.Scale(localBounds.extents, new Vector3((float)(((i & 1) != 0) ? 1 : (-1)), (float)(((i & 2) != 0) ? 1 : (-1)), (float)(((i & 4) != 0) ? 1 : (-1))));
				Vector3 val3 = vehicle.InverseTransformPoint(((Component)componentsInChild).transform.TransformPoint(val2));
				if (val3.z >= -10f)
				{
					num = Mathf.Min(num, val3.z - 0.25f);
				}
			}
		}
		return new Vector3(0f, 1.1644f, num);
	}

	private Material Material(string shader, Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected Obj, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Material val = new Material(Shader.Find(shader));
		val.SetColor("_BaseColor", color);
		val.SetFloat("_Cull", 0f);
		materials.Add(val);
		return val;
	}

	private void Box(string name, Vector3 position, Vector3 scale, Material material)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		((Object)val).name = name;
		val.transform.SetParent(root.transform, false);
		val.transform.localPosition = position;
		val.transform.localScale = scale;
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		MeshRenderer component = val.GetComponent<MeshRenderer>();
		((Renderer)component).sharedMaterial = material;
		((Renderer)component).shadowCastingMode = (ShadowCastingMode)0;
	}

	internal void Dispose()
	{
		if ((Object)(object)rear != (Object)null)
		{
			((Behaviour)rear).enabled = false;
			rear.targetTexture = null;
		}
		if ((Object)(object)cameraObject != (Object)null)
		{
			Object.Destroy((Object)(object)cameraObject);
		}
		if ((Object)(object)root != (Object)null)
		{
			Object.Destroy((Object)(object)root);
		}
		if ((Object)(object)surface != (Object)null)
		{
			Object.Destroy((Object)(object)surface);
		}
		if ((Object)(object)texture != (Object)null)
		{
			texture.Release();
			Object.Destroy((Object)(object)texture);
		}
		foreach (Material material in materials)
		{
			if ((Object)(object)material != (Object)null)
			{
				Object.Destroy((Object)(object)material);
			}
		}
		materials.Clear();
		root = null;
		cameraObject = null;
		body = null;
		rear = null;
		texture = null;
		surface = null;
	}
}
