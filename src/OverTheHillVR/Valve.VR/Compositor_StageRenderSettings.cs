using System.Runtime.InteropServices;

namespace Valve.VR;

public struct Compositor_StageRenderSettings
{
	public HmdColor_t m_PrimaryColor;

	public HmdColor_t m_SecondaryColor;

	public float m_flVignetteInnerRadius;

	public float m_flVignetteOuterRadius;

	public float m_flFresnelStrength;

	[MarshalAs(UnmanagedType.I1)]
	public bool m_bBackfaceCulling;

	[MarshalAs(UnmanagedType.I1)]
	public bool m_bGreyscale;

	[MarshalAs(UnmanagedType.I1)]
	public bool m_bWireframe;
}
