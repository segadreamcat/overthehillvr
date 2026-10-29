using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace OTH;

internal sealed class StaticWiperRemoval
{
	private sealed class Patch
	{
		internal MeshFilter filter;

		internal Mesh original;

		internal Mesh replacement;
	}

	private readonly List<Patch> patches = new List<Patch>();

	private readonly HashSet<(IntPtr filter, IntPtr mesh)> checkedMeshes = new HashSet<(IntPtr, IntPtr)>();

	private Transform body;

	private float nextSearch;

	internal void Update(Transform current)
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)body != (Object)null && ((Object)(object)current == (Object)null || ((Il2CppObjectBase)(object)body).Pointer != ((Il2CppObjectBase)(object)current).Pointer))
		{
			Dispose();
		}
		body = current;
		foreach (Patch patch in patches)
		{
			if ((Object)(object)patch.filter != (Object)null && (Object)(object)patch.original != (Object)null && (Object)(object)patch.replacement != (Object)null && (Object)(object)patch.filter.sharedMesh == (Object)(object)patch.original)
			{
				patch.filter.sharedMesh = patch.replacement;
			}
		}
		if ((Object)(object)body == (Object)null || Time.unscaledTime < nextSearch)
		{
			return;
		}
		nextSearch = Time.unscaledTime + 1f;
		foreach (MeshFilter componentsInChild in ((Component)body).GetComponentsInChildren<MeshFilter>(true))
		{
			Mesh sharedMesh = componentsInChild.sharedMesh;
			if (!((Object)(object)sharedMesh == (Object)null) && ((Object)sharedMesh).name.IndexOf("_body_LOD", StringComparison.OrdinalIgnoreCase) >= 0 && checkedMeshes.Add((((Il2CppObjectBase)(object)componentsInChild).Pointer, ((Il2CppObjectBase)(object)sharedMesh).Pointer)) && (int)sharedMesh.GetVertexAttributeFormat((VertexAttribute)0) == 0)
			{
				Mesh val;
				int removed;
				try
				{
					val = CreateReplacement(componentsInChild, body, out removed);
				}
				catch (Exception ex)
				{
					CockpitUpdate.Self.Warn("Static wiper mesh: " + ex.Message);
					continue;
				}
				if (!((Object)(object)val == (Object)null))
				{
					patches.Add(new Patch
					{
						filter = componentsInChild,
						original = sharedMesh,
						replacement = val
					});
					componentsInChild.sharedMesh = val;
					CockpitUpdate.Self.Log($"Static windshield wipers hidden from {((Object)sharedMesh).name}: {removed} triangles; original mesh retained for exterior views.");
				}
			}
		}
	}

	internal static Mesh CreateReplacement(MeshFilter filter, Transform body, out int removed)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Expected Obj, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		removed = 0;
		Mesh sharedMesh = filter.sharedMesh;
		Il2CppStructArray<byte>[] array = new Il2CppStructArray<byte>[sharedMesh.vertexBufferCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ReadBuffer(sharedMesh.GetVertexBuffer(i));
		}
		int vertexAttributeStream = sharedMesh.GetVertexAttributeStream((VertexAttribute)0);
		int vertexBufferStride = sharedMesh.GetVertexBufferStride(vertexAttributeStream);
		int vertexAttributeOffset = sharedMesh.GetVertexAttributeOffset((VertexAttribute)0);
		byte[] value = array[vertexAttributeStream].ToArray();
		Vector3[] array2 = new Vector3[sharedMesh.vertexCount];
		Matrix4x4 val = body.worldToLocalMatrix * ((Component)filter).transform.localToWorldMatrix;
		for (int j = 0; j < array2.Length; j++)
		{
			int num = j * vertexBufferStride + vertexAttributeOffset;
			Vector3 val2 = val.MultiplyPoint3x4(new Vector3(BitConverter.ToSingle(value, num), BitConverter.ToSingle(value, num + 4), BitConverter.ToSingle(value, num + 8)));
			array2[j] = new Vector3(val2.x, val2.y, val2.z);
		}
		byte[] indexBytes = ReadBuffer(sharedMesh.GetIndexBuffer()).ToArray();
		int width = (((int)sharedMesh.indexFormat == 0) ? 2 : 4);
		List<int[]> list = new List<int[]>();
		List<int> list2 = new List<int>();
		for (int k = 0; k < sharedMesh.subMeshCount; k++)
		{
			SubMeshDescriptor subMesh = sharedMesh.GetSubMesh(k);
			if ((int)subMesh.topology != 0)
			{
				return null;
			}
			int[] array3 = new int[subMesh.indexCount];
			for (int l = 0; l < array3.Length; l++)
			{
				array3[l] = ReadIndex(subMesh.indexStart + l) + subMesh.baseVertex;
			}
			list.Add(array3);
			list2.AddRange(array3);
		}
		bool[] array4 = StaticWiperGeometry.Find(array2, list2.ToArray());
		if (!array4.Any((bool v) => v))
		{
			return null;
		}
		List<int[]> list3 = new List<int[]>();
		foreach (int[] item in list)
		{
			List<int> list4 = new List<int>();
			for (int num2 = 0; num2 + 2 < item.Length; num2 += 3)
			{
				if (array4[item[num2]] && array4[item[num2 + 1]] && array4[item[num2 + 2]])
				{
					removed++;
					continue;
				}
				list4.Add(item[num2]);
				list4.Add(item[num2 + 1]);
				list4.Add(item[num2 + 2]);
			}
			list3.Add(list4.ToArray());
		}
		if (removed == 0)
		{
			return null;
		}
		Mesh val3 = new Mesh();
		try
		{
			((Object)val3).name = "OTHVR Body Without Static Wipers";
			val3.indexFormat = sharedMesh.indexFormat;
			val3.SetVertexBufferParams(sharedMesh.vertexCount, sharedMesh.GetVertexAttributes());
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				val3.SetVertexBufferData<byte>((Il2CppArrayBase<byte>)array[num3], 0, 0, array[num3].Length, num3, (MeshUpdateFlags)8);
			}
			val3.subMeshCount = list3.Count;
			for (int num4 = 0; num4 < list3.Count; num4++)
			{
				val3.SetTriangles((Il2CppStructArray<int>?)list3[num4], num4, false);
			}
			val3.bounds = sharedMesh.bounds;
			return val3;
		}
		catch
		{
			Object.Destroy((Object)(object)val3);
			throw;
		}
		int ReadIndex(int index)
		{
			if (width != 2)
			{
				return BitConverter.ToInt32(indexBytes, index * 4);
			}
			return BitConverter.ToUInt16(indexBytes, index * 2);
		}
	}

	private static Il2CppStructArray<byte> ReadBuffer(GraphicsBuffer buffer)
	{
		try
		{
			Il2CppStructArray<byte> il2CppStructArray = new Il2CppStructArray<byte>(checked(buffer.count * buffer.stride));
			buffer.GetData(((Il2CppObjectBase)il2CppStructArray).Cast<Array>());
			return il2CppStructArray;
		}
		finally
		{
			buffer.Dispose();
		}
	}

	internal void Restore()
	{
		foreach (Patch patch in patches)
		{
			if ((Object)(object)patch.filter != (Object)null && (Object)(object)patch.original != (Object)null && (Object)(object)patch.filter.sharedMesh == (Object)(object)patch.replacement)
			{
				patch.filter.sharedMesh = patch.original;
			}
		}
	}

	internal void Dispose()
	{
		Restore();
		foreach (Patch patch in patches)
		{
			if ((Object)(object)patch.replacement != (Object)null)
			{
				Object.Destroy((Object)(object)patch.replacement);
			}
		}
		patches.Clear();
		checkedMeshes.Clear();
		body = null;
		nextSearch = 0f;
	}
}
