using CarParkManager.Application.Models;
using CarParkManager.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarParkManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ParkingController(IParkingService parkingService) : ControllerBase
{
    private readonly IParkingService _parkingService = parkingService;

    /// <summary>
    /// Parks a given vehicle in the first available space and returns the vehicle and its space number.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AllocateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Allocate([FromBody] AllocateRequest request)
    {
        try
        {
            var result = await _parkingService.AllocateAsync(request);
            if (result is null)
                return Conflict("No available spaces.");

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Gets available and occupied number of spaces.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Status()
    {
        var result = await _parkingService.GetStatusAsync();
        return Ok(result);
    }

    /// <summary>
    /// Frees up this vehicle's space and returns its final charge from its parking time until now.
    /// </summary>
    [HttpPost("exit")]
    [ProducesResponseType(typeof(ExitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exit([FromBody] ExitRequest request)
    {
        var result = await _parkingService.ExitAsync(request);
        if (result is null)
            return NotFound("Vehicle not found or not parked.");

        return Ok(result);
    }
}
