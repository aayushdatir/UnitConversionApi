using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IUnitRegistry
{
    IReadOnlyCollection<string> Categories { get; }
    IReadOnlyCollection<UnitDefinition> GetUnits(string category);
    bool TryGetUnit(string category, string unit, out UnitDefinition definition);
}
