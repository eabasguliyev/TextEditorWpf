using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextEditor.ViewModels
{
    public interface IMainWindowViewModel
    {
        void LoadAsync();
        void OnTextChanged();
        void SetLine(int line);
        void SetColumn(int column);
        void SetPosition(int position);
        Task<List<string>> GetWrongWords();
    }
}