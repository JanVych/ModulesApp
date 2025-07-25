using System.Diagnostics;
using System.Runtime.InteropServices;
using ModulesApp.Models.ModulesPrograms;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class ModuleProgramManager
{
    private readonly IConfiguration _configuration;
    private readonly ModuleProgramService _moduleProgramService;
    //private IServiceProvider _serviceProvider;

    public ModuleProgramManager(IConfiguration configuration, ModuleProgramService moduleProgramService)
    {
        _configuration = configuration;
        _moduleProgramService = moduleProgramService;
    }
    //public ModuleProgramManager(IServiceProvider services)
    //{

    //}

    public async Task BuildProgramAsync(DbModuleProgram program, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => BuildProgram(program, outputHandler));
    }
    public void BuildProgram(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        RunEspIdfCommand(program, "idf.py build", outputHandler);
    }

    public async Task CleanProgramAsync(DbModuleProgram program, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => CleanProgram(program, outputHandler));
    }
    public void CleanProgram(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        RunEspIdfCommand(program, "idf.py fullclean", outputHandler);
    }

    public async Task BuildNvsDataAync(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        await Task.Run(() => BuildNvsData(program, outputHandler));
    }
    public void BuildNvsData(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        var idfPath = Path.GetFullPath(program.Firmware.IDF.Path);
        var commandPath = Path.Combine(idfPath, "components", "nvs_flash", "nvs_partition_generator", "nvs_partition_gen.py");
        var csvFile = "nvs_data.csv";
        var binFile = "nvs_data.bin";
        var partitionSize = "0x20000";

        var command = $"python \"{commandPath}\" generate \"{csvFile}\" \"{binFile}\" {partitionSize}";
        RunEspIdfCommand(program, command, OutputHandler);
    }

    private static void RunEspIdfCommand(DbModuleProgram program, string command, DataReceivedEventHandler? outputHandler)
    {
        var programPath = Path.GetFullPath(program.Path);
        var idfPath = Path.GetFullPath(program.Firmware.IDF.Path);

        if (!Directory.Exists(programPath))
        {
            InvokeOutputHandler(outputHandler, $"Program path does not exist: {programPath}");
            return;
        }
        if(!Directory.Exists(idfPath))
        {
            InvokeOutputHandler(outputHandler, $"IDF path does not exist: {idfPath}");
            return;
        }
        RunEspIdfProcess(idfPath, programPath, command, outputHandler);
    }

    private static void RunEspIdfProcess(string idfAbsolutePath, string programAbsolutePath, string command, DataReceivedEventHandler? outputHandler)
    {
        using Process cmdProcess = new();
        cmdProcess.StartInfo.WorkingDirectory = programAbsolutePath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = $"/C {idfAbsolutePath}\\install.bat && {idfAbsolutePath}\\export.bat && {command}";
        }

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // TODO fix install.sh ?
            cmdProcess.StartInfo.FileName = "/bin/bash";
            cmdProcess.StartInfo.Arguments = $"-c \"source {idfAbsolutePath}/export.sh && {command}\"";
        }

        cmdProcess.StartInfo.RedirectStandardOutput = true;
        cmdProcess.StartInfo.RedirectStandardError = true;
        cmdProcess.StartInfo.UseShellExecute = false;
        cmdProcess.StartInfo.CreateNoWindow = true;

        if (outputHandler != null)
        {
            cmdProcess.OutputDataReceived += outputHandler;
            cmdProcess.ErrorDataReceived += outputHandler;
        }

        cmdProcess.Start();
        if (outputHandler != null)
        {
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();
        }
        cmdProcess.WaitForExit();
    }

    public async Task<DbModuleProgram?> CreateNewProgram(DbModuleProgram program, DbModuleFirmware firmware)
    {
        var programPath = _configuration["AppSettings:ProgramPath"];
        if (string.IsNullOrEmpty(programPath))
        {
            Console.WriteLine($"Config: AppSettings:ProgramPath, does not exist in appsettings.json");
            return null;
        }
        program.FirmwareId = firmware.Id;
        program = await _moduleProgramService.Add(program);

        program.Path = Path.Combine(programPath, program.Id.ToString());
        await Task.Run(() => CopyAllFromDirectory(firmware.NormalizedPath, program.Path));

        var programFilesPath = Path.Combine(program.Path, "components","program");
        foreach (var f in Directory.GetFiles(programFilesPath))
        {
            program.Files.Add(new()
            {
                Name = Path.GetFileName(f),
                Path = f
            });
        }
        await _moduleProgramService.Update(program);
        return program;
    }

    public async Task DeleteProgram(DbModuleProgram program)
    {
        await _moduleProgramService.Delete(program);
        if (Directory.Exists(program.Path))
        {
            Directory.Delete(program.Path, true);
        }
    }

    public async Task SaveProgram(DbModuleProgram program)
    {
        await _moduleProgramService.Update(program);
        if (program.Files != null)
        {
            foreach (var f in program.Files)
            {
                await File.WriteAllTextAsync(f.Path, f.Content);
            }
        }
    }

    private static void CopyAllFromDirectory(string from, string to)
    {
        var dir = new DirectoryInfo(from);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {from}");
        }

        if (!Directory.Exists(to))
        {
            Directory.CreateDirectory(to);
        }

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(to, file.Name);
            file.CopyTo(targetFilePath, true);
        }
        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            string newDestinationDir = Path.Combine(to, subDir.Name);
            CopyAllFromDirectory(subDir.FullName, newDestinationDir);
        }
    }

    private static void InvokeOutputHandler(DataReceivedEventHandler? handler, string message)
    {
        if (handler == null) return;

        var ctor = typeof(DataReceivedEventArgs)
            .GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                            null, [typeof(string)], null);

        var args = ctor?.Invoke([message]) as DataReceivedEventArgs;
        handler.Invoke(null, args!);
    }

    private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
