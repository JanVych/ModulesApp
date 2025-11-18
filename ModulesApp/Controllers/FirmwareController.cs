using Microsoft.AspNetCore.Mvc;
using ModulesApp.Services.Data;

namespace ModulesApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FirmwareController : ControllerBase
{
    ModuleProgramRepository _moduleProgramService;
    public FirmwareController(ModuleProgramRepository moduleProgramService)
    {
        _moduleProgramService = moduleProgramService;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string program)
    {
        Console.WriteLine("GET FIRMWARE");
        Console.WriteLine(program);
        var programs = _moduleProgramService.GetProgramsList();

        foreach (var p in programs)
        {
            if (p.Name == program)
            {
                var path = p.GetProgramBinPath();
                Console.WriteLine(path);
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