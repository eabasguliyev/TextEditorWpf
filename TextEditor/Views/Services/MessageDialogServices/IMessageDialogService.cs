using TextEditor.Enums;

namespace TextEditor.Views.Services.MessageDialogServices
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowYesNoDialog(string text, string title);
    }
}