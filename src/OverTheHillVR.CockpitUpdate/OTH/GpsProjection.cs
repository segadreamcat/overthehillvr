using System;
using System.Numerics;

namespace OTH;

internal static class GpsProjection
{
	internal static Vector2 WorldUv(Vector2 world, Vector2 origin, float width, Vector2 border)
	{
		return border + (world - origin) / width * (Vector2.One - 2f * border);
	}

	internal static (Vector2 min, Vector2 size) Window(Vector2 player, float width, float range, float aspect, Vector2 border)
	{
		Vector2 value = new Vector2(range / width, range / width / aspect) * (Vector2.One - 2f * border);
		value = Vector2.Clamp(value, new Vector2(0.0001f), Vector2.One);
		return (min: Vector2.Clamp(player - value * 0.5f, Vector2.Zero, Vector2.One - value), size: value);
	}

	internal static Vector2 Screen(Vector2 uv, Vector2 min, Vector2 size)
	{
		return (uv - min) / size - new Vector2(0.5f);
	}

	internal static float Heading(float x, float z)
	{
		return MathF.Atan2(x, z) * 180f / (float)Math.PI;
	}
}
