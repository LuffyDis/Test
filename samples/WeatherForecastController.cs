using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

/// <summary>
/// Controller responsible for managing weather forecasts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastService _service;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherForecastService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Retrieves all weather forecasts.
    /// </summary>
    /// <returns>A list of weather forecasts.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecastDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetAll()
    {
        var forecasts = await _service.GetAllAsync();
        return Ok(forecasts);
    }

    /// <summary>
    /// Retrieves a specific weather forecast by id.
    /// </summary>
    /// <param name="id">The forecast identifier.</param>
    /// <returns>The requested weather forecast.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherForecastDto>> GetById(int id)
    {
        var forecast = await _service.GetByIdAsync(id);
        if (forecast is null)
            return NotFound();

        return Ok(forecast);
    }

    /// <summary>
    /// Creates a new weather forecast.
    /// </summary>
    /// <param name="request">The forecast creation request.</param>
    /// <returns>The created weather forecast.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(WeatherForecastDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherForecastDto>> Create([FromBody] CreateWeatherForecastRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing weather forecast.
    /// </summary>
    /// <param name="id">The forecast identifier.</param>
    /// <param name="request">The forecast update request.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWeatherForecastRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, request);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a weather forecast.
    /// </summary>
    /// <param name="id">The forecast identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
