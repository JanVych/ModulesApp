using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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

    public class ModuleBinFile
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int Address { get; set; }
        public string? Data { get; set; }

        public async Task LoadData()
        {
            if (File.Exists(Path))
            {
                var builder = new StringBuilder();
                foreach (var b in await File.ReadAllBytesAsync(Path))
                {
                    builder.Append((char)b);
                }
                Data = builder.ToString();
            }
        }
    }

    [NotMapped]
    public List<ModuleBinFile> BinFiles { get; set; } = [];

    public string? GetProgramBinPath()
    {
        var path = System.IO.Path.Combine(Path, "build");
        return Directory.GetFiles(path, "main-project-1.bin").FirstOrDefault();
    }

    public async Task LoadBinFilesData()
    {
        foreach (var file in BinFiles)
        {
            await file.LoadData();
        }
    }

    public void LoadBinFiles()
    {
        var files = new List<ModuleBinFile>();
        var path = System.IO.Path.Combine(Path, "build");

        files.Add(new()
        {
            Name = "bootloader",
            Path = System.IO.Path.Combine(path, "bootloader", "bootloader.bin"),
            Address = 0x1000
        });
        files.Add(new()
        {
            Name = "main-project-1",
            Path = System.IO.Path.Combine(path, "main-project-1.bin"),
            Address = 0x30000
        });
        files.Add(new()
        {
            Name = "partition_table",
            Path = System.IO.Path.Combine(path, "partition_table", "partition-table.bin"),
            Address = 0x8000
        });
        files.Add(new()
        {
            Name = "ota_data_initial",
            Path = System.IO.Path.Combine(path, "ota_data_initial.bin"),
            Address = 0x29000
        });
        files.Add(new()
        {
            Name = "phy_init_data",
            Path = System.IO.Path.Combine(path, "phy_init_data.bin"),
            Address = 0x2b000
        });
        BinFiles = files;
    }
}
