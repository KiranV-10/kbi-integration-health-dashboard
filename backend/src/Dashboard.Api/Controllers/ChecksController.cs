using Dashboard.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("api/checks")]
public class ChecksController : ControllerBase
{
    private readonly IHealthCheckRunner _healthCheckRunner;

    public ChecksController(IHealthCheckRunner healthCheckRunner)
    {
        _healthCheckRunner = healthCheckRunner;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunChecks(CancellationToken cancellationToken)
    {
        var results = await _healthCheckRunner.RunChecksAsync(cancellationToken);
        return Ok(results);
    }
}
