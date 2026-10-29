using Il2Cpp;
using Il2CppDistantLands.Cozy;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace OTH;

internal sealed class WindshieldWeather
{
	private WeatherDetector detector;

	private float nextSearch;

	internal float Read()
	{
		if ((Object)(object)detector == (Object)null && Time.unscaledTime >= nextSearch)
		{
			nextSearch = Time.unscaledTime + 2f;
			foreach (Object item in Object.FindObjectsOfType(Il2CppType.Of<WeatherDetector>()))
			{
				detector = ((Il2CppObjectBase)(object)item).TryCast<WeatherDetector>();
				if ((Object)(object)detector != (Object)null)
				{
					break;
				}
			}
		}
		return Intensity(detector, CozyWeather.instance);
	}

	internal static float Intensity(WeatherDetector detector, CozyWeather weather)
	{
		WeatherDetectorConfig val = (((Object)(object)detector != (Object)null) ? detector.weatherDetectorConfig : null);
		List<WeatherRelation> val2 = (((Object)(object)weather != (Object)null && (Object)(object)weather.weatherModule != (Object)null) ? weather.weatherModule.currentWeatherProfiles : null);
		if ((Object)(object)val != (Object)null && val.rainyWeatherProfiles != null && val.rainyWeatherProfiles.Count > 0 && val2 != null)
		{
			float num = 0f;
			int num2 = 0;
			Enumerator<WeatherRelation> enumerator = val2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeatherRelation current = enumerator.Current;
				if (current != null && !((Object)(object)current.profile == (Object)null))
				{
					num2++;
					if (WeatherDetectorConfig.IsWeatherInList(current.profile.WeatherId, val.rainyWeatherProfiles))
					{
						num += Mathf.Max(0f, current.weight);
					}
				}
			}
			if (num2 > 0)
			{
				if (!float.IsFinite(num))
				{
					return 0f;
				}
				return Mathf.Clamp01(num);
			}
		}
		if ((Object)(object)weather != (Object)null && (Object)(object)weather.climateModule != (Object)null)
		{
			float rainSpeed = weather.climateModule.rainSpeed;
			if (!float.IsFinite(rainSpeed) || !(rainSpeed > 1E-05f))
			{
				return 0f;
			}
			return Mathf.Clamp(0.35f + rainSpeed, 0.35f, 1f);
		}
		if (!((Object)(object)detector != (Object)null) || !detector.IsRaining)
		{
			return 0f;
		}
		return 0.7f;
	}
}
