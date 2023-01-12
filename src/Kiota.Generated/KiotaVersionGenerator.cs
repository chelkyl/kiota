using System;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;

[Generator]
public class KiotaVersionGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var mainSyntaxTree = context.Compilation.SyntaxTrees
                            .First(static x => x.HasCompilationUnitRoot);

        var directory = Path.GetDirectoryName(mainSyntaxTree.FilePath);

        var version = "unknown";
        try {
            XmlDocument csproj = new XmlDocument();
            csproj.Load(Path.Join(directory, "Kiota.Builder.csproj"));

            version = csproj.GetElementsByTagName("Version")[0].InnerText;
        } catch (Exception e)
        {
            throw new SystemException("KiotaVersionGenerator expanded in an invalid project, missing 'Kiota.Builder.csproj' file.", e);
        }

        string source = $@"// <auto-generated/>
namespace Kiota.Generated
{{
    public static class KiotaVersion
    {{
        public static string Current()
        {{
            return ""{version}"";
        }}
    }}
}}
";

        // Add the source code to the compilation
        context.AddSource($"KiotaVersion.g.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}
