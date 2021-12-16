using System;
using System.Collections.Generic;
using System.Text;

namespace vk.gen.spec.resources;

internal class ResourceEnumRefPage
{
    public string ReferenceToType { get; set; }

    public string Description { get; set; }

    public Dictionary<string, string> FieldDescription { get; set; }

    public ResourceEnumRefPage() {
        FieldDescription = new Dictionary<string, string>();
    }
}
