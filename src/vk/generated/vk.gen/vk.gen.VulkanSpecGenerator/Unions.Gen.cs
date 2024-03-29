﻿// This file is automatically generated, do not edit
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable CheckNamespace
// ReSharper disable PartialTypeWithSinglePart

using System.Runtime.InteropServices;
using vk.builtin.types;

namespace vk;

/// <summary>
/// Union allowing specification of floating point, integer, or unsigned integer color data. Actual value selected is based on image/attachment being cleared.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct VkClearColorValue
{
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public float[] float32;
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public int[] int32;
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public uint[] uint32;
}

/// <summary>
/// Union allowing specification of color or depth and stencil values. Actual value selected is based on attachment being cleared.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct VkClearValue
{
	[FieldOffset(0)]
	public VkClearColorValue color;
	[FieldOffset(0)]
	public VkClearDepthStencilValue depthStencil;
}

/// <summary>
/// Union of all the possible return types a counter result could return
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct VkPerformanceCounterResultKHR
{
	[FieldOffset(0)]
	public int int32;
	[FieldOffset(0)]
	public long int64;
	[FieldOffset(0)]
	public uint uint32;
	[FieldOffset(0)]
	public ulong uint64;
	[FieldOffset(0)]
	public float float32;
	[FieldOffset(0)]
	public double float64;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkPerformanceValueDataINTEL
{
	[FieldOffset(0)]
	public uint value32;
	[FieldOffset(0)]
	public ulong value64;
	[FieldOffset(0)]
	public float valueFloat;
	[FieldOffset(0)]
	public VkBool32 valueBool;
	[FieldOffset(0)]
	public IntPtr valueString;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkPipelineExecutableStatisticValueKHR
{
	[FieldOffset(0)]
	public VkBool32 b32;
	[FieldOffset(0)]
	public long i64;
	[FieldOffset(0)]
	public ulong u64;
	[FieldOffset(0)]
	public double f64;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkDeviceOrHostAddressKHR
{
	[FieldOffset(0)]
	public VkDeviceAddress deviceAddress;
	[FieldOffset(0)]
	public IntPtr hostAddress;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkDeviceOrHostAddressConstKHR
{
	[FieldOffset(0)]
	public VkDeviceAddress deviceAddress;
	[FieldOffset(0)]
	public IntPtr hostAddress;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkAccelerationStructureGeometryDataKHR
{
	[FieldOffset(0)]
	public VkAccelerationStructureGeometryTrianglesDataKHR triangles;
	[FieldOffset(0)]
	public VkAccelerationStructureGeometryAabbsDataKHR aabbs;
	[FieldOffset(0)]
	public VkAccelerationStructureGeometryInstancesDataKHR instances;
}

[StructLayout(LayoutKind.Explicit)]
public partial struct VkAccelerationStructureMotionInstanceDataNV
{
	[FieldOffset(0)]
	public VkAccelerationStructureInstanceKHR staticInstance;
	[FieldOffset(0)]
	public VkAccelerationStructureMatrixMotionInstanceNV matrixMotionInstance;
	[FieldOffset(0)]
	public VkAccelerationStructureSRTMotionInstanceNV srtMotionInstance;
}

