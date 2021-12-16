using vk.gen.spec.utils;

namespace vk.gen.spec.types;

internal class SpecEnum : ISpec
{
    public string Name { get; set; }

    public string SpecName { get; set; }

    public List<SpecEnumValue> Values { get; }

    public bool IsBitMask { get; set; }

    public SpecEnum() {
        Values = new List<SpecEnumValue>();
    }
}
