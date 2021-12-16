using System;
using System.Collections.Generic;
using System.Text;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.generators;

internal class UnionSpecGenerator
{
    private static void ProcessHeaderGeneric(VulkanSpecCodeWriter cw) {
        cw.UseDefaultHeaderComment();
        cw.UseSingleComment("ReSharper disable InconsistentNaming");
        cw.UseSingleComment("ReSharper disable IdentifierTypo");
        cw.UseSingleComment("ReSharper disable UnusedMember.Global");
        cw.UseSingleComment("ReSharper disable CommentTypo");
        cw.UseSingleComment("ReSharper disable CheckNamespace");
        cw.UseSingleComment("ReSharper disable PartialTypeWithSinglePart");
        cw.WriteLine("");
        cw.WriteLine("using System.Runtime.InteropServices;");
        cw.WriteLine("using vk.builtin.types;");
        cw.WriteLine("");
        cw.UseNamespace("vk");
    }

    public static void Process(List<SpecUnion> unions, VulkanSpecifications specs, VulkanSpecCodeWriter cw) {
        ProcessUnions(unions, specs.Enums, cw);
    }

    private static void ProcessUnions(List<SpecUnion> unions, IReadOnlyCollection<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        ProcessHeaderGeneric(cw);

        foreach (var union in unions) {
            ProcessUnionStructure(union, enums, cw);
        }

        cw.FinalizeSourceFile("Unions");
    }

    private static void ProcessUnionStructure(SpecUnion spec, IReadOnlyCollection<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        if (spec.Name == null) {
            return;
        }

        if (!string.IsNullOrWhiteSpace(spec.Comment)) {
            cw.BeginComment();
            cw.UseMultiComment(spec.Comment);
            cw.EndComment();
        }
        cw.WriteLine("[StructLayout(LayoutKind.Explicit)]");
        cw.WriteLine($"public partial struct {spec.Name}");
        cw.WriteLine("{");

        foreach (var member in spec.Members) {
            cw.BeginBlock();
            ProcessUnionStructureMember(member, enums, cw);
            cw.EndBlock();
        }

        cw.WriteLine("}\n");
    }

    private static void ProcessUnionStructureMember(SpecUnionMember spec, IEnumerable<SpecEnum> enums, VulkanSpecCodeWriter cw) {
        if (spec.Name == null) {
            return;
        }

        cw.WriteLine($"[FieldOffset(0)]");

        var targetType = spec.Type;
        if (!spec.IsPointer) {
            var typeLookup = enums.FirstOrDefault(x => x.SpecName == spec.Type);
            if (typeLookup != null) {
                targetType = typeLookup.Name;
            }
        } else {
            targetType = nameof(IntPtr);
        }

        if (spec.Count > 1) {
            cw.WriteLine($"[MarshalAs(UnmanagedType.ByValArray, SizeConst = {spec.Count})]");
            cw.WriteLine($"public {targetType}[] {spec.Name};");
        } else {
            cw.WriteLine($"public {targetType} {spec.Name};");
        }
    }
}