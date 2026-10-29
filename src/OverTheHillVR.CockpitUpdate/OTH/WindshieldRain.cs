using System.Collections.Generic;
using System.Numerics;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;

namespace OTH;

internal sealed class WindshieldRain
{
	internal static MelonPreferences_Entry<bool> Enabled;

	internal static readonly Vector3 Position = new Vector3(0f, 1.225f, 0.752f);

	internal static readonly Quaternion Rotation = Quaternion.Euler(-29.75f, 0f, 0f);

	private GameObject root;

	private Transform body;

	private Transform leftWiper;

	private Transform rightWiper;

	private Mesh dropMesh;

	private Texture2D dropTexture;

	private Material dropMaterial;

	private readonly List<Material> solids = new List<Material>();

	private Il2CppStructArray<Vector3> vertices;

	private Il2CppStructArray<Vector2> uv;

	private readonly WindshieldWeather weather = new WindshieldWeather();

	private readonly StaticWiperRemoval originals = new StaticWiperRemoval();

	private WindshieldSimulation simulation;

	private float rain;

	private float nextWeather;

	private float nextMesh;

	private bool? lastRunning;

	internal void Update()
	{
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
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
			originals.Restore();
			return;
		}
		if ((Object)(object)root == (Object)null)
		{
			Build(val);
		}
		root.SetActive(true);
		originals.Update(val);
		if (Time.unscaledTime >= nextWeather)
		{
			nextWeather = Time.unscaledTime + 0.25f;
			rain = weather.Read();
			Light sun = RenderSettings.sun;
			float num2;
			if (!((Object)(object)sun != (Object)null))
			{
				Color ambientLight = RenderSettings.ambientLight;
				num2 = Mathf.Clamp01(ambientLight.maxColorComponent);
			}
			else
			{
				num2 = Mathf.Clamp01((0f - ((Component)sun).transform.forward.y) * 1.5f) * Mathf.Clamp01(sun.intensity);
			}
			float num3 = num2;
			float num4 = Mathf.Lerp(0.2f, 0.9f, Mathf.Sqrt(num3));
			dropMaterial.SetColor("_BaseColor", new Color(num4, num4, num4, 1f));
		}
		simulation.Step(rain, Time.deltaTime);
		Transform val2 = leftWiper;
		Quaternion localRotation = (rightWiper.localRotation = Quaternion.Euler(0f, 0f, simulation.Angle));
		val2.localRotation = localRotation;
		if (lastRunning != simulation.Running)
		{
			lastRunning = simulation.Running;
			CockpitUpdate.Self.Log($"Windshield auto wipers {(simulation.Running ? "running" : "parked")}; native rain {rain:0.00}.");
		}
		if (Time.unscaledTime >= nextMesh)
		{
			nextMesh = Time.unscaledTime + 1f / 30f;
			RefreshDrops();
		}
	}

	private void Build(Transform current)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected Obj, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected Obj, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		body = current;
		root = new GameObject("OTHVR Rain Windshield");
		root.transform.SetParent(body, false);
		root.transform.localPosition = Position;
		root.transform.localRotation = Rotation;
		simulation = new WindshieldSimulation();
		dropTexture = CreateDropTexture();
		dropMaterial = DashboardCluster.CreateMat((Texture)(object)dropTexture, Color.white);
		((Object)dropMaterial).name = "OTHVR translucent water beads";
		dropMaterial.SetFloat("_Surface", 1f);
		dropMaterial.SetFloat("_Blend", 0f);
		dropMaterial.SetFloat("_SrcBlend", 5f);
		dropMaterial.SetFloat("_DstBlend", 10f);
		dropMaterial.SetFloat("_ZWrite", 0f);
		dropMaterial.SetOverrideTag("RenderType", "Transparent");
		dropMaterial.renderQueue = 3000;
		dropMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
		GameObject val = new GameObject("Windshield rain droplets");
		val.transform.SetParent(root.transform, false);
		int num = LayerMask.NameToLayer("TransparentFX");
		val.layer = ((num < 0) ? 1 : num);
		dropMesh = new Mesh();
		((Object)dropMesh).name = "OTHVR bounded rain beads";
		dropMesh.MarkDynamic();
		int num2 = 512;
		vertices = new Il2CppStructArray<Vector3>(num2 * 4);
		uv = new Il2CppStructArray<Vector2>(num2 * 4);
		int[] array = new int[num2 * 6];
		for (int i = 0; i < num2; i++)
		{
			int num3 = i * 4;
			int num4 = i * 6;
			uv[num3] = new Vector2(0f, 0f);
			uv[num3 + 1] = new Vector2(1f, 0f);
			uv[num3 + 2] = new Vector2(1f, 1f);
			uv[num3 + 3] = new Vector2(0f, 1f);
			array[num4] = num3;
			array[num4 + 1] = num3 + 2;
			array[num4 + 2] = num3 + 1;
			array[num4 + 3] = num3;
			array[num4 + 4] = num3 + 3;
			array[num4 + 5] = num3 + 2;
		}
		dropMesh.vertices = vertices;
		dropMesh.uv = uv;
		dropMesh.triangles = array;
		dropMesh.bounds = new Bounds(new Vector3(0f, 0.3475f, 0f), new Vector3(1.46f, 0.72f, 0.08f));
		val.AddComponent<MeshFilter>().sharedMesh = dropMesh;
		MeshRenderer val2 = val.AddComponent<MeshRenderer>();
		((Renderer)val2).sharedMaterial = dropMaterial;
		((Renderer)val2).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)val2).receiveShadows = false;
		Material rubber = Solid(new Color(0.018f, 0.023f, 0.025f));
		Material metal = Solid(new Color(0.1f, 0.12f, 0.13f));
		leftWiper = BuildWiper("Driver automatic wiper", WindshieldSimulation.LeftPivot, rubber, metal);
		rightWiper = BuildWiper("Passenger automatic wiper", WindshieldSimulation.RightPivot, rubber, metal);
		nextWeather = (nextMesh = 0f);
		lastRunning = null;
		CockpitUpdate.Self.Log("Windshield rain and two automatic wipers built on the shared cabin glass plane.");
	}

	private Transform BuildWiper(string name, Vector2 pivot, Material rubber, Material metal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.SetParent(root.transform, false);
		val.transform.localPosition = new Vector3(pivot.X, pivot.Y, -0.011f);
		Box("Wiper pivot", val.transform, new Vector3(0f, 0f, -0.004f), new Vector3(0.028f, 0.028f, 0.018f), metal);
		Box("Wiper arm", val.transform, new Vector3(0.198f, 0f, -0.003f), new Vector3(0.396f, 0.009f, 0.009f), metal);
		Box("Rubber blade", val.transform, new Vector3(0.45f, 0f, 0f), new Vector3(0.37f, 0.015f, 0.009f), rubber);
		Box("Blade frame", val.transform, new Vector3(0.45f, 0f, -0.006f), new Vector3(0.305f, 0.009f, 0.009f), metal);
		val.transform.localRotation = Quaternion.Euler(0f, 0f, 5f);
		return val.transform;
	}

	private Material Solid(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Material val = DashboardCluster.CreateMat(null, color);
		solids.Add(val);
		return val;
	}

	private static void Box(string name, Transform parent, Vector3 position, Vector3 size, Material material)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		((Object)val).name = name;
		val.transform.SetParent(parent, false);
		val.transform.localPosition = position;
		val.transform.localScale = size;
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		MeshRenderer component = val.GetComponent<MeshRenderer>();
		((Renderer)component).sharedMaterial = material;
		((Renderer)component).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)component).receiveShadows = false;
	}

	private void RefreshDrops()
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < simulation.Drops.Length; i++)
		{
			WindshieldSimulation.Drop drop = simulation.Drops[i];
			int num = i * 4;
			if (!drop.alive)
			{
				for (int j = 0; j < 4; j++)
				{
					vertices[num + j] = Vector3.zero;
				}
				continue;
			}
			float num2 = Mathf.Clamp01((drop.lifetime - drop.age) / 3f);
			float x = drop.position.X;
			float y = drop.position.Y;
			float radius = drop.radius;
			float num3 = radius * (1.12f + drop.slide * 12f);
			float num4 = Mathf.Max(0.005f, y - num3);
			float num5 = Mathf.Min(0.695f, y + num3);
			vertices[num] = new Vector3(x - radius, num4, -0.002f);
			vertices[num + 1] = new Vector3(x + radius, num4, -0.002f);
			vertices[num + 2] = new Vector3(x + radius, num5, -0.002f);
			vertices[num + 3] = new Vector3(x - radius, num5, -0.002f);
			if (num2 < 1f)
			{
				float num6 = Mathf.Sqrt(num2);
				for (int k = 0; k < 4; k++)
				{
					Vector3 val = vertices[num + k];
					vertices[num + k] = new Vector3(x + (val.x - x) * num6, y + (val.y - y) * num6, val.z);
				}
			}
		}
		dropMesh.vertices = vertices;
	}

	private static Texture2D CreateDropTexture()
	{
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected Obj, but got Unknown
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = new Color[4096];
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				float num = ((float)j + 0.5f) / 64f * 2f - 1f;
				float num2 = ((float)i + 0.5f) / 64f * 2f - 1f;
				float num3 = Mathf.Sqrt(num * num + num2 * num2);
				if (num3 >= 1f)
				{
					array[i * 64 + j] = Color.clear;
					continue;
				}
				float num4 = Mathf.Exp(0f - Mathf.Pow((num3 - 0.83f) / 0.13f, 2f));
				float num5 = Mathf.Exp(0f - ((num + 0.31f) * (num + 0.31f) / 0.035f + (num2 - 0.4f) * (num2 - 0.4f) / 0.07f));
				float num6 = Mathf.Exp(0f - ((num - 0.12f) * (num - 0.12f) / 0.2f + (num2 + 0.55f) * (num2 + 0.55f) / 0.045f));
				float num7 = Mathf.Clamp01(0.33f + 0.28f * num2 + 0.56f * num5 + 0.23f * num6);
				float num8 = Mathf.Clamp01((0.045f + 0.38f * num4 + 0.4f * num5 + 0.12f * num6) * Mathf.Clamp01((1f - num3) * 12f));
				array[i * 64 + j] = new Color(num7 * 0.91f, num7 * 0.97f, Mathf.Min(1f, num7 * 1.04f), num8);
			}
		}
		Texture2D val = new Texture2D(64, 64, (TextureFormat)4, false)
		{
			name = "OTHVR analytic water lens",
			wrapMode = (TextureWrapMode)1,
			filterMode = (FilterMode)1
		};
		val.SetPixels((Il2CppStructArray<Color>?)array);
		val.Apply(false, true);
		return val;
	}

	internal void Dispose()
	{
		originals.Dispose();
		if ((Object)(object)root != (Object)null)
		{
			Object.Destroy((Object)(object)root);
		}
		if ((Object)(object)dropMesh != (Object)null)
		{
			Object.Destroy((Object)(object)dropMesh);
		}
		if ((Object)(object)dropMaterial != (Object)null)
		{
			Object.Destroy((Object)(object)dropMaterial);
		}
		if ((Object)(object)dropTexture != (Object)null)
		{
			Object.Destroy((Object)(object)dropTexture);
		}
		foreach (Material solid in solids)
		{
			if ((Object)(object)solid != (Object)null)
			{
				Object.Destroy((Object)(object)solid);
			}
		}
		solids.Clear();
		root = null;
		body = (leftWiper = (rightWiper = null));
		dropMesh = null;
		dropTexture = null;
		dropMaterial = null;
		vertices = null;
		uv = null;
		simulation = null;
		rain = (nextWeather = (nextMesh = 0f));
		lastRunning = null;
	}

	static WindshieldRain()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	}
}
