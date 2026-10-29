using System;
using System.Numerics;

namespace OTH;

internal sealed class WindshieldSimulation
{
	internal struct Drop
	{
		internal Vector2 position;

		internal float radius;

		internal float age;

		internal float lifetime;

		internal float slide;

		internal bool alive;
	}

	internal const int Capacity = 512;

	internal const float HalfWidth = 0.725f;

	internal const float Height = 0.695f;

	internal const float ParkDegrees = 5f;

	internal const float TopDegrees = 92f;

	internal const float BladeInner = 0.265f;

	internal const float BladeOuter = 0.635f;

	internal static readonly Vector2 LeftPivot = new Vector2(-0.66f, 0.018f);

	internal static readonly Vector2 RightPivot = new Vector2(0.035f, 0.018f);

	internal readonly Drop[] Drops = new Drop[512];

	private readonly Random random;

	private float phase;

	private float dryFor;

	private float spawnBudget;

	private int cursor;

	internal float Angle { get; private set; } = 5f;

	internal bool Running { get; private set; }

	internal int Count { get; private set; }

	internal WindshieldSimulation(int seed = 7139)
	{
		random = new Random(seed);
	}

	internal void Step(float rain, float delta)
	{
		if (!float.IsFinite(rain))
		{
			rain = 0f;
		}
		if (float.IsFinite(delta) && !(delta <= 0f))
		{
			rain = Math.Clamp(rain, 0f, 1f);
			delta = Math.Min(delta, 0.1f);
			int num = (int)MathF.Ceiling(delta * 120f);
			for (int i = 0; i < num; i++)
			{
				Tick(rain, delta / (float)num);
			}
		}
	}

	private void Tick(float rain, float dt)
	{
		bool flag = rain > 0.025f;
		dryFor = (flag ? 0f : (dryFor + dt));
		if (flag && !Running)
		{
			Running = true;
			phase = 0f;
		}
		float angle = Angle;
		if (Running)
		{
			phase += dt * (0.45f + 0.75f * rain);
			if (phase >= 1f)
			{
				phase--;
				if (dryFor >= 2.5f)
				{
					Running = false;
					phase = 0f;
				}
			}
			Angle = (Running ? (5f + 87f * (0.5f - 0.5f * MathF.Cos(phase * 2f * (float)Math.PI))) : 5f);
		}
		if (flag)
		{
			spawnBudget += dt * (35f + 185f * rain);
			while (spawnBudget >= 1f)
			{
				Spawn();
				spawnBudget--;
			}
		}
		for (int i = 0; i < Drops.Length; i++)
		{
			ref Drop reference = ref Drops[i];
			if (reference.alive)
			{
				reference.age += dt;
				reference.slide = ((reference.radius > 0.0032f) ? Math.Min(0.07f, 0.006f + reference.age * 0.004f) : 0.0007f);
				reference.position.Y -= reference.slide * dt;
				if ((Running && (Swept(reference.position, LeftPivot, angle, Angle, reference.radius) || Swept(reference.position, RightPivot, angle, Angle, reference.radius))) || reference.position.Y < 0.008f || reference.age >= reference.lifetime)
				{
					reference.alive = false;
					Count--;
				}
			}
		}
	}

	private void Spawn()
	{
		int num = -1;
		for (int i = 0; i < Drops.Length; i++)
		{
			int num2 = (cursor + i) % Drops.Length;
			if (!Drops[num2].alive)
			{
				num = num2;
				break;
			}
		}
		if (num >= 0)
		{
			cursor = (num + 1) % Drops.Length;
			Drops[num] = new Drop
			{
				position = new Vector2((float)random.NextDouble() * 1.4f - 0.7f, 0.018f + (float)random.NextDouble() * 0.65999997f),
				radius = 0.0013f + (float)Math.Pow(random.NextDouble(), 1.7) * 0.0037f,
				lifetime = 12f + (float)random.NextDouble() * 17f,
				alive = true
			};
			Count++;
		}
	}

	internal static bool Swept(Vector2 point, Vector2 pivot, float from, float to, float radius = 0f)
	{
		Vector2 vector = point - pivot;
		float num = vector.Length();
		if (num < 0.265f - radius || num > 0.635f + radius)
		{
			return false;
		}
		float num2 = MathF.Atan2(vector.Y, vector.X) * 180f / (float)Math.PI;
		float num3 = 1.2f + MathF.Atan2(radius, Math.Max(0.01f, num)) * 180f / (float)Math.PI;
		if (num2 >= Math.Min(from, to) - num3)
		{
			return num2 <= Math.Max(from, to) + num3;
		}
		return false;
	}
}
