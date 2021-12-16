using vk.gen.spec.utils;

namespace vk.gen.spec.types;

internal class SpecEnumValue : ISpec
{
    public string Name { get; set; }

    public string SpecName { get; set; }

    public string[] SpecNameParts { get; set; }

    public string Comment { get; set; }
    
    public string Value { get; set; }

    public string Alias { get; set; }

    public override string ToString() {
        return $"SpecEnumValue[SpecName = {SpecName}, Value = {Value}]";
    }
}
