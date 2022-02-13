using System;
using System.Collections.Generic;
using System.Text;
using vk.gen.spec.utils;

namespace vk.gen.spec.types;

internal class SpecType : ISpec
{
    public string Name { get; set; }

    public bool IsBaseType { get; set; }

    public string BaseTypeName { get; set; }

    public List<string> Aliases { get; set; }

    public string Requires { get; set; }

    public string TypeName { get; set; }

    public SpecType() {
        Aliases = new List<string>();
    }
}
