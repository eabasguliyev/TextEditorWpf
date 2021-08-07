using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Dapper;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Prism.Events;
using TextEditor.Data;
using TextEditor.DatabaseSchemaBuilder;
using TextEditor.Enums;
using TextEditor.Events;
using TextEditor.Snapshot;
using TextEditor.ViewModels.Services;
using TextEditor.Views.Services;
using TextEditor.Views.Services.FileDialogServices;
using TextEditor.Views.Services.MessageDialogServices;

namespace TextEditor.ViewModels
{
    public class MainWindowViewModel:ObservableObject, IMainWindowViewModel
    {
        private readonly ISnapshotCare<string> _snapshotCare;
        private readonly Func<string, ISaveFileDialogService> _saveFileDialogServiceCreator;
        private readonly Func<string, IOpenFileDialogService> _openFileDialogServiceCreator;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly SqlConnection _sqlConnection;
        private readonly IWordDataService _wordDataService;
        private string _text;
        private readonly string _filter;

        private string _filePath;
        private string _tmpFilePath;
        private int _wordCount;
        private int _line;
        private int _column;
        private int _position;

        public MainWindowViewModel(ISnapshotCare<string> snapshotCare, Func<string, ISaveFileDialogService> saveFileDialogServiceCreator, 
            Func<string, IOpenFileDialogService> openFileDialogServiceCreator, IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            SqlConnection sqlConnection, IWordDataService wordDataService)
        {
            _snapshotCare = snapshotCare;
            _saveFileDialogServiceCreator = saveFileDialogServiceCreator;
            _openFileDialogServiceCreator = openFileDialogServiceCreator;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _sqlConnection = sqlConnection;
            _wordDataService = wordDataService;

            _filter = "Text files (*.txt)|*.txt";

            SaveCommand = new DelegateCommand(Save);
            OpenCommand = new DelegateCommand(Open);
            NewFileCommand = new DelegateCommand(CreateNewFile);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            CloseCommand = new DelegateCommand(Close);

            _eventAggregator.GetEvent<OnSaveChangesEvent>().Subscribe(OnSaveChanges);
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public string FileName => ParseFileNameFromPath(_filePath);
        public bool HasChanges { get; set; }

        public int WordCount
        {
            get => _wordCount;
            set
            {
                _wordCount = value;
                OnPropertyChanged();
            }
        }

        public int Line
        {
            get => _line;
            set
            {
                _line = value;
                OnPropertyChanged();
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                OnPropertyChanged();
            }
        }

        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand NewFileCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }


        public void OnTextChanged()
        {
            _snapshotCare.CreateSnapshot(Text);

            HasChanges = true;
            
            CountWords();

            AutoSave();
        }

        public void SetLine(int line)
        {
            Line = line;
        }

        public void SetColumn(int column)
        {
            Column = column;
        }

        public void SetPosition(int position)
        {
            Position = position;
        }

        public async void LoadAsync()
        {
            await CreateDatabaseSchema();

            //var words = await _wordDataService.GetAllAsync();
            //var word = await _wordDataService.GetByNameAsync("necesen");

            _snapshotCare.CreateSnapshot("");

            await Task.Run(CreateTempFile);
        }

        private async Task CreateDatabaseSchema()
        {
            var dbName = "TextEditor";

            var dbBuilder = new DatabaseBuilder();

            var dbString = dbBuilder.SetIfNotExist(dbName)
                .SetDatabaseName(dbName)
                .Build();
            
            await _sqlConnection.OpenAsync();

            await _sqlConnection.ExecuteAsync(dbString);

            await _sqlConnection.CloseAsync();

            _sqlConnection.ConnectionString += $"Initial Catalog = {dbName};";

            await _sqlConnection.OpenAsync();

            var columnBuilder = new ColumnBuilder();

            var idColumnStr = columnBuilder.SetColumnName("Id")
                .SetColumnType(DataType.Int)
                .SetNotNull()
                .SetIdentity()
                .SetPrimaryKey().Build();

            var wordNameColumnStr = columnBuilder.Reset()
                .SetColumnName("WordName")
                .SetColumnType(DataType.Varchar, 255)
                .Build();

            var tableBuilder = new TableBuilder();

            var tableSchemaStr = tableBuilder.SetIfNotExist("Words")
                .SetTableName("Words")
                .AddColumn(idColumnStr)
                .AddColumn(wordNameColumnStr)
                .Build();

            await _sqlConnection.ExecuteAsync(tableSchemaStr);

            var indexBuilder = new IndexBuilder();

            var wordIndexStr = indexBuilder.SetIfNotExist("Words", "WordName")
                .CreateIndex("Words", "WordName", IndexType.NonClustered)
                .Build();

            await _sqlConnection.ExecuteAsync(wordIndexStr);

            await _sqlConnection.CloseAsync();
        }

