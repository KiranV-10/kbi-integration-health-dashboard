using Dashboard.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly IServiceQueryService _serviceQueryService;

    public ServicesController(IServiceQueryService serviceQueryService)
    {
        _serviceQueryService = serviceQueryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices(
        [FromQuery] bool? activeOnly,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var services = await _serviceQueryService.GetServicesAsync(activeOnly, search, cancellationToken);
        return Ok(services);
    }
}
