using System;
using System.Collections.Generic;
using System.Numerics;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using MelonLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OTH;

internal sealed class ConsoleGps
{
	internal static MelonPreferences_Entry<bool> Enabled;

	internal static MelonPreferences_Entry<float> Range;

	internal static readonly Vector3 Position = new Vector3(0.035f, 1.025f, 0.065f);

	internal const float Width = 0.27f;

	internal const float Height = 0.165f;

	private GameObject root;

	private GameObject screen;

	private Transform body;

	private Transform marker;

	private Mesh mapMesh;

	private Material mapMaterial;

	private Texture2D font;

	private Texture mapTexture;

	private MapView mapView;

	private AsyncOperationHandle<GameObject> mapPrefab;

	private bool holdsMapPrefab;

	private DashboardCluster.Glyph status;

	private DashboardCluster.Glyph scale;

	private readonly List<Material> materials = new List<Material>();

	private readonly List<Mesh> meshes = new List<Mesh>();

	private float nextSearch;

	private float nextUpdate;

	private bool ready;

	internal void Update()
	{
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Invalid comparison between Unknown and I4
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		Transform val = CockpitUpdate.Read<Transform>("body");
		bool num = Enabled.Value && (Object)(object)val != (Object)null && CockpitUpdate.Read<bool>("inCockpit");
		if (root != null && ((Object)(object)root == (Object)null || (Object)(object)body == (Object)null || (Object)(object)val == (Object)null || ((Il2CppObjectBase)(object)val).Pointer != ((Il2CppObjectBase)(object)body).Pointer))
		{
			Dispose();
		}
		if (!num)
		{
			if ((Object)(object)root != (Object)null)
			{
				root.SetActive(false);
			}
			return;
		}
		if ((Object)(object)root == (Object)null)
		{
			Build(val);
		}
		root.SetActive(true);
		if (Time.unscaledTime < nextUpdate)
		{
			return;
		}
		nextUpdate = Time.unscaledTime + 0.05f;
		MapManager instance = MapManager.Instance;
		if ((Object)(object)instance == (Object)null || (Object)(object)instance.MapTexture == (Object)null || instance.MapWidth <= 0f)
		{
			SetWaiting();
			return;
		}
		if ((Object)(object)mapView == (Object)null && Time.unscaledTime >= nextSearch)
		{
			nextSearch = Time.unscaledTime + 1f;
			foreach (Object item in Resources.FindObjectsOfTypeAll(Il2CppType.Of<MapView>()))
			{
				MapView val2 = ((Il2CppObjectBase)(object)item).TryCast<MapView>();
				if ((Object)(object)val2 != (Object)null && (Object)(object)val2.mapRawImage != (Object)null)
				{
					mapView = val2;
					break;
				}
			}
			if ((Object)(object)mapView == (Object)null && !holdsMapPrefab)
			{
				mapPrefab = Addressables.LoadAssetAsync<GameObject>((Object)(object)String.op_Implicit(UI.MapScreen));
				holdsMapPrefab = true;
			}
		}
		if ((Object)(object)mapView == (Object)null && holdsMapPrefab && mapPrefab.IsValid() && mapPrefab.IsDone && (int)mapPrefab.Status == 1 && (Object)(object)mapPrefab.Result != (Object)null)
		{
			mapView = mapPrefab.Result.GetComponentInChildren<MapView>(true);
		}
		if ((Object)(object)mapView == (Object)null)
		{
			SetWaiting();
			return;
		}
		Texture2D val3 = instance.MapTexture;
		Vector2 borderSizeInPixels = mapView.borderSizeInPixels;
		Vector2 border = new Vector2(borderSizeInPixels.x / (float)((Texture)val3).width, borderSizeInPixels.y / (float)((Texture)val3).height);
		if (border.X < 0f || border.Y < 0f || border.X >= 0.5f || border.Y >= 0.5f)
		{
			SetWaiting();
			return;
		}
		if ((Object)(object)mapTexture == (Object)null || ((Il2CppObjectBase)(object)mapTexture).Pointer != ((Il2CppObjectBase)(object)val3).Pointer)
		{
			mapTexture = (Texture)(object)val3;
			mapMaterial.SetTexture("_BaseMap", (Texture)(object)val3);
			CockpitUpdate.Self.Log($"Console GPS map: {((Object)val3).name}, {((Texture)val3).width}x{((Texture)val3).height}; terrain width {instance.MapWidth:0} m, origin {instance.TerrainOffset}, border {borderSizeInPixels}.");
		}
		Vector3 position = body.position;
		Vector3 terrainOffset = instance.TerrainOffset;
		Vector2 vector = GpsProjection.WorldUv(new Vector2(position.x, position.z), new Vector2(terrainOffset.x, terrainOffset.z), instance.MapWidth, border);
		float num2 = Range.Value;
		if (!float.IsFinite(num2))
		{
			num2 = 900f;
		}
		num2 = Mathf.Clamp(num2, 150f, 4000f);
		(Vector2, Vector2) tuple = GpsProjection.Window(vector, instance.MapWidth, num2, 1.6363636f, border);
		float x = tuple.Item1.X;
		float y = tuple.Item1.Y;
		float x2 = tuple.Item2.X;
		float y2 = tuple.Item2.Y;
		mapMesh.uv = new Vector2[4]
		{
			new Vector2(x, y),
			new Vector2(x + x2, y),
			new Vector2(x + x2, y + y2),
			new Vector2(x, y + y2)
		};
		Vector2 vector2 = GpsProjection.Screen(vector, tuple.Item1, tuple.Item2);
		marker.localPosition = new Vector3(Mathf.Clamp(vector2.X, -0.47f, 0.47f) * 0.27f, Mathf.Clamp(vector2.Y, -0.46f, 0.46f) * 0.165f, -0.004f);
		Vector3 forward = body.forward;
		float num3 = GpsProjection.Heading(forward.x, forward.z);
		marker.localRotation = Quaternion.Euler(0f, 0f, 0f - num3);
		screen.SetActive(true);
		((Component)marker).gameObject.SetActive(true);
		status.Set("GPS    N UP");
		float x3 = tuple.Item2.X * instance.MapWidth / (1f - 2f * border.X);
		scale.Set($"{MathF.Round(x3):0} M ACROSS");
		if (!ready)
		{
			ready = true;
			CockpitUpdate.Self.Log("Console GPS following the owned vehicle; north-up map and live heading arrow.");
		}
	}

