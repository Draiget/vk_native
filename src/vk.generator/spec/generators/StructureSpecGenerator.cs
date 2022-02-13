using System;
using System.Collections.Generic;
using System.Text;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen.spec.generators;

internal class StructureSpecGenerator
{
    private static void ProcessHeaderGeneric(VulkanSpecCodeWriter cw) {
        cw.UseDefaultHeaderComment();
        cw.UseSingleComment("ReSharper disable InconsistentNaming");
        cw.UseSingleComment("ReSharper disable IdentifierTypo");
        cw.UseSingleComment("ReSharper disable UnusedMember.Global");
        cw.UseSingleComment("ReSharper disable CommentTypo");
        cw.UseSingleComment("ReSharper disable CheckNamespace");
        cw.UseSingleComment("ReSharper disable PartialTypeWithSinglePart");
        cw.UseNamespace("vk");
        cw.WriteLine("using vk.builtin;");
        cw.WriteLine("using vk.builtin.types;");
        cw.Write("\n");
    }

    public static void Process(List<SpecStructure> structures, VulkanSpecCodeWriter cw) {
        ProcessStructures(structures, cw);
    }

    private static void ProcessStructures(List<SpecStructure> structures, VulkanSpecCodeWriter cw) {
        ProcessHeaderGeneric(cw);

        foreach (var spec in structures) {
            ProcessStructure(spec, cw);
        }

        cw.FinalizeSourceFile("Structures");
    }

    private static void ProcessStructure(SpecStructure spec, VulkanSpecCodeWriter cw) {
        if (spec.Name == null) {
            return;
        }

        cw.WriteLine($"public unsafe partial struct {spec.Name}");
        cw.WriteLine("{");
        cw.BeginBlock();

        foreach (var member in spec.Members) {
            ProcessStructureMember(member, spec, cw);
        }

        cw.Write("\n");
        cw.WriteLine($"public static {spec.Name} New() {{");
        cw.BeginBlock();
        cw.WriteLine($"var ret = new {spec.Name} {{");
        cw.BeginBlock();

        foreach (var member in spec.Members.Where(x => x.Values.Count > 0)) {
            var values = string.Join(" | ", member.Values.Select(x => $"{x.Type}.{x.Name}"));
            cw.WriteLine($"{member.Name} = {values};");
        }

        cw.EndBlock();
        cw.WriteLine("};");
        cw.WriteLine("return ret;");

        cw.EndBlock();
        cw.WriteLine("}");

        cw.EndBlock();
        cw.WriteLine("}\n");
    }

    private static void ProcessStructureMember(SpecStructureMember member, SpecStructure spec,VulkanSpecCodeWriter cw) {
        cw.WriteLine($"public {member.TypeName.ToSharpTypeShort()}{new string('*', (int)member.PointerLevel)} {member.Name};");
    }
}
