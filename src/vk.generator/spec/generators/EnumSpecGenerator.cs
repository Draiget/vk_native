using vk.gen.spec.types;

namespace vk.gen.spec.generators;

internal static class EnumSpecGenerator
{
    public static void Process(List<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        ProcessEnums(enums, cw);
        ProcessEnumRawConstants(enums, cw);
    }

    private static void ProcessHeaderGeneric(VulkanSpecCodeWriter cw) {
        cw.UseDefaultHeaderComment();
        cw.UseSingleComment("ReSharper disable InconsistentNaming");
        cw.UseSingleComment("ReSharper disable IdentifierTypo");
        cw.UseSingleComment("ReSharper disable UnusedMember.Global");
        cw.UseSingleComment("ReSharper disable CommentTypo");
        cw.UseSingleComment("ReSharper disable CheckNamespace");
        cw.UseNamespace("vk");
    }

    private static void ProcessEnumRawConstants(List<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        ProcessHeaderGeneric(cw);
        cw.WriteLine("public static class EnumsRaw\n{");

        foreach (var specEnum in enums) {
            ProcessEnumConstantsClasses(specEnum, cw);
        }

        cw.WriteLine("}\n");
        cw.FinalizeSourceFile("Enums.Raw");
    }

    private static void ProcessEnumConstantsClasses(SpecEnum e, VulkanSpecCodeWriter cw) {
        foreach (var specValue in e.Values) {
            if (specValue.Name == "None" && string.IsNullOrWhiteSpace(specValue.SpecName)) {
                continue;
            }

            cw.BeginBlock();
            cw.WriteLine($"public const {e.Name} {specValue.SpecName} = {e.Name}.{specValue.Name};");
            cw.EndBlock();
        }
    }

    private static void ProcessEnums(List<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        ProcessHeaderGeneric(cw);

        foreach (var specEnum in enums) {
            ProcessEnumClasses(specEnum, cw);
        }

        cw.FinalizeSourceFile("Enums");
    }

    private static void ProcessEnumClasses(SpecEnum e, VulkanSpecCodeWriter cw) {
        if (e.IsBitMask) {
            cw.WriteLine("[Flags]");
        }

        cw.WriteLine($"public enum {e.Name}\n{{");

        foreach (var specValue in e.Values) {
            cw.BeginBlock();
            ProcessEnumClassValue(specValue, e, cw);
            cw.EndBlock();
        }

        cw.WriteLine("}\n");
    }

    private static void ProcessEnumClassValue(SpecEnumValue e, SpecEnum parent, VulkanSpecCodeWriter cw) {
        if (!string.IsNullOrWhiteSpace(e.Comment)) {
            cw.BeginComment();
            cw.UseMultiComment(e.Comment);
            cw.EndComment();
        }

        cw.WriteLine($"{e.Name} = {e.Value},");
    }
}
