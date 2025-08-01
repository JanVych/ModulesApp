using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ModulesPrograms;


[Table("ModuleFirmware")]
public class DbModuleFirmware
{
    public DbModuleFirmware() { }

    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public string IdfVersion { get; set; } = string.Empty;

    //public long IDFId { get; set; }
    //[ForeignKey("IDFId")]
    //public DbModuleIDF IDF { get; set; } = null!;

    public string NormalizedPath => Path.Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());
}
