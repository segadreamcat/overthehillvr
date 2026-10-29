using System;
using System.Collections.Generic;
using System.Numerics;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace OTH;

internal sealed class DashboardTilt
{
	internal static MelonPreferences_Entry<bool> Enabled;

	private GameObject root;

	private Transform body;

	private Transform rollNeedle;

	private Transform pitchNeedle;

	private DashboardCluster.Glyph rollText;

	private DashboardCluster.Glyph pitchText;

	private Texture2D font;

	private readonly List<Material> materials = new List<Material>();

	private readonly Dictionary<IntPtr, (Graphic graphic, bool enabled)> originals = new Dictionary<IntPtr, (Graphic, bool)>();

	private float nextSearch;

	internal static readonly Vector3 Position = new Vector3(0.035f, 1.205f, 0.12f);

	internal void Update()
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		Transform val = CockpitUpdate.Read<Transform>("body");
		bool flag = Enabled.Value && (Object)(object)val != (Object)null && CockpitUpdate.Read<bool>("inCockpit");
		if (!flag || (root != null && ((Object)(object)root == (Object)null || (Object)(object)body == (Object)null || ((Il2CppObjectBase)(object)val).Pointer != ((Il2CppObjectBase)(object)body).Pointer)))
		{
			Restore();
			if (!flag)
			{
				return;
			}
		}
		if ((Object)(object)root == (Object)null)
		{
			Build(val);
		}
		if (Time.unscaledTime >= nextSearch)
		{
			nextSearch = Time.unscaledTime + 1f;
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<ContextUIInclineIndicator>(), true))
			{
				ContextUIInclineIndicator val2 = ((Il2CppObjectBase)(object)item).TryCast<ContextUIInclineIndicator>();
				if ((Object)(object)val2 == (Object)null)
				{
					continue;
				}
				foreach (Graphic componentsInChild in ((Component)val2).GetComponentsInChildren<Graphic>(true))
				{
					if (!originals.ContainsKey(((Il2CppObjectBase)(object)componentsInChild).Pointer))
					{
						originals.Add(((Il2CppObjectBase)(object)componentsInChild).Pointer, (componentsInChild, ((Behaviour)componentsInChild).enabled));
					}
					((Behaviour)componentsInChild).enabled = false;
				}
			}
		}
		Vector3 forward = body.forward;
		Vector3 right = body.right;
		Vector3 up = body.up;
		Vector2 vector = VehicleTilt.Degrees(new Vector3(forward.x, forward.y, forward.z), new Vector3(right.x, right.y, right.z), new Vector3(up.x, up.y, up.z));
		rollNeedle.localRotation = Quaternion.Euler(0f, 0f, 0f - vector.Y);
		pitchNeedle.localRotation = Quaternion.Euler(0f, 0f, vector.X);
		rollText.Set("ROLL " + MathF.Round(vector.Y).ToString("+0;-0;0"));
		pitchText.Set("PITCH " + MathF.Round(vector.X).ToString("+0;-0;0"));
	}

	private Material Mat(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Material val = DashboardCluster.CreateMat(null, color);
		materials.Add(val);
		return val;
	}

	private void Build(Transform current)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected Obj, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		body = current;
		root = new GameObject("OTHVR Center Dash Tilt");
		root.transform.SetParent(body, false);
		root.transform.localPosition = Position;
		root.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
		font = DashboardCluster.Load("Font");
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		((Object)val).name = "Tilt instrument housing";
		val.transform.SetParent(root.transform, false);
		val.transform.localPosition = new Vector3(0f, 0f, 0.009f);
		val.transform.localScale = new Vector3(0.238f, 0.104f, 0.016f);
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		((Renderer)val.GetComponent<MeshRenderer>()).sharedMaterial = Mat(new Color(0.018f, 0.023f, 0.025f));
		Color color = new Color(0.75f, 0.67f, 0.51f);
		Color color2 = new Color(0.22f, 0.27f, 0.28f);
		for (int i = 0; i < 2; i++)
		{
			float num = ((i == 0) ? (-0.058f) : 0.058f);
			DashboardCluster.Quad("Level reference", root.transform, new Vector3(num, 0.009f, -0.001f), 0.092f, 0.0014f, Mat(color2));
			for (int j = -2; j <= 2; j++)
			{
				DashboardCluster.Quad("Tilt tick", root.transform, new Vector3(num + (float)j * 0.02f, 0.009f, -0.0015f), 0.0012f, (j == 0) ? 0.013f : 0.006f, Mat(color2));
			}
			GameObject val2 = new GameObject((i == 0) ? "Live roll" : "Live pitch");
			val2.transform.SetParent(root.transform, false);
			val2.transform.localPosition = new Vector3(num, 0.01f, -0.003f);
			DashboardCluster.Quad("Vehicle level bar", val2.transform, Vector3.zero, 0.067f, 0.003f, Mat(color));
			DashboardCluster.Quad("Vehicle center mark", val2.transform, new Vector3(0f, 0.005f, 0f), 0.003f, 0.011f, Mat(color));
			DashboardCluster.Glyph glyph = new DashboardCluster.Glyph((i == 0) ? "Roll reading" : "Pitch reading", root.transform, new Vector3(num, -0.029f, -0.004f), 0.0125f, (Texture)(object)font, color);
			materials.Add(glyph.mat);
			if (i == 0)
			{
				rollNeedle = val2.transform;
				rollText = glyph;
			}
			else
			{
				pitchNeedle = val2.transform;
				pitchText = glyph;
			}
		}
		CockpitUpdate.Self.Log("Mesh vehicle level instrument built at " + ((object)Position/*cast due to constrained. prefix*/).ToString() + " with live roll/pitch and fixed backlight.");
	}

	internal void Restore()
	{
		foreach (var value in originals.Values)
		{
			if ((Object)(object)value.graphic != (Object)null)
			{
				((Behaviour)value.graphic).enabled = value.enabled;
			}
		}
		originals.Clear();
		if ((Object)(object)root != (Object)null)
		{
			Object.Destroy((Object)(object)root);
		}
		foreach (Material material in materials)
		{
			if ((Object)(object)material != (Object)null)
			{
				Object.Destroy((Object)(object)material);
			}
		}
		materials.Clear();
		if ((Object)(object)font != (Object)null)
		{
			Object.Destroy((Object)(object)font);
		}
		root = null;
		body = null;
		font = null;
	}

	static DashboardTilt()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
	}
}
