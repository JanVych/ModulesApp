using MudBlazor;

namespace ModulesApp.Components.Dialog;

public static class DialogServiceExtensions
{
    public static async Task<bool> ShowAsyncAndReturnResult(
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

        return result?.Canceled ?? true;
    }
}