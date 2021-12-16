using System.Xml.Linq;
using vk.gen.spec.types;

namespace vk.gen.spec.consumers;

internal static class StructureSpecConsumer
{
    public static List<SpecStructure> From(List<XElement> structures) {
        var res = new List<SpecStructure>();

        foreach (var e in structures) {
            ConsumeStructureClass(e, res);
        }

        return res;
    }

    private static void ConsumeStructureClass(XElement e, List<SpecStructure> res) {
        
    }
}
