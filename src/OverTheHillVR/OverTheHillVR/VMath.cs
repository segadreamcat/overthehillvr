using System;
using UnityEngine;
using Valve.VR;

namespace OverTheHillVR;

internal static class VMath
{
	public static readonly Quaternion Identity = Q(0f, 0f, 0f, 1f);

	public static Vector3 V(float x, float y, float z)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = default;
		result.x = x;
		result.y = y;
		result.z = z;
		return result;
	}

	public static Quaternion Q(float x, float y, float z, float w)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Quaternion result = default;
		result.x = x;
		result.y = y;
		result.z = z;
		result.w = w;
		return result;
	}

	public static Vector3 Add(Vector3 a, Vector3 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return V(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3 Sub(Vector3 a, Vector3 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return V(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3 Mul(Vector3 a, float s)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return V(a.x * s, a.y * s, a.z * s);
	}

	public static Quaternion Mul(Quaternion a, Quaternion b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		return Q(a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y, a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x, a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w, a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
	}

	public static Quaternion Inverse(Quaternion q)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Q(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static Vector3 Rotate(Quaternion q, Vector3 v)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		float num = 2f * (q.y * v.z - q.z * v.y);
		float num2 = 2f * (q.z * v.x - q.x * v.z);
		float num3 = 2f * (q.x * v.y - q.y * v.x);
		return V(v.x + q.w * num + (q.y * num3 - q.z * num2), v.y + q.w * num2 + (q.z * num - q.x * num3), v.z + q.w * num3 + (q.x * num2 - q.y * num));
	}

	public static Quaternion Yaw(float deg)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		float num = deg * ((float)Math.PI / 180f) * 0.5f;
		return Q(0f, (float)Math.Sin(num), 0f, (float)Math.Cos(num));
	}

	public static Quaternion Pitch(float deg)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		float num = deg * ((float)Math.PI / 180f) * 0.5f;
		return Q((float)Math.Sin(num), 0f, 0f, (float)Math.Cos(num));
	}

	public static float YawOf(Quaternion q)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Rotate(q, V(0f, 0f, 1f));
		if (val.x * val.x + val.z * val.z < 0.01f)
		{
			Vector3 val2 = Rotate(q, V(0f, 1f, 0f));
			float num = ((val.y < 0f) ? 1f : (-1f));
			val = V(val2.x * num, 0f, val2.z * num);
		}
		return (float)(Math.Atan2(val.x, val.z) * 57.295780181884766);
	}

	public static Quaternion Nlerp(Quaternion a, Quaternion b, float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w < 0f)
		{
			b = Q(0f - b.x, 0f - b.y, 0f - b.z, 0f - b.w);
		}
		Quaternion val = Q(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
		float num = (float)Math.Sqrt(val.x * val.x + val.y * val.y + val.z * val.z + val.w * val.w);
		if (!(num < 1E-06f))
		{
			return Q(val.x / num, val.y / num, val.z / num, val.w / num);
		}
		return b;
	}

	public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		return V(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	public static Vector3 PosOf(HmdMatrix34_t m)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return V(m.m3, m.m7, 0f - m.m11);
	}

	public static Quaternion RotOf(HmdMatrix34_t m)
	{
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		float m2 = m.m0;
		float m3 = m.m1;
		float num = 0f - m.m2;
		float m4 = m.m4;
		float m5 = m.m5;
		float num2 = 0f - m.m6;
		float num3 = 0f - m.m8;
		float num4 = 0f - m.m9;
		float m6 = m.m10;
		float num5 = m2 + m5 + m6;
		float num7;
		float num8;
		float num9;
		float num10;
		if (num5 > 0f)
		{
			float num6 = (float)Math.Sqrt(num5 + 1f) * 2f;
			num7 = 0.25f * num6;
			num8 = (num4 - num2) / num6;
			num9 = (num - num3) / num6;
			num10 = (m4 - m3) / num6;
		}
		else if (m2 > m5 && m2 > m6)
		{
			float num11 = (float)Math.Sqrt(1f + m2 - m5 - m6) * 2f;
			num7 = (num4 - num2) / num11;
			num8 = 0.25f * num11;
			num9 = (m3 + m4) / num11;
			num10 = (num + num3) / num11;
		}
		else if (m5 > m6)
		{
			float num12 = (float)Math.Sqrt(1f + m5 - m2 - m6) * 2f;
			num7 = (num - num3) / num12;
			num8 = (m3 + m4) / num12;
			num9 = 0.25f * num12;
			num10 = (num2 + num4) / num12;
		}
		else
		{
			float num13 = (float)Math.Sqrt(1f + m6 - m2 - m5) * 2f;
			num7 = (m4 - m3) / num13;
			num8 = (num + num3) / num13;
			num9 = (num2 + num4) / num13;
			num10 = 0.25f * num13;
		}
		float num14 = (float)Math.Sqrt(num8 * num8 + num9 * num9 + num10 * num10 + num7 * num7);
		return Q(num8 / num14, num9 / num14, num10 / num14, num7 / num14);
	}

	public static Matrix4x4 Projection(float l, float r, float t, float b, float n, float f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		return new Matrix4x4
		{
			m00 = 2f / (r - l),
			m02 = (r + l) / (r - l),
			m11 = 2f / (b - t),
			m12 = (t + b) / (b - t),
			m22 = (0f - (f + n)) / (f - n),
			m23 = -2f * f * n / (f - n),
			m32 = -1f
		};
	}

	static VMath()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
	}
}
