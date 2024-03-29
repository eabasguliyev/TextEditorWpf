﻿using System.Text;

namespace TextEditor.DatabaseSchemaBuilder
{
    public class ColumnBuilder
    {
        private readonly StringBuilder _str;

        public ColumnBuilder()
        {
            _str = new StringBuilder();
        }

        public ColumnBuilder SetColumnName(string columnName)
        {
            _str.Append(columnName);

            return this;
        }

        public ColumnBuilder SetColumnType(DataType dataType, int length = -1)
        {
            _str.Append($" {dataType.ToString().ToUpper()}");

            if (dataType == DataType.Varchar)
            {
                if (length == -1)
                    // max length of varchar
                    _str.Append("(MAX)");
                else
                    _str.Append($"({length})");
            }
            else if (dataType == DataType.Decimal)
                _str.Append($"(18, 5)");
            return this;
        }

        public ColumnBuilder SetNotNull()
        {
            _str.Append($" NOT NULL");

            return this;
        }

        public ColumnBuilder SetIdentity(int startIndex = 1, int seedValue = 1)
        {
            _str.Append($" IDENTITY({startIndex},{seedValue})");

            return this;
        }

        public ColumnBuilder SetPrimaryKey()
        {
            _str.Append(" PRIMARY KEY");

            return this;
        }
        public ColumnBuilder Reset()
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