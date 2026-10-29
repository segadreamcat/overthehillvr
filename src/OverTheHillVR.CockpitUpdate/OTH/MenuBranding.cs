using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace OTH;

internal sealed class MenuBranding
{
	internal static MelonPreferences_Entry<bool> Enabled;

	private GameObject suffix;

	private float nextSearch;

	internal void Update()
	{
		if ((Object)(object)suffix != (Object)null)
		{
			suffix.SetActive(Enabled.Value);
		}
		else
		{
			if (!Enabled.Value || Time.unscaledTime < nextSearch)
			{
				return;
			}
			nextSearch = Time.unscaledTime + 2f;
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<ContextUIMainMenu>()))
			{
				ContextUIMainMenu val = ((Il2CppObjectBase)(object)item).TryCast<ContextUIMainMenu>();
				if ((Object)(object)val == (Object)null)
				{
					continue;
				}
				foreach (Image componentsInChild in ((Component)val).GetComponentsInChildren<Image>(true))
				{
					if ((Object)(object)componentsInChild.sprite != (Object)null && ((Object)componentsInChild.sprite).name == "oth_logo_white")
					{
						Build(componentsInChild);
						return;
					}
				}
			}
		}
	}

	private void Build(Image logo)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected Obj, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected Obj, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Expected Obj, but got Unknown
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		Sprite sprite = logo.sprite;
		Rect region = sprite.textureRect;
		Texture2D source = sprite.texture;
		int num = Mathf.RoundToInt(region.width);
		int num2 = Mathf.RoundToInt(region.height);
		RenderTexture val = new RenderTexture(num, num2, 0, (RenderTextureFormat)0);
		val.Create();
		RenderTexture active = RenderTexture.active;
		Texture2D val2 = null;
		List<Rect> list = new List<Rect>();
		try
		{
			Graphics.Blit((Texture)(object)source, val, new Vector2(region.width / (float)((Texture)source).width, region.height / (float)((Texture)source).height), new Vector2(region.x / (float)((Texture)source).width, region.y / (float)((Texture)source).height));
			RenderTexture.active = val;
			val2 = new Texture2D(num, num2, (TextureFormat)4, false);
			val2.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
			val2.Apply();
			Il2CppStructArray<Color32> pixels = val2.GetPixels32();
			int num3 = -1;
			int num4 = num2;
			int num5 = -1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = false;
				int num6 = num2;
				int num7 = -1;
				if (i < num)
				{
					for (int j = 0; j < num2; j++)
					{
						if (pixels[j * num + i].a > 48)
						{
							flag = true;
							num6 = Math.Min(num6, j);
							num7 = Math.Max(num7, j);
						}
					}
				}
				if (flag)
				{
					if (num3 < 0)
					{
						num3 = i;
						num4 = num2;
						num5 = -1;
					}
					num4 = Math.Min(num4, num6);
					num5 = Math.Max(num5, num7);
				}
				else if (num3 >= 0)
				{
					if (i - num3 > 1)
					{
						list.Add(new Rect((float)num3, (float)num4, (float)(i - num3), (float)(num5 - num4 + 1)));
					}
					num3 = -1;
				}
			}
		}
		finally
		{
			RenderTexture.active = active;
			val.Release();
			Object.Destroy((Object)(object)val);
			if ((Object)(object)val2 != (Object)null)
			{
				Object.Destroy((Object)(object)val2);
			}
		}
		if (list.Count < 11)
		{
			throw new Exception("Cannot identify the original logo letters (found " + list.Count + ").");
		}
		Rect pixelAdjustedRect = ((Graphic)logo).GetPixelAdjustedRect();
		float num8 = pixelAdjustedRect.width / (float)num;
		float num9 = pixelAdjustedRect.height / (float)num2;
		if (logo.preserveAspect)
		{
			float num10 = Mathf.Min(num8, num9);
			pixelAdjustedRect.x += (pixelAdjustedRect.width - (float)num * num10) * 0.5f;
			pixelAdjustedRect.y += (pixelAdjustedRect.height - (float)num2 * num10) * 0.5f;
			num8 = (num9 = num10);
		}
		Rect letter = list[1];
		Rect letter2 = list[3];
		float num11 = 0.9f;
		Rect val3 = list[2];
		float num12 = Mathf.Max(1f, (val3.xMin - letter.xMax) * num8 * num11);
		float num13 = (letter.width + letter2.width) * num8 * num11 + num12;
		float xMin = pixelAdjustedRect.xMin;
		val3 = list[list.Count - 1];
		float num14 = xMin + val3.xMax * num8;
		float num15 = num2;
		foreach (Rect item in list)
		{
			Rect current = item;
			num15 = Mathf.Min(num15, current.yMin);
		}
		float y = pixelAdjustedRect.yMin + num15 * num9 - 3f;
		suffix = new GameObject("OTHVR Menu vr");
		RectTransform val4 = suffix.AddComponent<RectTransform>();
		((Transform)val4).SetParent((Transform)(object)((Graphic)logo).rectTransform, false);
		Vector2 anchorMin = (val4.anchorMax = new Vector2(0.5f, 0.5f));
		val4.anchorMin = anchorMin;
		val4.anchoredPosition = Vector2.zero;
		val4.sizeDelta = Vector2.zero;
		((Transform)val4).localPosition = Vector3.zero;
		Glyph("v", letter, num14 - num13, y, letter.width * num8 * num11, letter.height * num9 * num11);
		Glyph("r", letter2, num14 - letter2.width * num8 * num11, y, letter2.width * num8 * num11, letter2.height * num9 * num11);
		CockpitUpdate.Self.Log("Main-menu lowercase vr added below the title using its original v/r logo glyphs.");
		void Glyph(string name, Rect val10, float x, float num16, float w, float h)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			GameObject val6 = new GameObject("Logo " + name);
			RectTransform val7 = val6.AddComponent<RectTransform>();
			((Transform)val7).SetParent(suffix.transform, false);
			Vector2 anchorMin2 = (val7.anchorMax = new Vector2(0.5f, 0.5f));
			val7.anchorMin = anchorMin2;
			val7.pivot = new Vector2(0f, 1f);
			val7.anchoredPosition = new Vector2(x, num16);
			val7.sizeDelta = new Vector2(w, h);
			RawImage val9 = val6.AddComponent<RawImage>();
			val9.texture = (Texture)(object)source;
			((Graphic)val9).color = ((Graphic)logo).color;
			((Graphic)val9).raycastTarget = false;
			val9.uvRect = new Rect((region.x + val10.x) / (float)((Texture)source).width, (region.y + val10.y) / (float)((Texture)source).height, val10.width / (float)((Texture)source).width, val10.height / (float)((Texture)source).height);
		}
	}
}
