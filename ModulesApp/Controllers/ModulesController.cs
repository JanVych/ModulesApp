using Microsoft.AspNetCore.Mvc;
using ModulesApp.Models;
using ModulesApp.Services;
using ModulesApp.Services.Data;
using System.Text.Json;

namespace ModulesApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModulesController : ControllerBase
{
    private readonly ModuleService _moduleService;
    private readonly ActionService _modulActionsService;
    private readonly ServerTaskService _serverTasksService;

    private readonly ServerContextService _serverContextService;

    public ModulesController(ModuleService modulesService, ActionService moduleActionsService, ServerTaskService serverTaskService, ServerContextService serverContextService)
    {
        _moduleService = modulesService;
        _modulActionsService = moduleActionsService;
        _serverTasksService = serverTaskService;
        _serverContextService = serverContextService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        Console.WriteLine("GET");
        var data = new Dictionary<string, string>
        {
            { "name", "Jaroslav" },
            { "surname", "Basta" }
        };
        return Ok(data);
    }

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> PostJson([FromBody] JsonElement data)
    {
        Console.WriteLine("POST JSON");
        Console.WriteLine(data.ToString());

        var module = ProcessJsonData(data);
        if (module != null)
        {
            // if module exist in DB, update
            if (_moduleService.IsRegistrated(module.Id, module.Key))
            {
                return Ok(await ProcessExistingModule(module));
            }
            // or registrate new module
            else
            {
                return Ok(RegisterNewModule(module));
            }
        }
        return Ok();
    }

    private static DbModule? ProcessJsonData(JsonElement data)
    {
        try
        {
            return JsonSerializer.Deserialize<DbModule>(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return null;
        }
    }

    private async Task<string?> ProcessExistingModule(DbModule module)
    {
        module.LastResponse = DateTime.Now;
        // update module status
        _moduleService.Update(module);

        // process actions from module
        await _serverTasksService.ProcessNodes(_serverContextService, module);

        //get and delete all related actions
        var moduleActions = await _modulActionsService.GetListAndDeleteAsync(module);
        string response = string.Empty;
        if (moduleActions.Count != 0)
        {
            var jsonData = moduleActions.ToDictionary(a => a.Key, a => a.Value);
            response = JsonSerializer.Serialize(jsonData);
        }
        // send actions from server
        Console.WriteLine(response);
        return response;
    }

    private string? RegisterNewModule(DbModule module)
    {
        // add new module to DB,
        // TODO
        module.LastResponse = DateTime.Now;
        module.Id = 0;
        module.Key = module.LastResponse.GetHashCode().ToString();
        module.Name = $"esp-32-{module.LastResponse.GetHashCode() % 1_000_000}";
        _moduleService.Add(module);
        // registr new module
        var dict = new Dictionary<string, string>
        {
            { "SetModuleId", module.Id.ToString() },
            { "SetModuleName", module.Name },
            { "SetModuleKey", module.Key }
        };
        return JsonSerializer.Serialize(dict);
    }
}

