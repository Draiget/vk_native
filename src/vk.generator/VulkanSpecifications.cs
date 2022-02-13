using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using vk.gen.spec.consumers;
using vk.gen.spec.generators;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen;

internal class VulkanSpecifications
{
    private readonly VulkanSpecCodeWriter _cw;

    public List<SpecType> Types { get; private set; }
    public List<SpecEnum> Enums { get; private set; }
    public List<SpecConstant> Constants { get; private set; }
    public List<SpecUnion> Unions { get; private set; }
    public List<SpecStructure> Structures { get; private set; }

    public VulkanSpecifications(GeneratorExecutionContext context) {
        _cw = new VulkanSpecCodeWriter(context);

        Types = new List<SpecType>();
        Enums = new List<SpecEnum>();
        Constants = new List<SpecConstant>();
        Unions = new List<SpecUnion>();
        Structures = new List<SpecStructure>();
    }

    public static VulkanSpecifications GenerateFromXml(GeneratorExecutionContext context, XElement registry, Dictionary<string, string> chaptersResources) {
        var elementsEnum = registry
            .Elements("enums")
            .ToList();

        if (elementsEnum.Count == 0) {
            throw new ArgumentException("Invalid specifications xml, enums is not defined or empty");
        }

        var elementsExt = registry
            .Element("extensions")?
            .Elements("extension")
            .ToList();

        if (elementsExt == null || elementsExt.Count == 0) {
            throw new ArgumentException("Invalid specifications xml, enums is not defined or empty");
        }

        var elementsTypes = registry
            .Element("types")?
            .Elements("type")
            .ToList();

        if (elementsTypes == null || elementsTypes.Count == 0) {
            throw new ArgumentException("Invalid specifications xml, types is not defined or empty");
        }

        var enums = EnumSpecConsumer.From(elementsEnum, elementsExt);
        var constants = ConstantSpecConsumer.From(elementsEnum);
        var types = TypesSpecConsumer.From(elementsTypes);

        var specs = new VulkanSpecifications(context) {
            Types = types,
            Enums = enums,
            Constants = constants,
            Unions = UnionSpecConsumer.From(elementsTypes),
            Structures = StructureSpecConsumer.From(elementsTypes, constants, types)
        };

        ResourcesConsumer.Process(chaptersResources["resources"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["descriptorsets"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["queries"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["pipelines"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["samplers"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["fragops"], specs.Enums);
        ResourcesConsumer.Process(chaptersResources["cmdbuffers"], specs.Enums);
        return specs;
    }

    public void GenerateCode() {
        EnumSpecGenerator.Process(Enums, _cw);
        ConstantSpecGenerator.Process(Constants, _cw);
        UnionSpecGenerator.Process(Unions, this, _cw);
        StructureSpecGenerator.Process(Structures, _cw);
    }
}
