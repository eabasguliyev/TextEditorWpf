namespace TextEditor.ViewModels
{
    public interface IMainWindowViewModel
    {
        void LoadAsync();
        void OnTextChanged();
    }
}