	private void SetWaiting()
	{
		screen.SetActive(false);
		((Component)marker).gameObject.SetActive(false);
		status.Set("GPS");
		scale.Set("MAP LOADING");
		ready = false;
	}

	private Material Mat(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Material val = DashboardCluster.CreateMat(null, color);
		materials.Add(val);
		return val;
	}

	private void Quad(string name, Vector3 position, float width, float height, Material material)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = DashboardCluster.Quad(name, root.transform, position, width, height, material);
		meshes.Add(val.GetComponent<MeshFilter>().sharedMesh);
	}

	private void Build(Transform current)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected Obj, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		body = current;
		root = new GameObject("OTHVR Center Console GPS");
		root.transform.SetParent(body, false);
		root.transform.localPosition = Position;
		root.transform.localRotation = Quaternion.Euler(28f, 12f, 0f);
		font = DashboardCluster.Load("Font");
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		((Object)val).name = "GPS charcoal housing";
		val.transform.SetParent(root.transform, false);
		val.transform.localPosition = new Vector3(0f, 0f, 0.012f);
		val.transform.localScale = new Vector3(0.3f, 0.227f, 0.025f);
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		MeshRenderer component = val.GetComponent<MeshRenderer>();
		((Renderer)component).sharedMaterial = Mat(new Color(0.018f, 0.024f, 0.025f));
		((Renderer)component).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)component).receiveShadows = false;
		Quad("GPS screen surround", new Vector3(0f, 0f, -0.002f), 0.282f, 0.178f, Mat(new Color(0.007f, 0.012f, 0.014f)));
		mapMaterial = Mat(new Color(0.69f, 0.74f, 0.7f));
		screen = DashboardCluster.Quad("Native terrain map", root.transform, new Vector3(0f, 0f, -0.003f), 0.27f, 0.165f, mapMaterial);
		mapMesh = screen.GetComponent<MeshFilter>().sharedMesh;
		mapMesh.MarkDynamic();
		meshes.Add(mapMesh);
		GameObject val2 = new GameObject("Your vehicle heading");
		val2.transform.SetParent(root.transform, false);
		marker = val2.transform;
		Arrow("Heading outline", 0.0135f, 0.019f, Mat(new Color(0.012f, 0.025f, 0.03f)), 0f);
		Arrow("Vehicle arrow", 0.009f, 0.014f, Mat(new Color(0.43f, 0.91f, 0.85f)), -0.001f);
		Color color = new Color(0.64f, 0.75f, 0.68f);
		status = new DashboardCluster.Glyph("GPS heading label", root.transform, new Vector3(0f, 0.099f, -0.004f), 0.012f, (Texture)(object)font, color);
		scale = new DashboardCluster.Glyph("GPS map scale", root.transform, new Vector3(0f, -0.1f, -0.004f), 0.011f, (Texture)(object)font, color);
		DashboardCluster.Glyph[] array = new DashboardCluster.Glyph[2] { status, scale };
		foreach (DashboardCluster.Glyph glyph in array)
		{
			materials.Add(glyph.mat);
			meshes.Add(glyph.go.GetComponent<MeshFilter>().sharedMesh);
		}
		SetWaiting();
		CockpitUpdate.Self.Log("Center-console GPS built below the level gauge at " + ((object)Position/*cast due to constrained. prefix*/).ToString() + ".");
	}

	private void Arrow(string name, float halfWidth, float length, Material material, float z)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected Obj, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.SetParent(marker, false);
		val.transform.localPosition = new Vector3(0f, 0f, z);
		Mesh val2 = new Mesh();
		val2.vertices = new Vector3[4]
		{
			new Vector3(0f, length, 0f),
			new Vector3(halfWidth, (0f - length) * 0.65f, 0f),
			new Vector3(0f, (0f - length) * 0.28f, 0f),
			new Vector3(0f - halfWidth, (0f - length) * 0.65f, 0f)
		};
		val2.triangles = new int[6] { 0, 2, 1, 0, 3, 2 };
		val2.RecalculateNormals();
		val2.RecalculateBounds();
		meshes.Add(val2);
		val.AddComponent<MeshFilter>().sharedMesh = val2;
		MeshRenderer val3 = val.AddComponent<MeshRenderer>();
		((Renderer)val3).sharedMaterial = material;
		((Renderer)val3).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)val3).receiveShadows = false;
	}

	internal void Dispose()
	{
		if ((Object)(object)root != (Object)null)
		{
			Object.Destroy((Object)(object)root);
		}
		foreach (Mesh mesh in meshes)
		{
			if ((Object)(object)mesh != (Object)null)
			{
				Object.Destroy((Object)(object)mesh);
			}
		}
		foreach (Material material in materials)
		{
			if ((Object)(object)material != (Object)null)
			{
				Object.Destroy((Object)(object)material);
			}
		}
		if ((Object)(object)font != (Object)null)
		{
			Object.Destroy((Object)(object)font);
		}
		if (holdsMapPrefab && mapPrefab.IsValid())
		{
			Addressables.Release<GameObject>(mapPrefab);
		}
		meshes.Clear();
		materials.Clear();
		root = (screen = null);
		body = (marker = null);
		mapMesh = null;
		mapMaterial = null;
		mapTexture = null;
		mapView = null;
		holdsMapPrefab = false;
		mapPrefab = null;
		font = null;
		status = (scale = null);
		ready = false;
		nextSearch = (nextUpdate = 0f);
	}

	static ConsoleGps()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
	}
}
