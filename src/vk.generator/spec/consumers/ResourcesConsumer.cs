using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using vk.gen.spec.resources;
using vk.gen.spec.types;

namespace vk.gen.spec.consumers;

internal static class ResourcesConsumer
{
    private static readonly Regex SpecDefinitionBlock = new(@"\[open,\s?refpage='(?<refpage>[a-zA-Z0-9]+)',\s?desc='(?<desc>[a-zA-Z\s0-9]+)',\s?type='(?<type>[a-zA-Z]+)'\](\r|\n)+^(?<block>--)", RegexOptions.Multiline);
    private static readonly Regex SpecEndBlock = new(@"(^--\r\n^\r\n)", RegexOptions.Multiline);
    private static readonly Regex SpecEnumEntry = new(@"^\s{2}\*\sename:(?<enum>[A-Z_]+)(?<content>([a-zA-Z':\-_\(\)\<\>\.,\/\s])+)$", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
    private static readonly Regex SpecConditional = new(@"^ifdef::(?<condition>[a-zA-Z0-9_,]+)\[\]\r\n(?<content>.*?)\r\n^endif(.*?)$", RegexOptions.Multiline | RegexOptions.Singleline);

    public static List<ResourceEnumRefPage> Process(string content, List<SpecEnum> enums) {
        var res = new List<ResourceEnumRefPage>();

        var matches = SpecDefinitionBlock.Matches(content);
        foreach (Match m in matches) {
            var type = m.Groups["type"].Value;
            if (type != "enums") {
                continue;
            }

            var page = new ResourceEnumRefPage {
                ReferenceToType = m.Groups["refpage"].Value,
                Description = m.Groups["desc"].Value
            };

            var refEnum = enums.FirstOrDefault(x => x.SpecName == page.ReferenceToType);
            if (refEnum == null) {
                continue;
            }

            var blockContent = content.Substring(m.Groups["block"].Index + m.Groups["block"].Length);
            var blockEndMatch = SpecEndBlock.Match(blockContent);

            blockContent = blockContent.Substring(0, blockEndMatch.Index);
            var contentParts = blockContent.Split(new []{ "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var contentPart in contentParts) {
                if (!contentPart.Contains("ename")) {
                    continue;
                }

                ProcessRefPage(page, contentPart, refEnum, enums);
                res.Add(page);
            }
        }

        return res;
    }

    private static readonly Regex ItalicRegex = new(@"\s_(?<text>[a-zA-Z0-9\s]+)_");
    private static readonly Regex BoldRegex = new(@"(\s:)?(?<text>[a-zA-Z]+):\s");
    private static readonly Regex SeeOther = new(@"<<(?<refname>[a-z-]+),\s?ename:(?<enum>[A-Z0-9_]+)>>");
    private static readonly Regex SectionReference = new(@"<<(?<ref>[a-zA-Z-]+),(?<text>.*)>>");
    private static readonly Regex EnumReference = new(@"ename:(?<enum>[A-Z0-9_]+)");

    private static void ProcessRefPage(ResourceEnumRefPage r, string content, SpecEnum e, List<SpecEnum> others) {
        // Remove conditions
        content = SpecConditional.Replace(content, "${content}");

        foreach (Match m in SpecEnumEntry.Matches(content)) {
            var enumName = m.Groups["enum"].Value;
            var enumObj = e.Values.FirstOrDefault(x => x.SpecName == enumName);
            if (enumObj == null) {
                continue;
            }

            if (e.Name == "VkBufferUsageFlags") {
                Debug.WriteLine("sdg");
            }

            var text = new string(m.Groups["content"].Value
                .Trim()
                .Replace("    ", " ")
                .Replace("\r", "")
                .Replace("\n", "")
                .Select((c, i) => i == 0 ? char.ToUpperInvariant(c) : c)
                .ToArray());

            text = ItalicRegex.Replace(text, " <i>${text}</i>");
            text = BoldRegex.Replace(text, "<b>${text}</b> ");
            text = SectionReference.Replace(text, "<seealso href=\"https://www.khronos.org/registry/vulkan/specs/1.2-extensions/html/vkspec.html#${ref}\">${text}</seealso>");

            foreach (Match enumRefMatch in EnumReference.Matches(text)) {
                var referencedEnumClass = others.FirstOrDefault(x => x.Values.Any(t => t.SpecName == enumRefMatch.Groups["enum"].Value));
                if (referencedEnumClass == null) {
                    continue;
                }

                var referencedEnumValue = referencedEnumClass.Values.FirstOrDefault(x => x.SpecName == enumRefMatch.Groups["enum"].Value);
                if (referencedEnumValue == null) {
                    continue;
                }

                text = text.Replace(enumRefMatch.Value, $"<see cref=\"{referencedEnumClass.Name}.{referencedEnumValue.Name}\">{referencedEnumClass.Name}.{referencedEnumValue.Name}</see>");
            }

            foreach (Match tm in SeeOther.Matches(text)) {
                var referencedEnumName = tm.Groups["enum"].Value;
                var referencedEnum = others
                    .Where(x => x.Values
                        .Any(v => v.SpecName == referencedEnumName))
                    .Select(x => new Tuple<string, string>(x.Name, x.Values
                        .FirstOrDefault(t =>
                            t.SpecName == referencedEnumName)?.Name))
                    .ToArray()
                    .FirstOrDefault();

                if (referencedEnum == null) {
                    text = text.Replace(tm.Value, referencedEnumName);
                    continue;
                }

                text = text.Replace(tm.Value, $"<see cref=\"{referencedEnum.Item1}.{referencedEnum.Item2}\">${{{referencedEnum.Item2}}}</see>");
            }

            enumObj.Comment = text;
        }
    }
}
