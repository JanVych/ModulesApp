using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ModulesPrograms;


[Table("ModuleProgram")]
public class DbModuleProgram
{
    public DbModuleProgram() { }

    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public long FirmwareId { get; set; }
    [ForeignKey("FirmwareId")]
    public DbModuleFirmware Firmware { get; set; } = null!;

    public List<DbModuleProgramFile> Files { get; private set; } = [];

    public string NormalizedPath => Path.Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());

    public async Task LoadtProgramFiles() 
    {
        try
        {
            foreach (var f in Files)
            {
                f.Content = await File.ReadAllTextAsync(f.Path);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public string? GetBinPath()
    {
        var path = System.IO.Path.Combine(Path, "build");
        return Directory.GetFiles(path, "main-project-1.bin").FirstOrDefault();
    }
}
