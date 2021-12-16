using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.consumers;

internal class UnionSpecConsumer
{
    private static readonly Regex UnionMemberCountRegex = new(@"\[(?<count>[0-9]+)\]", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

    public static List<SpecUnion> From(List<XElement> types) {
        var res = new List<SpecUnion>();

        foreach (var e in types.Where(x => x.Attribute("category")?.Value == "union")) {
            ConsumeUnionClass(e, res);
        }

        return res;
    }

    private static void ConsumeUnionClass(XElement e, List<SpecUnion> res) {
        var elementName = e.Attribute("name");
        var elementComment = e.Attribute("comment");

        if (elementName == null) {
            return;
        }

        var union = new SpecUnion {
            Name = elementName.Value,
            Comment = elementComment?.Value.Replace("// ", "")
        };

        var index = 0;
        foreach (var x in e.Elements("member")) {
            ConsumeUnionMemberClass(x, union, index++);
        }

        res.Add(union);
    }

    private static void ConsumeUnionMemberClass(XElement e, SpecUnion union, int index) {
        var elementName = e.Element("name");
        var elementType = e.Element("type");
        var elementNoAutoValidity = e.Attribute("noautovalidity");
        var content = e.Value;

        if (elementName == null || elementType == null) {
            return;
        }

        var member = new SpecUnionMember {
            Name = elementName.Value,
            Type = elementType.Value.ToSharpType().ToShortName() ?? elementType.Value,
            Offset = index,
            NoAutoValidity = elementNoAutoValidity != null && bool.Parse(elementNoAutoValidity.Value),
            IsPointer = content.Contains("*")
        };

        var contentCountMatch = UnionMemberCountRegex.Match(content);
        if (contentCountMatch.Success) {
            var elementCount = int.Parse(contentCountMatch.Groups["count"].Value);
            member.Count = elementCount;
        }

        union.Members.Add(member);
    }
}