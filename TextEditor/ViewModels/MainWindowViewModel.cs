﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using TextEditor.Enums;
using TextEditor.Events;
using TextEditor.Snapshot;
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
        private string _text;
        private readonly string _filter;

        private string _filePath;
        private string _tmpFilePath;
        private int _wordCount;

        public MainWindowViewModel(ISnapshotCare<string> snapshotCare, Func<string, ISaveFileDialogService> saveFileDialogServiceCreator, 
            Func<string, IOpenFileDialogService> openFileDialogServiceCreator, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            _snapshotCare = snapshotCare;
            _saveFileDialogServiceCreator = saveFileDialogServiceCreator;
            _openFileDialogServiceCreator = openFileDialogServiceCreator;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

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

        public async void LoadAsync()
        {
            _snapshotCare.CreateSnapshot("");

            await Task.Run(CreateTempFile);
        }

        private void CountWords()
        {
            if (Text == null)
            {
                WordCount = 0;
                return;
            }

            WordCount = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Count(w =>
            {
                if (w.Length == 1 && (char.IsDigit(w[0]) || char.IsSeparator(w[0]) ||
                                      char.IsNumber(w[0]) || char.IsDigit(w[0]) ||
                                      char.IsPunctuation(w[0]) || char.IsSymbol(w[0])))
                    return false;

                return true;
            });
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
                    SaveCommand.Execute(null);
                }
                else
                {
                    DeleteOldTempFile(_tmpFilePath);
                }
            }
        }

        private async void AutoSave()
        {
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