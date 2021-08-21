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

        public MainWindow()
        {
            InitializeComponent();

            this.TextBox.SelectionChanged += TextBoxOnSelectionChanged;
        }

        private void TextBoxOnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var lineIndex = this.TextBox.GetLineIndexFromCharacterIndex(this.TextBox.SelectionStart) + 1;
            var lineLength = this.TextBox.GetLineLength(lineIndex - 1);
            var charIndex = this.TextBox.GetCharacterIndexFromLineIndex(lineIndex - 1) - this.TextBox.SelectionStart;

            var position = this.TextBox.SelectionStart + 1;
            var column = (lineLength - charIndex) / 2 + 1;
            
            //_viewModel.SetColumn(this.TextBox.GetLineLength() - this.TextBox.GetCharacterIndexFromLineIndex());

            ((IMainWindowViewModel)this.DataContext).SetEditorStatusData(position, lineIndex, column);
        }
        private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
