using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.utils;

internal static class StringExtensions
{
    private static readonly Dictionary<string, string> KnownToConvert = new() {
        { "uint", "UInt" },
        { "sint", "SInt" },
        { "ushort", "UShort" },
        { "ulong", "ULong" },
        { "uscaled", "UScaled" },
        { "sscaled", "SScaled" },
        { "unorm", "UNorm" },
        { "snorm", "SNorm" },
        { "srgb", "SRgb" },
        { "sfloat", "SFloat" },
        { "ufloat", "UFloat" },
        { "initialized", "Initialized" },
        { "coreavi", "CoreAvi" },
        { "moltenvk", "MoltenVk" },
        { "swiftshader", "SwiftShader" },
        { "framebuffer", "FrameBuffer" },
        { "frontface", "FrontFace" }
    };

    public static string ToTitleCase(this string self, char separator = '_', bool trim = true) {
        var c = new StringBuilder(self.Length);
        var selfArray = self.ToLowerInvariant().Split(separator);
        for (var i = 0; i < selfArray.Length; i++) {
            var selfPart = selfArray[i];
            foreach (var replace in KnownToConvert.Keys.Where(r => selfPart.Contains(r))) {
                selfArray[i] = selfPart.Replace(replace, KnownToConvert[replace]);
            }

            if (selfPart.Length > 0) {
                c.Append(char.ToUpperInvariant(selfArray[i][0]));
                c.Append(selfArray[i].Substring(1, selfPart.Length - 1));
            }
        }

        return trim ? c.ToString().Replace(" ", "") : c.ToString();
    }

    private static readonly Dictionary<string, Type> VulkanToLocalTypeMap = new() {
        { "uint16_t", typeof(ushort) },
        { "uint32_t", typeof(uint) },
        { "uint64_t", typeof(ulong) },
        { "int16_t", typeof(short) },
        { "int32_t", typeof(int) },
        { "int64_t", typeof(long) },
        { "float", typeof(float) },
        { "double", typeof(double) }
    };

    private static readonly Dictionary<Type, string> LocalTypeToShortNameMap = new() {
        { typeof(ushort), "ushort" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(float), "float" },
        { typeof(double), "double" }
    };

    public static Type ToSharpType(this string type) {
        if (VulkanToLocalTypeMap.ContainsKey(type)) {
            return VulkanToLocalTypeMap[type];
        }

        return Type.GetType(type);
    }

    public static string ToShortName(this Type self) {
        if (self == null) {
            return null;
        }

        if (LocalTypeToShortNameMap.ContainsKey(self)) {
            return LocalTypeToShortNameMap[self];
        }

        return typeof(void).Name;
    }
}