        private void CountWords()
        {
            if (Text == null)
            {
                WordCount = 0;
                return;
            }

            var wordCounter = new WordOperation();

            WordCount = wordCounter.GetWordCount(Text);
        }

        private void Close()
        {
            _eventAggregator.GetEvent<OnCloseWindowViewEvent>().Publish();
        }

        private void OnSaveChanges()
        {
            if (HasChanges)
            {
                var result =
                    _messageDialogService.ShowYesNoDialog("You've made changes. Do you want to save?", "Information");

                if (result == MessageDialogResult.Yes)
                {
                    Save();
                }
            }
            DeleteOldTempFile(_tmpFilePath);
        }

        private async void AutoSave()
        {
            if (_tmpFilePath == null)
                return;
            await SaveData(_tmpFilePath, Text);
        }

        private async void Save()
        {
            if(_filePath == null)
                _filePath = GetFilePath(_saveFileDialogServiceCreator, _filter);
            

            if (_filePath != null)
            {
                DeleteOldTempFile(_tmpFilePath);
                CreateTempFile();

                HasChanges = false;
                await SaveData(_filePath, Text);
            }
        }

        private void DeleteOldTempFile(string tmpFilePath)
        {
            File.Delete(tmpFilePath);
        }

        private async void Open()
        {
            _filePath = GetFilePath(_openFileDialogServiceCreator, _filter);

            if (_filePath != null)
            {
                DeleteOldTempFile(_tmpFilePath);
                CreateTempFile();

                Text = await ReadData(_filePath);
            }
        }

        private void Forward()
        {
            var snapshot = _snapshotCare.GetForwardSnapshot();

            if (snapshot != null)
                Text = snapshot.GetState();
        }

        private void Back()
        {
            var snapshot = _snapshotCare.GetBackSnapshot();

            if (snapshot != null)
                Text = snapshot.GetState();
        }

        private void CreateNewFile()
        {
            _filePath = GetFilePath(_saveFileDialogServiceCreator, _filter);

            if (_filePath != null)
            {
                DeleteOldTempFile(_tmpFilePath);
                CreateEmptyFile(_filePath);
                CreateTempFile();
            }
        }
        private string GetFilePath(Func<string, IFileDialogService> fileDialogServiceCreator, string filter)
        {
            var fileDialogService = fileDialogServiceCreator(filter);

            if (fileDialogService.ShowDialog() == false)
                return null;

            return fileDialogService.FileName;
        }

        private void CreateTempFile()
        {
            var directoryName = "tmp";

            CreateTempDirectory(directoryName);

            var fileName = ParseFileNameFromPath(_filePath);

            _tmpFilePath = string.IsNullOrWhiteSpace(fileName) ? $@"{directoryName}\{Guid.NewGuid() + ".txt"}" : $@"{directoryName}\{"tmp_" + fileName}";

            CreateEmptyFile(_tmpFilePath);

            File.SetAttributes(_tmpFilePath, File.GetAttributes(_tmpFilePath) | FileAttributes.Hidden);
        }

        private static void CreateTempDirectory(string directoryName)
        {
            DirectoryInfo directoryInfo = null;

            directoryInfo = !Directory.Exists(directoryName) ? Directory.CreateDirectory(directoryName) : new DirectoryInfo(directoryName);

            directoryInfo.Attributes |= FileAttributes.Hidden;

        }

        private void CreateEmptyFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var fs = File.Create(filePath);
            
            fs.Close();
        }
        private string ParseFileNameFromPath(string filePath)
        {
            return filePath?.Substring(filePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
        }

        private async Task SaveData(string filePath, string text)
        {
            File.SetAttributes(_tmpFilePath, FileAttributes.Normal);
            await File.WriteAllTextAsync(filePath, text);
            File.SetAttributes(_tmpFilePath, File.GetAttributes(_tmpFilePath) | FileAttributes.Hidden);
        }

        private async Task<string> ReadData(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}