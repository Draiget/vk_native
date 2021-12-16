using System.Diagnostics;
using System.Xml.Linq;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.consumers;

internal static class EnumSpecConsumer
{
    public static List<SpecEnum> From(List<XElement> enums, List<XElement> ext) {
        var res = new List<SpecEnum>();

        // Read from XML and create objects with raw data
        foreach (var e in enums) {
            ConsumeEnumClass(e, res);
        }

        // Read extensions from XML and update existing raw enums
        foreach (var extension in ext) {
            var number = extension.Attribute("number")?.Value;

            foreach (var re in extension.Elements("require")) {
                foreach (var enumExtension in re.Elements("enum")) {
                    ConsumeEnumExtension(enumExtension, number, res);
                }
            }
        }

        // Process raw object, rename, create aliases, etc
        foreach (var s in res) {
            UpdateEnumClass(s);
        }

        return res;
    }

    private static void UpdateEnumClass(SpecEnum s) {
        s.Name = TransformEnumClassOrValueName(s.SpecName);
        foreach (var rep in SpecNameExcludeList) {
            s.Name = s.Name.Replace(rep, "");
        }

        if (s.Values.Count == 0) {
            s.Values.Add(new SpecEnumValue {
                Name = "None",
                Value = "0"
            });
            return;
        }

        Debug.WriteLine($"Enum: {s.SpecName}");

        string[] commonParts;
        if (s.Values.Count == 1) {
            commonParts = ExtractCommonNameBySpecOnly(s);
        } else {
            commonParts = ExtractCommonNameByAllValues(s);
        }

        var toRemove = new List<SpecEnumValue>();
        foreach (var v in s.Values) {
            UpdateEnumClassValue(v, s, commonParts.Length);

            if (v.Alias != null) {
                var targetLink = s.Values.FirstOrDefault(x => x.SpecName == v.Alias);
                if (targetLink != null) {
                    v.Value = targetLink.Name;
                }

                if (s.Values.Any(x => x.SpecName == v.Alias)) {
                    toRemove.Add(v);
                }
            }
        }

        if (toRemove.Count < 1) {
            return;
        }

        s.Values.RemoveAll(x => toRemove.Contains(x));
    }

    private static void UpdateEnumClassValue(SpecEnumValue v, SpecEnum parent, int skipNameElementsCount) {
        var nameParts = v.SpecName.Split('_');
        var enumValueName = string.Join("_", nameParts.Skip(skipNameElementsCount));

        v.Name = enumValueName.ToTitleCase();
        if (string.IsNullOrWhiteSpace(v.Name)) {
            v.Name = "Default";
        } else {
            foreach (var s in EnumValueNameExcludeList) {
                var tmp = v.Name.Replace(s, "");
                if (!string.IsNullOrWhiteSpace(tmp)) {
                    v.Name = tmp;
                }
            }
        }

        Debug.WriteLine($"Field: {v.Name}");
    }

    private static readonly List<string> EnumValueNameExcludeList = new() {
        "Bit",
        "Nv"
    };

    private static void ConsumeEnumClass(XElement e, ICollection<SpecEnum> res) {
        var enumName = e.Attribute("name");
        var enumType = e.Attribute("type");

        if (enumName == null || enumType == null) {
            return;
        }

        var enumClass = new SpecEnum {
            SpecName = enumName.Value,
            IsBitMask = enumType.Value == "bitmask"
        };

        foreach (var element in e.Elements("enum")) {
            ConsumeEnumElement(element, enumClass);
        }

        res.Add(enumClass);
    }

