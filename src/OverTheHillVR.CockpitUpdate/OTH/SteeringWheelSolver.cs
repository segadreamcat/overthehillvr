using System;
using System.Numerics;

namespace OTH;

public sealed class SteeringWheelSolver
{
	private int previousMask;

	private float previousAngle;

	private Vector3 right = Vector3.UnitX;

	private Vector3 center;

	public float Steering { get; private set; }

	public bool Gripping { get; private set; }

	public float Update(bool leftGrip, bool rightGrip, Vector3 left, Vector3 rightHand, Vector3 headRight, float lockDegrees, float dt)
	{
		if (!Finite(left) || !Finite(rightHand) || !Finite(headRight) || !float.IsFinite(dt))
		{
			Reset();
			return 0f;
		}
		int num = (leftGrip ? 1 : 0) | (rightGrip ? 2 : 0);
		Gripping = num != 0;
		switch (num)
		{
		case 0:
			previousMask = 0;
			Steering = MoveToZero(Steering, Math.Clamp(dt, 0f, 0.1f) * 1.8f);
			return Steering;
		case 3:
			if ((left - rightHand).LengthSquared() < 0.0036f)
			{
				previousMask = 0;
				return Steering;
			}
			break;
		}
		if (num != previousMask)
		{
			right = new Vector3(headRight.X, 0f, headRight.Z);
			right = ((right.LengthSquared() < 0.01f) ? Vector3.UnitX : Vector3.Normalize(right));
			center = num switch
			{
				1 => left + right * 0.18f, 
				3 => (left + rightHand) * 0.5f, 
				_ => rightHand - right * 0.18f, 
			};
			previousAngle = Angle(num, left, rightHand);
			previousMask = num;
			return Steering;
		}
		float num2 = Angle(num, left, rightHand);
		float num3 = num2 - previousAngle;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		if (num3 < -180f)
		{
			num3 += 360f;
		}
		if (Math.Abs(num3) < 60f)
		{
			Steering = Math.Clamp(Steering - num3 / (Math.Clamp(lockDegrees, 180f, 1080f) * 0.5f), -1f, 1f);
		}
		previousAngle = num2;
		return Steering;
	}

	private float Angle(int mask, Vector3 l, Vector3 r)
	{
		Vector3 vector = ((mask == 3) ? (r - l) : (((mask == 1) ? l : r) - center));
		return MathF.Atan2(vector.Y, Vector3.Dot(vector, right)) * 180f / (float)Math.PI;
	}

	public void Reset()
	{
		previousMask = 0;
		Steering = 0f;
		Gripping = false;
	}

	public static float Smooth(float current, float target, float dt, float seconds)
	{
		if (!float.IsFinite(current) || !float.IsFinite(target) || !float.IsFinite(dt) || !float.IsFinite(seconds))
		{
			return 0f;
		}
		target = Math.Clamp(target, -1f, 1f);
		if (seconds <= 0f)
		{
			return target;
		}
		float num = 1f - MathF.Exp((0f - Math.Clamp(dt, 0f, 0.1f)) / Math.Clamp(seconds, 0.001f, 0.1f));
		return current + (target - current) * num;
	}

	private static float MoveToZero(float x, float d)
	{
		if (!(Math.Abs(x) <= d))
		{
			return x - (float)Math.Sign(x) * d;
		}
		return 0f;
	}

	private static bool Finite(Vector3 v)
	{
		if (float.IsFinite(v.X) && float.IsFinite(v.Y))
		{
			return float.IsFinite(v.Z);
		}
		return false;
	}
}
