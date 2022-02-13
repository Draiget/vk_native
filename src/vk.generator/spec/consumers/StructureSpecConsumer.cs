using System.Text.RegularExpressions;
using System.Xml.Linq;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.consumers;

internal static class StructureSpecConsumer
{
    private static readonly Regex StructureMemberCountRegex = new(@"\[(?<count>[0-9]+)\]", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

    public static List<SpecStructure> From(List<XElement> elements, List<SpecConstant> constants, List<SpecType> types) {
        var res = new List<SpecStructure>();

        foreach (var e in elements.Where(x => x.Attribute("category")?.Value == "struct")) {
            ConsumeStructureClass(e, res, constants, types);
        }

        return res;
    }

    private static void ConsumeStructureClass(XElement e, List<SpecStructure> res, List<SpecConstant> constants, List<SpecType> types) {
        var elementName = e.Attribute("name");
        var elementReturnedOnly = e.Attribute("returnedonly");

        if (elementName == null) {
            return;
        }

        var spec = new SpecStructure {
            Name = elementName.Value,
            IsReturnUsageOnly = elementReturnedOnly?.Value.Contains("true") ?? false
        };

        foreach (var x in e.Elements("member")) {
            ConsumeStructureClassMember(x, spec, constants, types);
        }

        res.Add(spec);
    }

    private static void ConsumeStructureClassMember(XElement e, SpecStructure spec, List<SpecConstant> constants, List<SpecType> types) {
        var memberOptional = e.Attribute("optional");
        var memberNoAutoValidity = e.Attribute("noautovalidity");
        var memberValues = e.Attribute("values");
        var memberLen = e.Attribute("len");

        var memberType = e.Element("type");
        var memberName = e.Element("name");
        var memberComment = e.Element("comment");
        var memberEnumReference= e.Element("enum");

        if (memberName == null || memberType == null) {
            return;
        }

        var elementCount = 0u;
        if (memberEnumReference != null) {
            var referencedEnum = constants.FirstOrDefault(x => x.SpecName == memberEnumReference.Value);
            if (referencedEnum != null && e.Value == $"[{referencedEnum.SpecName}]") {
                elementCount = (uint)Convert.ChangeType(Convert.ChangeType(referencedEnum.Value, referencedEnum.Type), typeof(uint));
            }
        } else {
            var contentCountMatch = StructureMemberCountRegex.Match(e.Value);
            if (contentCountMatch.Success) {
                elementCount = uint.Parse(contentCountMatch.Groups["count"].Value);
            }
        }

        var member = new SpecStructureMember {
            Name = memberName.Value,
            Comment = memberComment?.Value,
            TypeName = memberType.Value,
            IsOptional = memberOptional?.Value.Contains("true") ?? false,
            IsNoAutoValidity = memberNoAutoValidity?.Value.Contains("true") ?? false,
            Count = elementCount,
            PointerLevel = (uint)e.Value.Count(x => x.Equals('*'))
        };

        var referencedBaseType = types.FirstOrDefault(x => x.Name == member.TypeName || x.Aliases.Any(alias => alias == x.Name));
        if (referencedBaseType != null) {
            if (referencedBaseType.IsBaseType && referencedBaseType.BaseTypeName != null) {
                member.TypeName = referencedBaseType.BaseTypeName.ToSharpTypeShort();
            } else {
                if (referencedBaseType.TypeName != null) {
                    member.TypeName = referencedBaseType.TypeName;
                }
            }
        }

        if (memberValues != null) {
            foreach (var memberValue in memberValues.Value.Split(',')) {
                var referencedConstant = constants.FirstOrDefault(x => x.SpecName == memberValue);
                if (referencedConstant != null) {
                    member.Values.Add(referencedConstant);
                }
            }
        }

        spec.Members.Add(member);
    }
}
