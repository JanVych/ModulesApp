using System.Diagnostics;
using System.Runtime.InteropServices;
using ModulesApp.Models.ModulesPrograms;

namespace ModulesApp.Services;

public class FirmwareService
{
    private readonly IConfiguration _configuration;

    private readonly List<ModuleFirmware> _firmware = [];
    public IReadOnlyList<ModuleFirmware> Firmware => _firmware;

    private readonly List<ModuleIDF> _idf = [];
    public IReadOnlyList<ModuleIDF> IDF => _idf;

    private readonly List<ModuleProgram> _programs = [];
    public IReadOnlyList<ModuleProgram> Programs => _programs;

    public FirmwareService(IConfiguration configuration)
    {
        _configuration = configuration;

        LoadFrimware();
        LoadIDF();
        LoadPrograms();

        //CleanProgram("test", "esp-idf-v5.2.3", OutputHandler);
        // BuildProgram("test", "esp-idf-v5.2.3", OutputHandler);
        //CreateNewProgram("test2", "main-project-1");
        //GetProgramBinPath("test2");
        //BuildProgram("test2", "esp-idf-v5.2.3", OutputHandler);
    }

    private string GetPathFromAppSettings(string appSettingsString)
    {
        var path = _configuration[appSettingsString];
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception($"Config: {path}, does not exist in appsettings.json");
        }
        if (!Path.Exists(path))
        {
            throw new Exception($"Path: {path}, does not exist");
        }
        return path;
    }

    private void LoadFrimware()
    {
        var path = GetPathFromAppSettings("AppSettings:FirmwarePath");
        _firmware.Clear();
        foreach (var d in Directory.GetDirectories(path))
        {
            _firmware.Add(new ModuleFirmware(Path.GetFileName(d), d, string.Empty));
        }
    }

    private void LoadIDF()
    {
        var path = GetPathFromAppSettings("AppSettings:IdfAbsolutePath");
        _idf.Clear();
        foreach (var d in Directory.GetDirectories(path))
        {
            _idf.Add(new ModuleIDF(Path.GetFileName(d), d, string.Empty));
        }
    }

    private void LoadPrograms()
    {
        var path = GetPathFromAppSettings("AppSettings:ProgramPath");
        _programs.Clear();
        foreach (var d in Directory.GetDirectories(path))
        {
            _programs.Add(new ModuleProgram(Path.GetFileName(d), d));
        }
    }

    public async Task BuildProgramAsync(string programName, string idfName, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => BuildProgram(programName, idfName, outputHandler));
    }
    public void BuildProgram(string programName, string idfName, DataReceivedEventHandler? outputHandler)
    {
        RunCMDCommand(idfName, programName, "build", outputHandler);
    }

    public async Task CleanProgramAsync(string programName, string idfName, DataReceivedEventHandler outputHandler)
    {
        await Task.Run(() => CleanProgram(programName, idfName, outputHandler));
    }
    public void CleanProgram(string programName, string idfName, DataReceivedEventHandler? outputHandler)
    {
        RunCMDCommand(idfName, programName, "fullclean", outputHandler);
    }

    public void CreateNewProgram(string newName, string firmwareName)
    {
        if (Programs.FirstOrDefault(x => x.Name == newName)?.RelativePath != null)
        {
            Debug.WriteLine($"Program: {newName}, already exists");
            return;
        }

        var firmwarePath = Firmware.FirstOrDefault(x => x.Name == firmwareName)?.RelativePath;
        if (firmwarePath == null)
        {
            Debug.WriteLine($"Firmware: {firmwareName}, does not exist");
            return;
        }

        var programPath = _configuration["AppSettings:ProgramPath"];
        if (string.IsNullOrEmpty(programPath))
        {
            Debug.WriteLine($"Config: AppSettings:FirmwarePath, does not exist in appsettings.json");
            return;
        }

        programPath = Path.Combine(programPath, newName);
        CopyAllFromDirectory(firmwarePath, programPath);
        _programs.Add(new ModuleProgram(newName, programPath));
    }

    public void DeleteProgram(ModuleProgram program)
    {
        _programs.Remove(program);
        if (Directory.Exists(program.RelativePath))
        {
            Directory.Delete(program.RelativePath, true);
        }
    }

    public async Task SaveProgram(ModuleProgram program)
    {
        if (program.Files != null)
        {
            foreach (var f in program.Files)
            {
                await File.WriteAllTextAsync(f.FilePath, f.Content);
            }
        }
    }

    private void RunCMDCommand(string idfName, string programName, string command, DataReceivedEventHandler? outputHandler)
    {
        var idfPath = IDF.FirstOrDefault(x => x.Name == idfName)?.AbsolutePath;
        var programPath = Programs.FirstOrDefault(x => x.Name == programName)?.RelativePath;
        if (idfPath == null)
        {
            Debug.WriteLine($"Invalid idf name: {idfName}");
            return;
        }
        if (programPath == null)
        {
            Debug.WriteLine($"Invalid program name: {programName}");
            return;
        }
        RunCMDProcess(idfPath, programPath, command, outputHandler);
    }

    private static void RunCMDProcess(string idfPath, string programPath, string command, DataReceivedEventHandler? outputHandler)
    {
        Process cmdProcess = new();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows-specific commands
            cmdProcess.StartInfo.WorkingDirectory = programPath;
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = $"/C {idfPath}\\install.bat && {idfPath}\\export.bat && idf.py {command}";
        }
        // TODO: Add support for Linux
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //{
        //    // Linux-specific commands
        //    cmdProcess.StartInfo.WorkingDirectory = programPath;
        //    cmdProcess.StartInfo.FileName = "/bin/bash";
        //    cmdProcess.StartInfo.Arguments = $"-c \"source {idfPath}/install.sh && source {idfPath}/export.sh && idf.py {command}\"";
        //}

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

    private static void CopyAllFromDirectory(string sourceDir, string destinationDir)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
        }

        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }
        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
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
