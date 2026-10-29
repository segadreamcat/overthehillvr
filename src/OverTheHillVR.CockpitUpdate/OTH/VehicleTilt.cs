using System;
using System.Numerics;

namespace OTH;

public static class VehicleTilt
{
	public static Vector2 Degrees(Vector3 forward, Vector3 right, Vector3 up)
	{
		float num = MathF.Atan2(forward.Y, MathF.Sqrt(forward.X * forward.X + forward.Z * forward.Z)) * 180f / (float)Math.PI;
		float num2 = MathF.Atan2(0f - right.Y, up.Y) * 180f / (float)Math.PI;
		if (!float.IsFinite(num) || !float.IsFinite(num2))
		{
			return Vector2.Zero;
		}
		return new Vector2(num, num2);
	}
}
