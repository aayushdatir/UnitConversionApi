using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ConversionsController(
    IConversionService conversionService,
    IUnitRegistry registry) : ControllerBase
{
    /// <summary>Convert a value from one unit to another within a category.</summary>
    [HttpPost]
    [ProducesResponseType<ConversionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        try
        {
            return Ok(conversionService.Convert(request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Unsupported category or unit",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid conversion request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>List all supported conversion categories.</summary>
    [HttpGet("categories")]
    public ActionResult<IReadOnlyCollection<string>> GetCategories() => Ok(registry.Categories);

    /// <summary>List supported units for a category, for example length or temperature.</summary>
    [HttpGet("units/{category}")]
    public ActionResult<IReadOnlyCollection<UnitDefinition>> GetUnits(string category)
    {
        try
        {
            return Ok(registry.GetUnits(category));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Unknown category",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}
