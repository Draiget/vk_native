using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.types;

internal class SpecUnionMember
{
    public string Name { get; set; }

    public int Offset { get; set; }

    public string Type { get; set; }

    public int Count { get; set; }

    public bool NoAutoValidity { get; set; }

    public bool IsPointer { get; set; }

    public SpecUnionMember() {
        Count = 1;
    }
}