    private static void ConsumeEnumElement(XElement e, SpecEnum parent) {
        var enumEntryName = e.Attribute("name");
        var enumEntryValue = e.Attribute("value");
        var enumBitMask = e.Attribute("bitpos");
        var enumCommentValue = e.Attribute("comment");
        var enumAlias = e.Attribute("alias");

        if (enumEntryName == null) {
            return;
        }

        var enumValue = new SpecEnumValue {
            SpecName = enumEntryName.Value,
            SpecNameParts = enumEntryName.Value.Split('_'),
            Comment = enumCommentValue?.Value,
            Value = enumBitMask?.Value ?? enumEntryValue?.Value,
            Alias = enumAlias?.Value,
        };

        parent.Values.Add(enumValue);
    }

    private static void ConsumeEnumExtension(XElement e, string extensionNumberRaw, List<SpecEnum> res) {
        var enumName = e.Attribute("name");
        var enumExtends = e.Attribute("extends");
        var enumOffset = e.Attribute("offset");
        var enumValue = e.Attribute("value");
        var enumDirection = e.Attribute("dir");
        var enumComment = e.Attribute("comment");

        if (enumExtends == null || enumName == null) {
            return;
        }

        var target = res.FirstOrDefault(x => x.SpecName == enumExtends.Value);
        if (target == null) {
            return;
        }

        if (target.Values
            .Any(
                x => x.SpecName == enumName.Value || 
                x.SpecName.Replace("_", "") == enumName.Value.Replace("_", ""))) 
        {
            return;
        }

        var extensionNumber = int.Parse(extensionNumberRaw);
        var enumExtension = new SpecEnumValue {
            SpecName = enumName.Value,
            SpecNameParts = enumName.Value.Split('_'),
            Comment = enumComment?.Value
        };

        if (enumOffset?.Value == null) {
            var enumBitPos = e.Attribute("bitpos");
            if (enumBitPos?.Value != null) {
                var shift = int.Parse(enumBitPos.Value);
                enumExtension.Value = (1 << shift).ToString();
            } else {
                enumExtension.Value = enumValue?.Value ?? "0";
            }
        } else {
            var offset = int.Parse(enumOffset.Value);
            var direction = 1;
            if (enumDirection?.Value == "-") {
                direction = -1;
            }

            var value = direction * (1000000000 + (extensionNumber - 1) * 1000 + offset);
            enumExtension.Value = value.ToString();
        }

        target.Values.Add(enumExtension);
    }


    private static readonly Dictionary<string, string> SpecNameConvertMap = new() {
        { "FlagBits", "Flags" },
        { "EXT", "" },
        { "KHR", "" }
    };

    private static readonly List<string> SpecNameExcludeList = new() {
        "BIT",
        "FLAGS"
    };

    private static string TransformEnumClassOrValueName(string name) {
        foreach (var e in SpecNameConvertMap.Where(x => name.Contains(x.Key))) {
            name = name.Replace(e.Key, e.Value);
        }

        return name;
    }

    private static string[] ExtractCommonNameBySpecOnly(SpecEnum s) {
        var name = TransformEnumClassOrValueName(s.SpecName);
        var parts = string.Join("",
                name.Select((x, t) =>
                        (char.IsUpper(x) && t + 1 < name.Length && !char.IsUpper(name[t + 1])) && t > 0 ? $"_{x}" : $"{x}".ToUpperInvariant())
                    .ToArray())
            .Split('_')
            .Where(x => !SpecNameExcludeList.Any(x.Contains))
            .ToArray();

        return parts;
    }

    private static string[] ExtractCommonNameByAllValues(SpecEnum s) {
        var longestEnumName = s.Values.OrderByDescending(x => x.SpecName.Length).First().SpecName;
        var longestEnumNameParts = longestEnumName.Split('_');
        var commonParts = longestEnumNameParts
            .TakeWhile((x, i) => s.Values
                .All(t =>
                    t.SpecName.StartsWith(string.Join("_", longestEnumNameParts.Take(i + 1))) &&
                    i + 1 < t.SpecNameParts.Length &&
                    !char.IsDigit(t.SpecNameParts[i + 1][0])
                )).ToArray();

        return commonParts;
    }
}
