using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Prism.Events;
using TextEditor.Events;
using TextEditor.ViewModels;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMainWindowViewModel _viewModel;
        private readonly IEventAggregator _eventAggregator;

        public MainWindow(IMainWindowViewModel viewModel, IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _eventAggregator = eventAggregator;

            this.DataContext = _viewModel;

            this.Loaded += OnLoaded;

            this.TextBox.TextChanged += TextBoxOnTextChanged;
            this.TextBox.SelectionChanged += TextBoxOnSelectionChanged;

            _eventAggregator.GetEvent<OnCloseWindowViewEvent>().Subscribe(OnCloseWindowView);
        }

        private void TextBoxOnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var lineIndex = this.TextBox.GetLineIndexFromCharacterIndex(this.TextBox.SelectionStart) + 1;
            var lineLength = this.TextBox.GetLineLength(lineIndex - 1);
            var charIndex = this.TextBox.GetCharacterIndexFromLineIndex(lineIndex - 1) - this.TextBox.SelectionStart;

            _viewModel.SetPosition(this.TextBox.SelectionStart + 1);
            _viewModel.SetLine(lineIndex);
            //_viewModel.SetColumn(this.TextBox.GetLineLength() - this.TextBox.GetCharacterIndexFromLineIndex());
            _viewModel.SetColumn((lineLength - charIndex) / 2 + 1);
        }

        private void OnCloseWindowView()
        {
            this.Close();
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnTextChanged();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _eventAggregator.GetEvent<OnSaveChangesEvent>().Publish();
        }
    }
}
