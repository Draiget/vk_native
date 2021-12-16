using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.types;

internal class SpecConstant
{
    public string Name { get; set; }

    public Type Type { get; set; }

    public string Value { get; set; }

    public string SpecName { get; set; }

    public string Alias { get; set; }

    public string Comment { get; set; }
}
