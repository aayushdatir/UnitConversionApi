using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IConversionService
{
    ConversionResponse Convert(ConversionRequest request);
}
