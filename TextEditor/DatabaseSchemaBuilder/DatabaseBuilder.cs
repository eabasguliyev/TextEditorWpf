using System.Text;

namespace TextEditor.DatabaseSchemaBuilder
{
    public class DatabaseBuilder
    {
        private readonly StringBuilder _str;

        public DatabaseBuilder()
        {
            _str = new StringBuilder();
        }

        public DatabaseBuilder SetIfNotExist(string dbName)
        {
            _str.Append(
                $"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '{dbName}' OR name = '{dbName}'))");

            return this;
        }
        public DatabaseBuilder SetDatabaseName(string dbName)
        {
            _str.Append($"CREATE DATABASE {dbName};");

            return this;
        }

        public DatabaseBuilder Reset()
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