using UnitConversionApi.Models;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests;

public sealed class ConversionServiceTests
{
    private readonly ConversionService _service = new(new UnitRegistry());

    [Theory]
    [InlineData(1, "km", "m", 1000)]
    [InlineData(100, "cm", "m", 1)]
    [InlineData(1, "mi", "km", 1.609344)]
    [InlineData(1, "kg", "g", 1000)]
    [InlineData(1, "lb", "kg", 0.45359237)]
    public void Converts_linear_units(double value, string from, string to, double expected)
    {
        var result = _service.Convert(new ConversionRequest
        {
            Category = from is "kg" or "lb" ? "weight" : "length",
            Value = value,
            FromUnit = from,
            ToUnit = to
        });

        Assert.Equal(expected, result.Result, 6);
    }

    [Theory]
    [InlineData(0, "c", "f", 32)]
    [InlineData(100, "c", "f", 212)]
    [InlineData(32, "f", "c", 0)]
    [InlineData(0, "c", "k", 273.15)]
    public void Converts_temperature(double value, string from, string to, double expected)
    {
        var result = _service.Convert(new ConversionRequest
        {
            Category = "temperature",
            Value = value,
            FromUnit = from,
            ToUnit = to
        });

        Assert.Equal(expected, result.Result, 6);
    }

    [Fact]
    public void Rejects_temperature_below_absolute_zero()
    {
        var request = new ConversionRequest
        {
            Category = "temperature",
            Value = -300,
            FromUnit = "c",
            ToUnit = "k"
        };

        Assert.Throws<ArgumentException>(() => _service.Convert(request));
    }

    [Fact]
    public void Rejects_unknown_unit()
    {
        var request = new ConversionRequest
        {
            Category = "length",
            Value = 1,
            FromUnit = "parsec",
            ToUnit = "m"
        };

        Assert.Throws<KeyNotFoundException>(() => _service.Convert(request));
    }

    [Fact]
    public void Rejects_non_finite_value()
    {
        var request = new ConversionRequest
        {
            Category = "length",
            Value = double.NaN,
            FromUnit = "m",
            ToUnit = "km"
        };

        Assert.Throws<ArgumentException>(() => _service.Convert(request));
    }
}
