//namespace UICrafter.API;

//using System.Text.Json;
//using Microsoft.AspNetCore.Mvc;
//using Serilog;

//[Route("csp-report-endpoint")]
//[ApiController]
//public class CspReportController : ControllerBase
//{
//	[HttpPost]
//	public IActionResult ReportCspViolation([FromBody] JsonElement cspReport)
//	{
//		// Log the report or save it to a database
//		Log.Information("CSP Violation:", cspReport);
//		return Ok();
//	}
//}

