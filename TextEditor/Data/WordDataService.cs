using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TextEditor.Models;

namespace TextEditor.Data
{
    public class WordDataService:IWordDataService
    {
        private readonly SqlConnection _sqlConnection;

        public WordDataService(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }
        public async Task<List<Word>> GetAllAsync()
        {
            await _sqlConnection.OpenAsync();
            var words = await _sqlConnection.QueryAsync<Word>("SELECT * FROM Words");

            await _sqlConnection.CloseAsync();
            return words.ToList();
        }

        public async Task<int> AddAsync(Word word)
        {
            await _sqlConnection.OpenAsync();
            
            var affectedRow =  await _sqlConnection.ExecuteAsync("INSERT INTO Words (WordName) VALUES (@WordName);", new
            {
                WordName = word.WordName,
            });

            await _sqlConnection.CloseAsync();

            return affectedRow;
        }

        public async Task<Word> GetByNameAsync(string wordName)
        {
            await _sqlConnection.OpenAsync();

            var word = await _sqlConnection.QuerySingleOrDefaultAsync<Word>(
                "SELECT TOP 1 * FROM Words WHERE WordName = @WordName", new
                {
                    WordName = wordName
                });

            await _sqlConnection.CloseAsync();

            return word;
        }
    }
}