using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct RenderModel_t_Packed(RenderModel_t unpacked)
{
	public IntPtr rVertexData = unpacked.rVertexData;

	public uint unVertexCount = unpacked.unVertexCount;

	public IntPtr rIndexData = unpacked.rIndexData;

	public uint unTriangleCount = unpacked.unTriangleCount;

	public int diffuseTextureId = unpacked.diffuseTextureId;

	public void Unpack(ref RenderModel_t unpacked)
	{
		unpacked.rVertexData = rVertexData;
		unpacked.unVertexCount = unVertexCount;
		unpacked.rIndexData = rIndexData;
		unpacked.unTriangleCount = unTriangleCount;
		unpacked.diffuseTextureId = diffuseTextureId;
	}
}
