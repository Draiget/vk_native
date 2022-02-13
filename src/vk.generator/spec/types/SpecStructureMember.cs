using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.types;

internal class SpecStructureMember
{
    public string Name { get; set; }

    public bool IsOptional { get; set; }

    public bool IsNoAutoValidity { get; set; }

    public uint PointerLevel { get; set; }

    public string TypeName { get; set; }

    public string Comment { get; set; }

    public uint Count { get; set; }

    public List<SpecConstant> Values { get; set; }

    public SpecStructureMember() {
        Values = new List<SpecConstant>();
    }
}
