﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace CodeOfChaos.CliArgsParser.Generators.Content.CommandGenerator;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValueProvider<ImmutableArray<ClassDto>> parameterStructs = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsCommandClass,
                GatherCommandClass
            ).Collect();

        context.RegisterSourceOutput(context.CompilationProvider.Combine(parameterStructs), GenerateSources);
    }

    private static bool IsCommandClass(SyntaxNode node, CancellationToken _) {
        if (node is not ClassDeclarationSyntax { BaseList: { Types.Count: > 0 } baseList, AttributeLists: {Count: > 0 } attributeLists}) return false;
        
        if (!attributeLists.SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString().Contains("CliArgsCommand"))) {
            return false;
        }

        return baseList.Types.Any(
            type => type.Type is GenericNameSyntax genericNameSyntax
                && genericNameSyntax.Identifier.ValueText.Contains("ICommand")
        );

    }

    private static ClassDto GatherCommandClass(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        return ClassDto.FromSyntax(context, (ClassDeclarationSyntax)context.Node);
    }
    
    private static void GenerateSources(SourceProductionContext context, (Compilation compilation, ImmutableArray<ClassDto> data) source) {
        GeneratorStringBuilder builder = new();
        
        foreach (ClassDto dto in source.data) {
            // We can only report diagnostics in this context, so do it here!
            dto.ReportDiagnostics(context);
            
            // After which we build the string to our best capabilities, which might result in errors
            //      These errors should resolve themselves if the user writes the correct data on their end.
            builder
                .AppendAutoGenerated()
                .AppendUsings("CodeOfChaos.CliArgsParser")
                .AppendLine($"namespace {dto.Namespace};")
                .AppendLine("#nullable enable")
                .AppendLine($"public partial class {dto.ToDeclarationName()} {{");

            builder
                .AppendLine("}");
            
            context.AddSource($"{dto.ClassName}.g.cs", builder.ToStringAndClear());
        }        
    }
}
