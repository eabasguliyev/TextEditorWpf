using System.Text;

namespace TextEditor.DatabaseSchemaBuilder
{
    public class IndexBuilder
    {
        private readonly StringBuilder _str;

        public IndexBuilder()
        {
            _str = new StringBuilder();
        }

        public string DefaultIndexName(string tableName, string columnName) => $"IX_{tableName}_{columnName}";

        public IndexBuilder SetIfNotExist(string tableName, string columnName)
        {
            _str.Append(
                $"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = '{DefaultIndexName(tableName, columnName)}')");
            return this;
        }
        public IndexBuilder CreateIndex(string tableName, string columnName, IndexType indexType = IndexType.None)
        {
            _str.Append("CREATE ");

            if (indexType != IndexType.None)
            {
                _str.Append(indexType.ToString().ToUpper() + " ");
            }

            _str.Append($"INDEX {DefaultIndexName(tableName, columnName)} ON {tableName}({columnName});");

            return this;
        }

        public IndexBuilder Reset()
        {
            _str.Clear();

            return this;
        }


        public string Build()
        {
            return _str.ToString();
        }
    }
}