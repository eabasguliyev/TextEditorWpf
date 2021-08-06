using System.Windows;
using System.Windows.Controls;
using TextEditor.ViewModels;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMainWindowViewModel _viewModel;

        public MainWindow(IMainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            this.DataContext = _viewModel;

            this.Loaded += OnLoaded;

            this.TextBox.TextChanged += TextBoxOnTextChanged;
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.OnTextChanged();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadAsync();
        }
    }
}
