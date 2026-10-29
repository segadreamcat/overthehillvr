using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR;

public struct IVRSpatialAnchors
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRSpatialAnchorError _CreateSpatialAnchorFromDescriptor(IntPtr pchDescriptor, ref uint pHandleOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRSpatialAnchorError _CreateSpatialAnchorFromPose(uint unDeviceIndex, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPose, ref uint pHandleOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRSpatialAnchorError _GetSpatialAnchorPose(uint unHandle, ETrackingUniverseOrigin eOrigin, ref SpatialAnchorPose_t pPoseOut);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	internal delegate EVRSpatialAnchorError _GetSpatialAnchorDescriptor(uint unHandle, StringBuilder pchDescriptorOut, ref uint punDescriptorBufferLenInOut);

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CreateSpatialAnchorFromDescriptor CreateSpatialAnchorFromDescriptor;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _CreateSpatialAnchorFromPose CreateSpatialAnchorFromPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSpatialAnchorPose GetSpatialAnchorPose;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	internal _GetSpatialAnchorDescriptor GetSpatialAnchorDescriptor;
}
