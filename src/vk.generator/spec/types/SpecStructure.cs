using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace vk.gen.spec.types;

internal class SpecStructure
{
    public string Name { get; set; }

    public Type Type { get; set; }

    public string TypeName { get; set; }

    /// <summary>
    /// Only applicable if "category" is struct or union. Notes that this struct/union
    /// is going to be filled in by the API, rather than an application
    /// filling it out and passing it to the API.
    /// </summary>
    public bool IsReturnUsageOnly { get; set; }


}
