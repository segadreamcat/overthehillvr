namespace Valve.VR;

public enum VROverlayTransformType
{
	VROverlayTransform_Invalid = -1,
	VROverlayTransform_Absolute = 0,
	VROverlayTransform_TrackedDeviceRelative = 1,
	VROverlayTransform_TrackedComponent = 3,
	VROverlayTransform_Cursor = 4,
	VROverlayTransform_DashboardTab = 5,
	VROverlayTransform_DashboardThumb = 6,
	VROverlayTransform_Mountable = 7,
	VROverlayTransform_Projection = 8,
	VROverlayTransform_Subview = 9
}
