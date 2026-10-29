using System;

namespace OTH;

public static class DashboardReadings
{
	public static float Kmh(float metersPerSecond)
	{
		if (!float.IsFinite(metersPerSecond))
		{
			return 0f;
		}
		return Math.Abs(metersPerSecond) * 3.6f;
	}

	public static float Mph(float metersPerSecond)
	{
		if (!float.IsFinite(metersPerSecond))
		{
			return 0f;
		}
		return Math.Abs(metersPerSecond) * 2.2369363f;
	}

	public static float Needle(float speed, bool mph)
	{
		return 135f - 270f * Math.Clamp(speed / (mph ? 100f : 160f), 0f, 1f);
	}

	public static string Gear(short code)
	{
		if (code != 0)
		{
			if (code >= 0)
			{
				return code.ToString();
			}
			return "R";
		}
		return "N";
	}
}
