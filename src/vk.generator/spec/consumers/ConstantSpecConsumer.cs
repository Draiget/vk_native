using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.consumers;

internal static class ConstantSpecConsumer
{
    private static readonly Regex ConstantNameReplaceRegex = new("(\\(|\\)|LL)");

    public static List<SpecConstant> From(List<XElement> enums) {
        var res = new List<SpecConstant>();

        foreach (var e in enums.Where(x => x.Attribute("name")?.Value == "API Constants")) {
            ConsumeConstantClass(e, res);
        }

        foreach (var s in res) {
            if (s.Alias != null) {
                s.Value = res.FirstOrDefault(x => x.SpecName == s.Alias)?.Name;
            }
        }

        return res;
    }

    private static void ConsumeConstantClass(XElement e, ICollection<SpecConstant> res) {
        foreach (var ec in e.Elements("enum")) {
            ConsumeConstantEntry(ec, res);
        }
    }

    private static void ConsumeConstantEntry(XElement e, ICollection<SpecConstant> res) {
        var constName = e.Attribute("name");
        var constType = e.Attribute("type");
        var constComment = e.Attribute("comment");
        var constAlias = e.Attribute("alias");
        var constValue = e.Attribute("value");

        if (constName == null) {
            return;
        }

        var constantType = constType?.Value.ToSharpType();
        if (constantType == null) {
            return;
        }

        var constant = new SpecConstant {
            SpecName = constName.Value,
            Comment = constComment?.Value,
            Alias = constAlias?.Value,
            Name = constName.Value.Substring(2).ToTitleCase(),
            Type = constantType,
            Value = GetConstantValueFromStr(constValue?.Value)
        };

        res.Add(constant);
    }

    private static string GetConstantValueFromStr(string value) {
        if (value == null) {
            return null;
        }

        return ConstantNameReplaceRegex.Replace(value, "");
    }
}
