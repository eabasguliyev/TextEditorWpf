using System.Runtime.InteropServices.ComTypes;

namespace TextEditor.Views.Services
{
    public interface IFileDialogService
    {
        public string FileName { get; }
        bool? ShowDialog();
    }

    public interface ISaveFileDialogService:IFileDialogService
    {

    }

    public interface IOpenFileDialogService : IFileDialogService
    {

    }
}