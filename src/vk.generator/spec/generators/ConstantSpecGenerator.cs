using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.generators;

internal class ConstantSpecGenerator
{
    private static void ProcessHeaderGeneric(VulkanSpecCodeWriter cw) {
        cw.UseDefaultHeaderComment();
        cw.UseSingleComment("ReSharper disable InconsistentNaming");
        cw.UseSingleComment("ReSharper disable IdentifierTypo");
        cw.UseSingleComment("ReSharper disable UnusedMember.Global");
        cw.UseSingleComment("ReSharper disable CommentTypo");
        cw.UseSingleComment("ReSharper disable CheckNamespace");
        cw.UseNamespace("vk");
    }

    public static void Process(List<SpecConstant> constants, VulkanSpecCodeWriter cw) {
        ProcessConstants(constants, cw);
    }

    private static void ProcessConstants(List<SpecConstant> constants, VulkanSpecCodeWriter cw) {
        ProcessHeaderGeneric(cw);
        cw.WriteLine($"public static partial class {GeneratorConstants.SharedClassName}\n{{");

        foreach (var spec in constants) {
            ProcessConstantEntry(spec, cw);
        }

        cw.WriteLine("}\n");
        cw.FinalizeSourceFile("Constants");
    }

    private static void ProcessConstantEntry(SpecConstant spec, VulkanSpecCodeWriter cw) {
        if (spec.Value == null || spec.Name == null) {
            return;
        }

        cw.BeginBlock();
        if (!string.IsNullOrWhiteSpace(spec.Comment)) {
            cw.BeginComment();
            cw.UseMultiComment(spec.Comment);
            cw.EndComment();
        }
        cw.WriteLine($"public const {spec.Type.ToShortName()} {spec.Name} = {spec.Value};");
        cw.EndBlock();
    }
}
