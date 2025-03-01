using Microsoft.AspNetCore.Mvc;
using ModulesApp.Services;
using System.Diagnostics;

namespace ModulesApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FirmwareController : ControllerBase
{
    FirmwareService _firmwareService;
    public FirmwareController(FirmwareService firmwareService)
    {
        _firmwareService = firmwareService;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string program)
    {
        Debug.WriteLine("GET FIRMWARE");
        Debug.WriteLine(program);
        var programs = _firmwareService.Programs;
        foreach (var p in programs)
        {
            if (p.Name == program)
            {
                var path = p.GetBinPath();
                if (path == null)
                {
                    return NotFound($"Binary file for program name: {program} not found");
                }
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return File(fileStream, "application/octet-stream", Path.GetFileName(path));
            }
        }
        return NotFound($"Program name: {program} not found");
    }
}