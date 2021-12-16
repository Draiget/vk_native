using System.Diagnostics;

#pragma warning disable CS8618

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
namespace vk;

public partial struct VkClearColorValue
{
    public VkClearColorValue(float r, float g, float b, float a = 1.0f) : this() {
        Debug.Assert(float32 != null, nameof(float32) + " != null");
        float32[0] = r;
        float32[1] = g;
        float32[2] = b;
        float32[3] = a;
    }

    public VkClearColorValue(int r, int g, int b, int a = 255) : this() {
        Debug.Assert(int32 != null, nameof(int32) + " != null");
        int32[0] = r;
        int32[1] = g;
        int32[2] = b;
        int32[3] = a;
    }

    public VkClearColorValue(uint r, uint g, uint b, uint a = 255) : this() {
        Debug.Assert(uint32 != null, nameof(uint32) + " != null");
        uint32[0] = r;
        uint32[1] = g;
        uint32[2] = b;
        uint32[3] = a;
    }
}
