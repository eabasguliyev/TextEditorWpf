using System.Collections.Generic;
using System.Threading.Tasks;
using TextEditor.Models;

namespace TextEditor.Data
{
    public interface IWordDataService
    {
        Task<List<Word>> GetAllAsync();

        Task<int> AddAsync(Word word);

        Task<Word> GetByNameAsync(string wordName);
    }
}