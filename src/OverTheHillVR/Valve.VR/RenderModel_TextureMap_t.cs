using System;

namespace Valve.VR;

public struct RenderModel_TextureMap_t
{
	public ushort unWidth;

	public ushort unHeight;

	public IntPtr rubTextureMapData;

	public EVRRenderModelTextureFormat format;

	public ushort unMipLevels;
}
