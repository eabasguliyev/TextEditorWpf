using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextEditor.ViewModels
{
    public interface IMainWindowViewModel
    {
        Task<List<string>> GetWrongWords();
        void SetEditorStatusData(int position, int line, int column);
    }
}