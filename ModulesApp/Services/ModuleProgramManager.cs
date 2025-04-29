using System.Diagnostics;
using System.Runtime.InteropServices;
using ModulesApp.Models.ModulesPrograms;
using ModulesApp.Services.Data;

namespace ModulesApp.Services;

public class ModuleProgramManager
{
    private readonly IConfiguration _configuration;
    private readonly ModuleProgramService _moduleProgramService;

    public ModuleProgramManager(IConfiguration configuration, ModuleProgramService moduleProgramService)
    {
        _configuration = configuration;
        _moduleProgramService = moduleProgramService;
    }

    public async Task BuildProgramAsync(DbModuleProgram program, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => BuildProgram(program, outputHandler));
    }
    public void BuildProgram(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        RunCMDCommand(program, "build", outputHandler);
    }

    public async Task CleanProgramAsync(DbModuleProgram program, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => CleanProgram(program, outputHandler));
    }
    public void CleanProgram(DbModuleProgram program, DataReceivedEventHandler? outputHandler)
    {
        RunCMDCommand(program, "fullclean", outputHandler);
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

    private static void RunCMDCommand(DbModuleProgram program, string command, DataReceivedEventHandler? outputHandler)
    {
        var programPath = Path.GetFullPath(program.Path);
        var idfPath = program.Firmware.IDF.NormalizedPath;

        if (!Directory.Exists(programPath))
        {
            Console.WriteLine($"Path: {programPath}, does not exist");
            return;
        }
        if(!Directory.Exists(idfPath))
        {
            Console.WriteLine($"Path: {idfPath}, does not exist");
            return;
        }
        RunCMDProcess(idfPath, programPath, command, outputHandler);
    }

    private static void RunCMDProcess(string idfAbsolutePath, string programAbsolutePath, string command, DataReceivedEventHandler? outputHandler)
    {
        Process cmdProcess = new();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            cmdProcess.StartInfo.WorkingDirectory = programAbsolutePath;
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = $"/C {idfAbsolutePath}\\install.bat && {idfAbsolutePath}\\export.bat && idf.py {command}";
            //cmdProcess.StartInfo.Arguments = $"/C {idfAbsolutePath}\\export.bat && idf.py {command}";
        }

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // TODO install.sh ?
            cmdProcess.StartInfo.WorkingDirectory = programAbsolutePath;
            cmdProcess.StartInfo.FileName = "/bin/bash";
            cmdProcess.StartInfo.Arguments = $"-c \"source {idfAbsolutePath}/export.sh && idf.py {command}\"";
        }


        cmdProcess.StartInfo.RedirectStandardOutput = true;
        cmdProcess.StartInfo.UseShellExecute = false;
        cmdProcess.StartInfo.CreateNoWindow = false;
        if (outputHandler != null)
        {
            cmdProcess.OutputDataReceived += new DataReceivedEventHandler(outputHandler);
        }

        cmdProcess.Start();
        cmdProcess.BeginOutputReadLine();
        cmdProcess.WaitForExit();
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

    //private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    //{
    //    if (!string.IsNullOrEmpty(outLine.Data))
    //    {
    //        Debug.WriteLine(outLine.Data);
    //    }
    //}
}
