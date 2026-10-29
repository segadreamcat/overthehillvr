using System;

namespace OTH;

public static class WheelResponse
{
	public static float Axis(int raw)
	{
		return Math.Clamp(((float)raw - 32767.5f) / 32767.5f, -1f, 1f);
	}

	public static float Steering(int raw, float hardwareDegrees, float drivingDegrees)
	{
		float value = Axis(raw);
		value = (float)Math.Sign(value) * Math.Max(0f, (Math.Abs(value) - 0.003f) / 0.997f);
		return Math.Clamp(value * Math.Clamp(hardwareDegrees, 180f, 1080f) / Math.Clamp(drivingDegrees, 180f, 1080f), -1f, 1f);
	}

	public static float Pedal(int raw, bool inverted)
	{
		float num = Math.Clamp((float)raw / 65535f, 0f, 1f);
		if (inverted)
		{
			num = 1f - num;
		}
		return Math.Clamp((num - 0.02f) / 0.96f, 0f, 1f);
	}
}
