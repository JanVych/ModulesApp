using ModulesApp.Types;
using MudBlazor;

namespace ModulesApp.Components.Dialog;

public static class DialogServiceExtensions
{
    public static async Task<DialogResult?> ShowAndReturnAsync(
        this IDialogService dialogService,
        string title,
        string contentText,
        string confirmText = "Confirm",
        Color color = Color.Primary)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<UserDialog>
        {
            { x => x.ContentText, contentText },
            { x => x.ButtonText, confirmText },
            { x => x.Color, color }
        };

        var dialog = await dialogService.ShowAsync<UserDialog>(title, parameters, options);
        var result = await dialog.Result;

        return result;
    }

    public static async Task<DialogResult?> ShowAndReturnAsync(this IDialogService dialogService, DialogType dialogType, string? contentText = null)
    {
        var (title, primaryContentText, confirmText, color) = _dialogTypeMappings[dialogType];
        return await ShowAndReturnAsync(dialogService, title, contentText ?? primaryContentText, confirmText, color);
    }

    private static readonly Dictionary<DialogType, (string title, string contentText, string confirmText, Color color)> _dialogTypeMappings = new()
    {
        [DialogType.Logout] = ("Logout", "Are you sure you want to logout?", "Confirm", Color.Primary),
        [DialogType.DeleteService] = ("Delete Service", "Are you sure you want to delete this service?", "Delete", Color.Error),
        [DialogType.DeleteModule] = ("Delete Module", "Are you sure you want to delete this module?", "Delete", Color.Error),
        [DialogType.DeleteTask] = ("Delete Task", "Are you sure you want to delete this task?", "Delete", Color.Error),
        [DialogType.DeleteProgram] = ("Delete Program", "Are you sure you want to delete this program?", "Delete", Color.Error),
        [DialogType.BuildProgram] = ("Build Program", "Are you sure you want to build this program?", "Build", Color.Primary),
        [DialogType.CleanProgram] = ("Clean Program", "Are you sure you want to clean this program?", "Clean", Color.Primary),
        [DialogType.ProgramSendOTA] = ("Send OTA", "Are you sure you want to inicialize OTA update?", "Send", Color.Primary),
    };
}