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


            _eventAggregator.GetEvent<OnCloseWindowViewEvent>().Subscribe(OnCloseWindowView);
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
