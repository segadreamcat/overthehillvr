using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.Rendering;

namespace OverTheHillVR;

internal static class Othm
{
	private static readonly Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

	private static GameObject root;

	public static GameObject Spawn(string file, string name, Transform parent)
	{
		GameObject val = Template(file);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		GameObject val2 = Object.Instantiate<GameObject>(val, parent, false);
		((Object)val2).name = name;
		val2.SetActive(true);
		return val2;
	}

	private static GameObject Template(string file)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected Obj, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected Obj, but got Unknown
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Expected Obj, but got Unknown
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected Obj, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected Obj, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		if (cache.TryGetValue(file, out var value) && (Object)(object)value != (Object)null)
		{
			return value;
		}
		string text = Path.Combine(Mod.ModDir, "OverTheHillVR", file);
		if (!File.Exists(text))
		{
			Mod.L.Warning("Model missing: " + text);
			cache[file] = null;
			return null;
		}
		if ((Object)(object)root == (Object)null)
		{
			root = new GameObject("OTHVR Models");
			root.SetActive(false);
			Object.DontDestroyOnLoad((Object)(object)root);
		}
		try
		{
			GameObject val = new GameObject(Path.GetFileNameWithoutExtension(file));
			val.transform.SetParent(root.transform, false);
			using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(text)))
			{
				if (new string(binaryReader.ReadChars(4)) != "OTHM")
				{
					throw new Exception("bad header");
				}
				binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				List<(Color, int, float, float)> list = new List<(Color, int, float, float)>();
				List<Mesh> list2 = new List<Mesh>();
				for (int i = 0; i < num; i++)
				{
					int num2 = binaryReader.ReadInt32();
					int num3 = binaryReader.ReadInt32();
					Vector3[] array = new Vector3[num2];
					Vector3[] array2 = new Vector3[num2];
					Vector2[] array3 = new Vector2[num2];
					int[] array4 = new int[num3];
					for (int j = 0; j < num2; j++)
					{
						array[j] = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
					}
					for (int k = 0; k < num2; k++)
					{
						array2[k] = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
					}
					for (int l = 0; l < num2; l++)
					{
						array3[l] = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
					}
					for (int m = 0; m < num3; m++)
					{
						array4[m] = (int)binaryReader.ReadUInt32();
					}
					Color item = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
					int item2 = binaryReader.ReadInt32();
					float item3 = binaryReader.ReadSingle();
					float item4 = binaryReader.ReadSingle();
					Mesh val2 = new Mesh();
					((Object)val2).name = ((Object)val).name + "_" + i;
					if (num2 > 65000)
					{
						val2.indexFormat = (IndexFormat)1;
					}
					val2.vertices = array;
					val2.normals = array2;
					val2.uv = array3;
					val2.triangles = array4;
					val2.RecalculateBounds();
					list2.Add(val2);
					list.Add((item, item2, item3, item4));
				}
				int num4 = binaryReader.ReadInt32();
				Texture2D[] array5 = new Texture2D[num4];
				for (int n = 0; n < num4; n++)
				{
					byte[] array6 = binaryReader.ReadBytes(binaryReader.ReadInt32());
					Texture2D val3 = new Texture2D(2, 2, (TextureFormat)4, true);
					ImageConversion.LoadImage(val3, (Il2CppStructArray<byte>?)array6);
					((Object)val3).hideFlags = (HideFlags)32;
					array5[n] = val3;
				}
				string[] array7 = new string[list2.Count];
				try
				{
					if (binaryReader.BaseStream.Position + 8 <= binaryReader.BaseStream.Length && new string(binaryReader.ReadChars(4)) == "NAME")
					{
						int num5 = binaryReader.ReadInt32();
						for (int num6 = 0; num6 < num5; num6++)
						{
							string text2 = Encoding.UTF8.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32()));
							if (num6 < array7.Length)
							{
								array7[num6] = text2;
							}
						}
					}
				}
				catch
				{
				}
				Shader val4 = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Universal Render Pipeline/Simple Lit");
				for (int num7 = 0; num7 < list2.Count; num7++)
				{
					GameObject val5 = new GameObject((array7[num7] != null) ? ("part_" + array7[num7]) : ("part" + num7));
					val5.transform.SetParent(val.transform, false);
					val5.AddComponent<MeshFilter>().sharedMesh = list2[num7];
					MeshRenderer val6 = val5.AddComponent<MeshRenderer>();
					Material val7 = new Material(val4);
					var (val8, num8, num9, num10) = list[num7];
					val7.SetColor("_BaseColor", val8);
					if (num8 >= 0 && num8 < array5.Length)
					{
						val7.SetTexture("_BaseMap", (Texture)(object)array5[num8]);
						val7.mainTexture = (Texture)(object)array5[num8];
					}
					val7.SetFloat("_Metallic", num9);
					val7.SetFloat("_Smoothness", Mathf.Clamp01(1f - num10));
					((Renderer)val6).sharedMaterial = val7;
					((Renderer)val6).shadowCastingMode = (ShadowCastingMode)0;
				}
			}
			cache[file] = val;
			Mod.L.Msg("Loaded model " + file + ".");
			return val;
		}
		catch (Exception ex)
		{
			Mod.L.Warning("Model " + file + " failed: " + ex.Message);
			cache[file] = null;
			return null;
		}
	}
}
