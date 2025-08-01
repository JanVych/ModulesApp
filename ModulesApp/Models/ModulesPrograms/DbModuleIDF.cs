//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ModulesApp.Models.ModulesPrograms;


//[Table("ModuleIDF")]
//public class DbModuleIDF
//{
//    public DbModuleIDF() { }

//    [Key]
//    public long Id { get; set; }
//    public string Name { get; set; } = string.Empty;
//    public string Path { get; set; } = string.Empty;
//    public string Version { get; set; } = string.Empty;

//    public string NormalizedPath => Path.Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());
//}
