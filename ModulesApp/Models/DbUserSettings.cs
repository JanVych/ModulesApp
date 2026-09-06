using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models;

[Table("UserSettings")]
public class DbUserSettings
{
    [Key]
    public long Id { get; set; }

    public UserSettings Settings { get; set; } = new();

}

public class UserSettings
{
    public bool IsEditButtonVisibleOnSmallScreens { get; set; } = false;
}
