using System;
using System.Runtime.InteropServices;

namespace Valve.VR;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct RenderModel_TextureMap_t_Packed(RenderModel_TextureMap_t unpacked)
{
	public ushort unWidth = unpacked.unWidth;

	public ushort unHeight = unpacked.unHeight;

	public IntPtr rubTextureMapData = unpacked.rubTextureMapData;

	public EVRRenderModelTextureFormat format = unpacked.format;

	public ushort unMipLevels = unpacked.unMipLevels;

	public void Unpack(ref RenderModel_TextureMap_t unpacked)
	{
		unpacked.unWidth = unWidth;
		unpacked.unHeight = unHeight;
		unpacked.rubTextureMapData = rubTextureMapData;
		unpacked.format = format;
		unpacked.unMipLevels = unMipLevels;
	}
}
