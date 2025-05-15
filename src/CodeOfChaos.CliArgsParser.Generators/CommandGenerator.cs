// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Threading;

namespace CodeOfChaos.CliArgsParser.Generators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class CommandGenerator : IIncrementalGenerator  {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValueProvider<ImmutableArray<CommandDto>> data = context.SyntaxProvider.CreateSyntaxProvider(
            Predicate,
            Transform
        )
        .Where(dto => dto is not null)
        .Collect()!;
        
        context.RegisterSourceOutput(context.CompilationProvider.Combine(data), GenerateSources);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken ct) {
        return node is ClassDeclarationSyntax or RecordDeclarationSyntax;
    }

    private static CommandDto? Transform(GeneratorSyntaxContext context, CancellationToken ct) {
        SemanticModel semanticModel = context.SemanticModel;

        INamedTypeSymbol? symbol = context.Node switch {
            ClassDeclarationSyntax classDeclaration => semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol,
            RecordDeclarationSyntax recordDeclaration => semanticModel.GetDeclaredSymbol(recordDeclaration) as INamedTypeSymbol,
            _ => null
        };
        if (symbol is null) return null;
        if (!symbol.HasInterfaceWithDisplayName(TypeNames.ICliCommandInterface)) return null;
        return CommandDto.TryCreate(symbol);

    }
    
    private static void GenerateSources(SourceProductionContext context, (Compilation Compilation, ImmutableArray<CommandDto> Data) Box) {
        Compilation compilation = Box.Compilation;
        ImmutableArray<CommandDto> data = Box.Data;

        foreach (CommandDto dto in data) {
            ParameterDto[] parameters = dto.Parameters;
        }
    }
    
}
