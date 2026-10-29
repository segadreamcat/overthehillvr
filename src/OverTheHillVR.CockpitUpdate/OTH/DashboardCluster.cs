using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using Il2CppVehiclePhysics;
using UnityEngine;
using UnityEngine.Rendering;

namespace OTH;

internal sealed class DashboardCluster
{
	internal sealed class Glyph
	{
		public readonly GameObject go;

		public readonly Material mat;

		private readonly Mesh mesh;

		private readonly float height;

		private string old;

		public Glyph(string name, Transform parent, Vector3 position, float size, Texture font, Color color)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Expected Obj, but got Unknown
			height = size;
			mat = CreateMat(font, color);
			go = new GameObject(name);
			go.layer = 0;
			go.transform.SetParent(parent, false);
			go.transform.localPosition = position;
			mat.SetFloat("_AlphaClip", 1f);
			mat.SetFloat("_Cutoff", 0.15f);
			mat.EnableKeyword("_ALPHATEST_ON");
			mat.SetOverrideTag("RenderType", "TransparentCutout");
			mat.renderQueue = 2450;
			mesh = new Mesh();
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer val = go.AddComponent<MeshRenderer>();
			((Renderer)val).sharedMaterial = mat;
			((Renderer)val).shadowCastingMode = (ShadowCastingMode)0;
			((Renderer)val).receiveShadows = false;
		}

