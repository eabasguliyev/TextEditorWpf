namespace TextEditor.Views.Services.FileDialogServices
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