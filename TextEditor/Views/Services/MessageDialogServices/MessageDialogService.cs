using System.Windows;
using TextEditor.Enums;

namespace TextEditor.Views.Services.MessageDialogServices
{
    public class MessageDialogService:IMessageDialogService
    {
        public MessageDialogResult ShowYesNoDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Information);

            return result == MessageBoxResult.Yes ? MessageDialogResult.Yes : MessageDialogResult.No;
        }
    }
}