		public void Set(string text)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			if (!(text == old))
			{
				old = text;
				List<Vector3> list = new List<Vector3>();
				List<Vector2> list2 = new List<Vector2>();
				List<int> list3 = new List<int>();
				float num = height * 40f / 72f;
				for (int i = 0; i < text.Length; i++)
				{
					int num2 = Math.Clamp(text[i] - 32, 0, 95);
					int count = list.Count;
					float num3 = ((float)i - (float)text.Length * 0.5f) * num;
					list.Add(new Vector3(num3, (0f - height) * 0.5f, 0f));
					list.Add(new Vector3(num3 + num, (0f - height) * 0.5f, 0f));
					list.Add(new Vector3(num3 + num, height * 0.5f, 0f));
					list.Add(new Vector3(num3, height * 0.5f, 0f));
					float num4 = (float)(num2 % 16) / 16f;
					float num5 = (float)(num2 % 16 + 1) / 16f;
					float num6 = 1f - (float)(num2 / 16 + 1) / 6f;
					float num7 = 1f - (float)(num2 / 16) / 6f;
					list2.Add(new Vector2(num4, num6));
					list2.Add(new Vector2(num5, num6));
					list2.Add(new Vector2(num5, num7));
					list2.Add(new Vector2(num4, num7));
					list3.AddRange(new int[6]
					{
						count,
						count + 2,
						count + 1,
						count,
						count + 3,
						count + 2
					});
				}
				mesh.Clear();
				mesh.vertices = list.ToArray();
				mesh.uv = list2.ToArray();
				mesh.triangles = list3.ToArray();
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
			}
		}
	}

	private GameObject root;

	private Transform body;

	private Transform needle;

	private Glyph gear;

	private Glyph speed;

	private Glyph secondary;

	private Glyph state;

	private Glyph selection;

	private ContextUIDrivingModeSelector selector;

	private Material dial;

	private Texture2D atlas;

	private Texture2D mphFace;

	private Texture2D kmhFace;

	private bool lastMph;

	private float brightness = 1f;

	private readonly List<(Material material, Color tint)> inks = new List<(Material, Color)>();

	private readonly Dictionary<IntPtr, (MeshRenderer renderer, bool enabled)> originalGauges = new Dictionary<IntPtr, (MeshRenderer, bool)>();

	private string lastGear;

	private static readonly Color Warm = new Color(0.94f, 0.84f, 0.64f);

	private static readonly Color Green = new Color(0.61f, 0.81f, 0.66f);

	public void Update()
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		Transform val = CockpitUpdate.Read<Transform>("body");
		bool num = CockpitUpdate.GearDisplayEnabled.Value && (Object)(object)val != (Object)null && CockpitUpdate.Read<bool>("inCockpit");
		if (root != null && ((Object)(object)root == (Object)null || (Object)(object)body == (Object)null || (Object)(object)val == (Object)null || ((Il2CppObjectBase)(object)body).Pointer != ((Il2CppObjectBase)(object)val).Pointer))
		{
			Dispose();
		}
		if (!num)
		{
			RestoreOriginalGauges();
			if ((Object)(object)root != (Object)null)
			{
				root.SetActive(false);
			}
			return;
		}
		float num2 = Mathf.Clamp01(CockpitUpdate.DayBrightness.Value);
		if ((Object)(object)root == (Object)null)
		{
			brightness = num2;
			Build(val);
		}
		root.SetActive(true);
		HideOriginalGauges();
		brightness = Mathf.MoveTowards(brightness, num2, 0.08f);
		foreach (var ink in inks)
		{
			ink.material.SetColor("_BaseColor", Dim(ink.tint));
		}
		NetworkedVehicleStatus val2 = CockpitUpdate.Read<NetworkedVehicleStatus>("carStatus");
		if ((Object)(object)val2 == (Object)null)
		{
			return;
		}
		if ((Object)(object)selector == (Object)null)
		{
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<ContextUIDrivingModeSelector>()))
			{
				selector = ((Il2CppObjectBase)(object)item).TryCast<ContextUIDrivingModeSelector>();
				if ((Object)(object)selector != (Object)null)
				{
					break;
				}
			}
		}
		string text = (((Object)(object)selector != (Object)null && (Object)(object)selector.TextGear != (Object)null) ? Regex.Replace(((TMP_Text)selector.TextGear).text ?? "", "<.*?>", "").Trim().ToUpperInvariant() : "");
		if ((Object)(object)selector == (Object)null || selector.lastInsertedGear != val2.GearboxGear || string.IsNullOrEmpty(text))
		{
			text = DashboardReadings.Gear(val2.GearboxGear);
		}
		if (val2.GearboxGear == 0 && (Object)(object)val2.vehicle != (Object)null && val2.vehicle.data.Get(Channel.Input, InputData.AutomaticGear) == 1)
		{
			text = "P";
		}
		gear.Set(text);
		int num3 = Array.IndexOf(new string[5] { "P", "R", "N", "1", "2" }, text);
		selection.go.SetActive(num3 >= 0);
		if (num3 >= 0)
		{
			selection.go.transform.localPosition = new Vector3(0.025f + (float)num3 * 0.03f, -0.022f, -0.003f);
			selection.Set(text);
		}
		string text2;
		if (val2.EngineRunning)
		{
			if (text == "P")
			{
				text2 = "PARK";
			}
			else if (text == "N")
			{
				text2 = "NEUTRAL";
			}
			else
			{
				text2 = (val2.HandbrakeSet ? "PARKING BRAKE" : "READY TO DRIVE");
			}
		}
		else
		{
			text2 = "ENGINE OFF";
		}
		state.Set(text2 + "  " + (val2.GearboxIsLowTransferCase ? "4L" : "4H"));
		state.mat.SetColor("_BaseColor", Dim((text2 == "READY TO DRIVE") ? Green : Warm));
		Rigidbody val3 = CockpitUpdate.Read<Rigidbody>("bodyRb");
		float num4;
		if (!((Object)(object)val3 != (Object)null))
		{
			num4 = 0f;
		}
		else
		{
			Vector3 velocity = val3.velocity;
			num4 = velocity.magnitude;
		}
		float metersPerSecond = num4;
		bool value = CockpitUpdate.SpeedometerMph.Value;
		float num5 = (value ? DashboardReadings.Mph(metersPerSecond) : DashboardReadings.Kmh(metersPerSecond));
		speed.Set(((num5 < 0.5f) ? "0" : MathF.Round(num5).ToString("0")) + (value ? " MPH" : " KM/H"));
		secondary.Set((value ? DashboardReadings.Kmh(metersPerSecond) : DashboardReadings.Mph(metersPerSecond)).ToString("0") + (value ? " KM/H" : " MPH"));
		needle.localRotation = Quaternion.Euler(0f, 0f, DashboardReadings.Needle(num5, value));
		if (value != lastMph)
		{
			lastMph = value;
			dial.SetTexture("_BaseMap", (Texture)(object)(value ? mphFace : kmhFace));
		}
		string text3 = $"{text} / code {val2.GearboxGear} / {text2}";
		if (lastGear != text3)
		{
			lastGear = text3;
			CockpitUpdate.Self.Log("Instruments: " + text3);
		}
	}

	private void HideOriginalGauges()
	{
		Transform[] array = CockpitUpdate.Read<Transform[]>("needles");
		if (array == null)
		{
			return;
		}
		Transform[] array2 = array;
		foreach (Transform val in array2)
		{
			if ((Object)(object)val == (Object)null || (Object)(object)val.parent == (Object)null || !((Object)val.parent).name.StartsWith("OTHVR Gauge "))
			{
				continue;
			}
			foreach (MeshRenderer componentsInChild in ((Component)val.parent).GetComponentsInChildren<MeshRenderer>(true))
			{
				if (!originalGauges.ContainsKey(((Il2CppObjectBase)(object)componentsInChild).Pointer))
				{
					originalGauges.Add(((Il2CppObjectBase)(object)componentsInChild).Pointer, (componentsInChild, ((Renderer)componentsInChild).enabled));
				}
				((Renderer)componentsInChild).enabled = false;
			}
		}
	}

	internal void RestoreOriginalGauges()
	{
		foreach (var value in originalGauges.Values)
		{
			if ((Object)(object)value.renderer != (Object)null)
			{
				((Renderer)value.renderer).enabled = value.enabled;
			}
		}
		originalGauges.Clear();
	}

	internal void Dispose()
	{
		RestoreOriginalGauges();
		if ((Object)(object)root != (Object)null)
		{
			foreach (MeshFilter componentsInChild in root.GetComponentsInChildren<MeshFilter>(true))
			{
				if ((Object)(object)componentsInChild.sharedMesh != (Object)null)
				{
					Object.Destroy((Object)(object)componentsInChild.sharedMesh);
				}
			}
			Object.Destroy((Object)(object)root);
		}
		foreach (var ink in inks)
		{
			if ((Object)(object)ink.material != (Object)null)
			{
				Object.Destroy((Object)(object)ink.material);
			}
		}
		inks.Clear();
		root = null;
		body = null;
		needle = null;
		selector = null;
		dial = null;
		gear = (speed = (secondary = (state = (selection = null))));
		lastGear = null;
	}

	private void Build(Transform current)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Expected Obj, but got Unknown
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		body = current;
		root = new GameObject("OTHVR Driver Instruments");
		root.transform.SetParent(body, false);
		root.transform.localPosition = new Vector3(-0.3289f, 1.1644f, 0.265f);
		root.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
		if ((Object)(object)atlas == (Object)null)
		{
			atlas = Load("Font");
		}
		if ((Object)(object)mphFace == (Object)null)
		{
			mphFace = Load("MPH");
		}
		if ((Object)(object)kmhFace == (Object)null)
		{
			kmhFace = Load("KMH");
		}
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		val.transform.SetParent(root.transform, false);
		val.transform.localPosition = new Vector3(0f, 0f, 0.012f);
		val.transform.localScale = new Vector3(0.452f, 0.15f, 0.018f);
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		((Renderer)val.GetComponent<MeshRenderer>()).sharedMaterial = Mat(null, new Color(0.025f, 0.033f, 0.036f));
		lastMph = CockpitUpdate.SpeedometerMph.Value;
		dial = Mat((Texture)(object)(lastMph ? mphFace : kmhFace), Color.white);
		Quad("Printed dial", root.transform, Vector3.zero, 0.44f, 0.14f, dial);
		gear = new Glyph("Actual gear", root.transform, new Vector3(0.145f, 0.02f, -0.002f), 0.058f, (Texture)(object)atlas, Warm);
		speed = new Glyph("Road speed", root.transform, new Vector3(-0.104f, 0.025f, -0.003f), 0.03f, (Texture)(object)atlas, Warm);
		secondary = new Glyph("Other speed unit", root.transform, new Vector3(-0.104f, -0.059f, -0.003f), 0.013f, (Texture)(object)atlas, new Color(0.6f, 0.68f, 0.69f));
		state = new Glyph("Drive readiness", root.transform, new Vector3(0.12f, -0.052f, -0.003f), 0.012f, (Texture)(object)atlas, Warm);
		selection = new Glyph("Selected position", root.transform, Vector3.zero, 0.022f, (Texture)(object)atlas, Warm);
		Glyph[] array = new Glyph[5] { gear, speed, secondary, state, selection };
		foreach (Glyph glyph in array)
		{
			inks.Add((glyph.mat, glyph.mat.GetColor("_BaseColor")));
		}
		GameObject val2 = new GameObject("Speed needle pivot");
		val2.transform.SetParent(root.transform, false);
		val2.transform.localPosition = new Vector3(-0.104f, 0f, -0.004f);
		needle = val2.transform;
		Quad("Live speed needle", needle, new Vector3(0f, 0.02f, 0f), 0.0025f, 0.046f, Mat(null, new Color(0.98f, 0.43f, 0.16f)));
		Quad("Needle cap", root.transform, new Vector3(-0.104f, 0f, -0.005f), 0.007f, 0.007f, Mat(null, Warm));
		CockpitUpdate.Self.Log("Textured mesh instruments built at car-local " + ((object)root.transform.localPosition/*cast due to constrained. prefix*/).ToString() + "; independent of UI canvases.");
	}

	internal static Texture2D Load(string name)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected Obj, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		using Stream stream = typeof(CockpitUpdate).Assembly.GetManifestResourceStream("OTH.Instruments." + name);
		using MemoryStream memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false);
		ImageConversion.LoadImage(val, (Il2CppStructArray<byte>?)memoryStream.ToArray(), false);
		((Texture)val).filterMode = (FilterMode)1;
		((Texture)val).wrapMode = (TextureWrapMode)1;
		return val;
	}

	private Color Dim(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return new Color(color.r * brightness, color.g * brightness, color.b * brightness, color.a);
	}

	private Material Mat(Texture tex, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Material val = CreateMat(tex, color);
		inks.Add((val, color));
		return val;
	}

	internal static Material CreateMat(Texture tex, Color color)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected Obj, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Material val = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
		val.SetFloat("_Cull", 0f);
		val.SetColor("_BaseColor", color);
		if ((Object)(object)tex != (Object)null)
		{
			val.SetTexture("_BaseMap", tex);
		}
		return val;
	}

	internal static GameObject Quad(string name, Transform parent, Vector3 pos, float w, float h, Material mat)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.layer = 0;
		val.transform.SetParent(parent, false);
		val.transform.localPosition = pos;
		Mesh val2 = new Mesh();
		val2.vertices = new Vector3[4]
		{
			new Vector3((0f - w) / 2f, (0f - h) / 2f, 0f),
			new Vector3(w / 2f, (0f - h) / 2f, 0f),
			new Vector3(w / 2f, h / 2f, 0f),
			new Vector3((0f - w) / 2f, h / 2f, 0f)
		};
		val2.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
		val2.triangles = new int[6] { 0, 2, 1, 0, 3, 2 };
		val2.RecalculateNormals();
		val2.RecalculateBounds();
		val.AddComponent<MeshFilter>().sharedMesh = val2;
		MeshRenderer val3 = val.AddComponent<MeshRenderer>();
		((Renderer)val3).sharedMaterial = mat;
		((Renderer)val3).shadowCastingMode = (ShadowCastingMode)0;
		((Renderer)val3).receiveShadows = false;
		return val;
	}

	static DashboardCluster()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
	}
}
