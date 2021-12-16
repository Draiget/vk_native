using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.types;

internal class SpecUnion
{
    public string Name { get; set; }

    public List<SpecUnionMember> Members { get; set; }

    public string Comment { get; set; }

    public SpecUnion() {
        Members = new List<SpecUnionMember>();
    }
}
