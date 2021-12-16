using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using vk.gen.spec;
using vk.gen.spec.types;
using vk.gen.spec.utils;

namespace vk.gen;

[Generator]
public class VulkanSpecGenerator : ISourceGenerator
{
    private VulkanSpecifications _specifications;

    private static readonly Dictionary<string, string> ResourceFiles = new() {
        { "resources", "resources.txt" },
        { "descriptorsets", "descriptorsets.txt" },
        { "queries", "queries.txt" },
        { "pipelines", "pipelines.txt" },
        { "samplers", "samplers.txt" },
        { "fragops", "fragops.txt" },
        { "cmdbuffers", "cmdbuffers.txt" },
    };

    public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
        if (!Debugger.IsAttached) {
            Debugger.Launch();
        }
#endif
    }

    public void Execute(GeneratorExecutionContext context) {
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectDirectory", out var projectDirectory) == false) {
            throw new ArgumentException("MSBuildProjectDirectory");
        }

        Debug.WriteLine("Reading Vulkan specification file");
        var vulkanSpecs = context.AdditionalFiles.First(e => e.Path.EndsWith("vk.xml")).GetText(context.CancellationToken);
        if (vulkanSpecs == null) {
            throw new ArgumentException("Unable to read vk.xml, content is null");
        }

        var chaptersResources = new Dictionary<string, string>();
        foreach (var res in ResourceFiles) {
            var resourcesDoc = context.AdditionalFiles.First(e => e.Path.EndsWith(res.Value)).GetText(context.CancellationToken);
            if (resourcesDoc == null) {
                throw new ArgumentException($"Unable to read {res.Value}, content is null");
            }

            chaptersResources[res.Key] = resourcesDoc.ToString();
        }

        var document = XDocument.Parse(vulkanSpecs.ToString(), LoadOptions.None);
        var root = document.Element("registry");
        if (root == null) {
            throw new ArgumentException("Invalid vk.xml, root token 'registry' is not found in document");
        }

        _specifications = VulkanSpecifications.GenerateFromXml(
            context, 
            root,
            chaptersResources);

        _specifications.GenerateCode();
    }
}