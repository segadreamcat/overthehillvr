namespace Valve.VR;

public enum VROverlayFlags
{
	NoDashboardTab = 8,
	SendVRDiscreteScrollEvents = 0x40,
	SendVRTouchpadEvents = 0x80,
	ShowTouchPadScrollWheel = 0x100,
	TransferOwnershipToInternalProcess = 0x200,
	SideBySide_Parallel = 0x400,
	SideBySide_Crossed = 0x800,
	Panorama = 0x1000,
	StereoPanorama = 0x2000,
	SortWithNonSceneOverlays = 0x4000,
	VisibleInDashboard = 0x8000,
	MakeOverlaysInteractiveIfVisible = 0x10000,
	SendVRSmoothScrollEvents = 0x20000,
	ProtectedContent = 0x40000,
	HideLaserIntersection = 0x80000,
	WantsModalBehavior = 0x100000,
	IsPremultiplied = 0x200000,
	IgnoreTextureAlpha = 0x400000,
	EnableControlBar = 0x800000,
	EnableControlBarKeyboard = 0x1000000,
	EnableControlBarClose = 0x2000000,
	MinimalControlBar = 0x4000000,
	EnableClickStabilization = 0x8000000,
	MultiCursor = 0x10000000,
	NoBackside = 0x20000000
}
