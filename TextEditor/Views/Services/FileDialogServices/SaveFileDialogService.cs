using Microsoft.Win32;

namespace TextEditor.Views.Services.FileDialogServices
{
    public class SaveFileDialogService : ISaveFileDialogService
    {
        private readonly SaveFileDialog _dialog;

        public SaveFileDialogService(string filter)
        {
            _dialog = new SaveFileDialog();

            _dialog.Filter = filter;
        }

        public string FileName => _dialog.FileName;

        public bool? ShowDialog()
        {
            return _dialog.ShowDialog();
        }
    }
}