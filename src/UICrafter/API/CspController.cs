namespace UICrafter.API;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

public class CspController : Controller
{
	private readonly ILogger<CspController> _logger;

	public CspController(ILogger<CspController> logger)
	{
		_logger = logger;
	}

	[HttpPost("csp-violations")]
	public IActionResult CSPReport([FromBody] JsonElement cspViolation)
	{
		_logger.LogInformation("CSP Violation: {Report}", cspViolation);
		return Ok();
	}
}
