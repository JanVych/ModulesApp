using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ModulesPrograms;


[Table("ModuleProgramFile")]
public class DbModuleProgramFile
{
    public DbModuleProgramFile() { }

    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Content { get; set; }

    public long ProgramId { get; set; }
    [ForeignKey("ProgramId")]
    public DbModuleProgram Program { get; set; } = null!;

    public string NormalizedPath => Path.Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());
}
