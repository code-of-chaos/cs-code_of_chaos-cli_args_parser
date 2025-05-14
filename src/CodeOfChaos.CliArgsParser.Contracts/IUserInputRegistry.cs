// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IParameterDictionary {
    T GetParameterByPossibleNames<T>(string name, string shortName);
    T? GetOptionalParameterByPossibleNames<T>(string name, string shortName);

    T GetParameter<T>(string key);
    T? GetOptionalParameter<T>(string key);
}
