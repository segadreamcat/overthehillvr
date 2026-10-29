namespace Valve.VR;

public enum EVRSubmitFlags
{
	Submit_Default = 0,
	Submit_LensDistortionAlreadyApplied = 1,
	Submit_GlRenderBuffer = 2,
	Submit_Reserved = 4,
	Submit_TextureWithPose = 8,
	Submit_TextureWithDepth = 16,
	Submit_FrameDiscontinuity = 32,
	Submit_VulkanTextureWithArrayData = 64,
	Submit_GlArrayTexture = 128,
	Submit_IsEgl = 256,
	Submit_TextureWithMotion = 536,
	Submit_Reserved2 = 32768,
	Submit_Reserved3 = 65536,
	Submit_Reserved4 = 131072,
	Submit_Reserved5 = 262144,
	Submit_Reserved6 = 524288
}
