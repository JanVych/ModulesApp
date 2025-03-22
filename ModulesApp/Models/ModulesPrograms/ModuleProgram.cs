namespace ModulesApp.Models.ModulesPrograms;

public class ModuleIDF(string name, string path, string version)
{
    public string Name { get; } = name;
    public string AbsolutePath { get; } = path;
    public string Version { get; } = version;
}
public class ModuleFirmware(string name, string path, string version)
{
    public string Name { get; } = name;
    public string RelativePath { get; } = path;
    public string Version { get; } = version;
}

public class ModuleProgramFile(string fileName, string filePath, string? content)
{
    public string FileName { get; } = fileName;
    public string FilePath { get; } = filePath;
    public string? Content { get; set; } = content;
}

public class ModuleProgram(string name, string path)
{
    public string Name { get; } = name;
    public string RelativePath { get; } = path;
    public string AbsolutePath => Path.GetFullPath(RelativePath);
    public IReadOnlyList<ModuleProgramFile>? Files { get; private set; }

    public async Task LoadtProgramFiles()
    {
        var list = new List<ModuleProgramFile>();
        var programPath = Path.Combine(RelativePath, Path.Join("components", "program"));
        try
        {
            foreach (var f in Directory.GetFiles(programPath))
            {
                list.Add(new ModuleProgramFile(Path.GetFileName(f), f, await File.ReadAllTextAsync(f)));
            }
            Files = list;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public string? GetBinPath()
    {
        var path = Path.Combine(RelativePath, "build");
        return Directory.GetFiles(path, "main-project-1.bin").FirstOrDefault();
    }
}
