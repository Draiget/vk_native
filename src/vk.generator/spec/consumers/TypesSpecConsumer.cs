using System.Xml.Linq;
using vk.gen.spec.types;

namespace vk.gen.spec.consumers;

internal class TypesSpecConsumer
{
    public static List<SpecType> From(List<XElement> types) {
        var res = new List<SpecType>();

        foreach (var e in types.Where(x => x.Attribute("category")?.Value == "basetype")) {
            ConsumeBaseTypes(e, res);
        }

        foreach (var e in types.Where(x => x.Attribute("category")?.Value == "bitmask")) {
            ConsumeBitMaskTypes(e, res);
        }

        return res;
    }

    private static void ConsumeBaseTypes(XElement e, List<SpecType> res) {
        if (e.Value.Contains("typedef")) {
            ConsumeBaseTypeTypedef(e, res);
        }
    }

    private static void ConsumeBaseTypeTypedef(XElement e, List<SpecType> res) {
        var elementName = e.Element("name");
        var elementType = e.Element("type");

        if (elementName == null || elementType == null) {
            return;
        }

        var ret = new SpecType {
            Name = elementName.Value,
            IsBaseType = true,
            BaseTypeName = elementType.Value,
        };

        res.Add(ret);
    }

    private static void ConsumeBitMaskTypes(XElement e, List<SpecType> res) {
        var elementName = e.Element("name");
        var elementType = e.Element("type");
        var attributeRequires = e.Attribute("requires");
        var attributeName = e.Attribute("name");
        var attributeAlias = e.Attribute("alias");

        if (attributeName != null && attributeAlias != null) {
            foreach (var refElement in res.Where(x => x.Name == attributeAlias.Value)) {
                refElement.Aliases.Add(attributeName.Value);
            }

            return;
        }

        var isBaseTypeReference = res.FirstOrDefault(x => x.IsBaseType && x.Name == elementType?.Value);
        if (isBaseTypeReference != null) {
            isBaseTypeReference.Aliases.Add(elementName?.Value);
            return;
        }

        var ret = new SpecType {
            Name = elementName?.Value,
            Requires = attributeRequires?.Value,
            TypeName = elementType?.Value
        };

        res.Add(ret);
    }
}
