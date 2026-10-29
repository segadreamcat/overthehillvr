using System;
using System.Numerics;

namespace OTH;

public sealed class ShifterMotion
{
	private Vector2 goal;

	private bool initialized;

	private bool viaNeutral;

	public Vector2 Pose { get; private set; }

	public static Vector2 GearPose(int gear, bool park)
	{
		if (park && gear == 0)
		{
			return new Vector2(16f, 0f);
		}
		if (gear < 0)
		{
			return new Vector2(-10f, -8f);
		}
		if (gear == 0)
		{
			return Vector2.Zero;
		}
		return new Vector2((gear % 2 == 1) ? 10 : (-10), (gear <= 2) ? 8 : ((gear > 4) ? (-8) : 0));
	}

	public static float TransferPitch(bool low)
	{
		return low ? (-22) : 0;
	}

	public void Reset(int gear, bool park)
	{
		Pose = (goal = GearPose(gear, park));
		initialized = true;
		viaNeutral = false;
	}

	public Vector2 Update(int gear, bool park, float dt)
	{
		if (!initialized)
		{
			Reset(gear, park);
		}
		Vector2 vector = GearPose(gear, park);
		if (vector != goal)
		{
			goal = vector;
			viaNeutral = Math.Abs(Pose.Y - goal.Y) > 0.5f && Pose.LengthSquared() > 0.09f && goal.LengthSquared() > 0.09f;
		}
		if (!float.IsFinite(dt) || dt <= 0f)
		{
			return Pose;
		}
		Vector2 vector2 = (viaNeutral ? Vector2.Zero : goal);
		float num = 1f - MathF.Exp((0f - Math.Clamp(dt, 0f, 0.1f)) / 0.055f);
		Pose += (vector2 - Pose) * num;
		if (viaNeutral && Pose.LengthSquared() < 0.09f)
		{
			Pose = Vector2.Zero;
			viaNeutral = false;
		}
		else if (Vector2.DistanceSquared(Pose, vector2) < 0.0001f)
		{
			Pose = vector2;
		}
		return Pose;
	}
}
