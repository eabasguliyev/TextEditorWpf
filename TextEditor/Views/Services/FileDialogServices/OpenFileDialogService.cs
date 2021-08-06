using Microsoft.Win32;

namespace TextEditor.Views.Services.FileDialogServices
{
    public class OpenFileDialogService:IOpenFileDialogService
    {
        private readonly OpenFileDialog _dialog;

        public OpenFileDialogService()
        {
            _dialog = new OpenFileDialog();
        }

        public string FileName => _dialog.FileName;

        public bool? ShowDialog()
        {
            return _dialog.ShowDialog();
        }
    }
}