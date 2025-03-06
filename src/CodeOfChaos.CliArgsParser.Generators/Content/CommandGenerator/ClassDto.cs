// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators.Content.CommandGenerator;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ClassDto(ISymbol symbol, ClassDeclarationSyntax syntax) {
    private readonly AttributeData? _commandNameAttribute = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsCommand") == true);

    private readonly AttributeData? _descriptionAttribute = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsDescription") == true);

    private readonly ITypeSymbol? _genericTypeArgument = (symbol as ITypeSymbol)?.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition.ToDisplayString().EndsWith("ICommand<T>"))?.TypeArguments.FirstOrDefault();
    private readonly bool _hasEmptyConstructor = syntax.ParameterList?.Parameters.Count == 0;
    private readonly bool _isPartial = syntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    private readonly Location _location = symbol.Locations.First();

    public readonly string ClassName = symbol.Name;
    public readonly string Namespace = symbol.ContainingNamespace.ToDisplayString();
    private string GenericTypeDisplayName => _genericTypeArgument?.ToDisplayString() ?? "UNDEFINED";

    private string CommandName =>
        _commandNameAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString()
        ?? ClassName.Replace("Command", "").ToLowerInvariant();// Maybe create a ToKebabCase method?

    private string Description => _descriptionAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static ClassDto FromSyntax(GeneratorSyntaxContext context, ClassDeclarationSyntax classSyntax) {
        ISymbol symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classSyntax)!;
        return new ClassDto(symbol, classSyntax);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string ToDeclarationName() {
        string constructor = _hasEmptyConstructor ? string.Empty : "()";
        return $"{ClassName}{constructor}";
    }

    public void ToCommandData(GeneratorStringBuilder builder) {
        builder.AppendLine("public CommandData CommandData { get; } = new CommandData(")
            .AppendLineIndented($"\"{CommandName}\",")
            .AppendLineIndented($"\"{Description}\",")
            .AppendLineIndented($"typeof({symbol.ToDisplayString()})")
            .AppendLine(");");
    }

    public void ToCommandInitialization(GeneratorStringBuilder builder) {
        builder.AppendLine("public Task InitializeAsync(IUserInputRegistry registry) {")
            .AppendLineIndented($"var data = {GenericTypeDisplayName}.FromRegistry(registry);")
            .AppendLineIndented("return ExecuteAsync(data);")
            .AppendLine("}");
    }

    public void ReportDiagnostics(SourceProductionContext context) {
        if (!_isPartial) context.ReportCommandClassMustBePartial(_location, ClassName);
        if (_genericTypeArgument is null) context.ReportCommandClassMustImplementICommand(_location, ClassName);
    }
}
