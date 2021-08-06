using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using TextEditor.Views.Services;

namespace TextEditor.ViewModels
{
    public class MainWindowViewModel:ObservableObject, IMainWindowViewModel
    {
        private readonly Func<string, ISaveFileDialogService> _saveFileDialogServiceCreator;
        private readonly Func<string, IOpenFileDialogService> _openFileDialogServiceCreator;
        private string _text;
        private string _filter;
        public MainWindowViewModel(Func<string,ISaveFileDialogService> saveFileDialogServiceCreator, 
            Func<string, IOpenFileDialogService> openFileDialogServiceCreator)
        {
            _saveFileDialogServiceCreator = saveFileDialogServiceCreator;
            _openFileDialogServiceCreator = openFileDialogServiceCreator;
            _filter = "Text files (*.txt)|*.txt";

            SaveCommand = new DelegateCommand(Save);
            OpenCommand = new DelegateCommand(Open);
            NewFileCommand = new DelegateCommand(CreateNewFile);
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

        public string FilePath { get; set; }
        public string TmpFilePath { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand NewFileCommand { get;}


        public async void LoadAsync()
        {
            await Task.Run(CreateTempFile);
        }

        public async void AutoSave()
        {
            await SaveData(TmpFilePath, Text);
        }

        private async void Save()
        {
            FilePath = GetFilePath(_saveFileDialogServiceCreator, _filter);

            if (FilePath != null)
            {
                CreateTempFile();

                await SaveData(FilePath, Text);
            }
        }

        private async void Open()
        {
            FilePath = GetFilePath(_openFileDialogServiceCreator, _filter);

            if (FilePath != null)
            {
                CreateTempFile();

                Text = await ReadData(FilePath);
            }
        }

        private void CreateNewFile()
        {
            FilePath = GetFilePath(_saveFileDialogServiceCreator, _filter);

            if (FilePath != null)
            {
                CreateEmptyFile(FilePath);
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

            var fileName = ParseFileNameFromPath(FilePath);

            TmpFilePath = string.IsNullOrWhiteSpace(fileName) ? $@"{directoryName}\{Guid.NewGuid() + ".txt"}" : $@"{directoryName}\{"tmp_" + fileName}";

            CreateEmptyFile(TmpFilePath);

            File.SetAttributes(TmpFilePath, File.GetAttributes(TmpFilePath) | FileAttributes.Hidden);
        }

        private static void CreateTempDirectory(string directoryName)
        {
            DirectoryInfo directoryInfo = null;

            directoryInfo = !Directory.Exists(directoryName) ? Directory.CreateDirectory(directoryName) : new DirectoryInfo(directoryName);

            directoryInfo.Attributes |= FileAttributes.Hidden;

        }

        private void CreateEmptyFile(string filePath)
        {
            var fs = File.Create(filePath);
            
            fs.Close();
        }
        private string ParseFileNameFromPath(string filePath)
        {
            return filePath?.Substring(filePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
        }

        private async Task SaveData(string filePath, string text)
        {
            await File.WriteAllTextAsync(filePath, text);
        }

        private async Task<string> ReadData(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}