using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OTH;

internal static class StaticWiperGeometry
{
	private sealed class Part
	{
		internal readonly List<int> vertices = new List<int>();

		internal Vector3 min = new Vector3(float.MaxValue);

		internal Vector3 max = new Vector3(float.MinValue);

		internal Vector3 Size => max - min;

		internal Vector3 Center => (min + max) * 0.5f;
	}

	internal static bool[] Find(Vector3[] points, int[] triangles)
	{
		int[] parent = Enumerable.Range(0, points.Length).ToArray();
		Dictionary<(int, int, int), int> dictionary = new Dictionary<(int, int, int), int>();
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 vector = points[i];
			(int, int, int) key = ((int)MathF.Round(vector.X * 10000f), (int)MathF.Round(vector.Y * 10000f), (int)MathF.Round(vector.Z * 10000f));
			if (dictionary.TryGetValue(key, out var value))
			{
				Join(i, value);
			}
			else
			{
				dictionary.Add(key, i);
			}
		}
		for (int j = 0; j + 2 < triangles.Length; j += 3)
		{
			int num = triangles[j];
			int num2 = triangles[j + 1];
			int num3 = triangles[j + 2];
			if (num < 0 || num2 < 0 || num3 < 0 || num >= points.Length || num2 >= points.Length || num3 >= points.Length)
			{
				return new bool[points.Length];
			}
			Join(num, num2);
			Join(num, num3);
		}
		Dictionary<int, Part> dictionary2 = new Dictionary<int, Part>();
		for (int k = 0; k < points.Length; k++)
		{
			int key2 = Root(k);
			if (!dictionary2.TryGetValue(key2, out var value2))
			{
				dictionary2.Add(key2, value2 = new Part());
			}
			value2.vertices.Add(k);
			value2.min = Vector3.Min(value2.min, points[k]);
			value2.max = Vector3.Max(value2.max, points[k]);
		}
		Part[] array = dictionary2.Values.Where((Part p) => Window(p) && p.min.Y > 1.2f && p.Size.X > 0.26f && p.Size.X < 0.75f && p.Size.Y < 0.08f && p.Size.Z < 0.1f).ToArray();
		bool[] hidden = new bool[points.Length];
		for (int num4 = 0; num4 < array.Length; num4++)
		{
			for (int num5 = num4 + 1; num5 < array.Length; num5++)
			{
				Part left = ((array[num4].Center.X < array[num5].Center.X) ? array[num4] : array[num5]);
				Part right = ((left == array[num4]) ? array[num5] : array[num4]);
				float spacing = right.Center.X - left.Center.X;
				if (left.Center.X > 0.05f || right.Center.X < -0.05f || !SamePair(left, right, spacing))
				{
					continue;
				}
				Part[] array2 = dictionary2.Values.Where((Part p) => Window(p) && p.Size.X > 0.15f && p.Size.X < 0.5f && p.Size.Y > 0.08f && p.Size.Y < 0.25f && p.Size.Z < 0.16f && p.max.Y < MathF.Max(left.max.Y, right.max.Y) + 0.04f).ToArray();
				Part[] array3 = array2;
				foreach (Part part in array3)
				{
					Part[] array4 = array2;
					foreach (Part part2 in array4)
					{
						if (part != part2 && !(part.Center.X >= part2.Center.X) && Touches(part, left) && Touches(part2, right) && SamePair(part, part2, spacing))
						{
							Hide(left);
							Hide(right);
							Hide(part);
							Hide(part2);
						}
					}
				}
			}
		}
		return hidden;
		void Hide(Part p)
		{
			foreach (int vertex in p.vertices)
			{
				hidden[vertex] = true;
			}
		}
		void Join(int a, int b)
		{
			parent[Root(a)] = Root(b);
		}
		int Root(int num8)
		{
			while (parent[num8] != num8)
			{
				parent[num8] = parent[parent[num8]];
				num8 = parent[num8];
			}
			return num8;
		}
		static bool SamePair(Part a, Part b, float num8)
		{
			if (MathF.Abs(a.Center.X - b.Center.X) > 0.3f && MathF.Abs(a.Center.X - b.Center.X) < 0.95f && MathF.Abs(MathF.Abs(a.Center.X - b.Center.X) - num8) < 0.1f && MathF.Abs(a.Center.Y - b.Center.Y) < 0.03f && MathF.Abs(a.Center.Z - b.Center.Z) < 0.05f)
			{
				return Vector3.Distance(a.Size, b.Size) < 0.08f;
			}
			return false;
		}
		static bool Touches(Part a, Part b)
		{
			if (a.min.X <= b.max.X + 0.025f && a.max.X >= b.min.X - 0.025f && a.min.Y <= b.max.Y + 0.025f && a.max.Y >= b.min.Y - 0.025f && a.min.Z <= b.max.Z + 0.025f)
			{
				return a.max.Z >= b.min.Z - 0.025f;
			}
			return false;
		}
		static bool Window(Part p)
		{
			if (p.min.Y > 1.15f && p.max.Y < 1.72f && p.min.Z > 0.2f && p.max.Z < 1.1f && p.min.X > -0.85f)
			{
				return p.max.X < 0.85f;
			}
			return false;
		}
	}
}
