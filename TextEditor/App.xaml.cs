using System.Windows;
using Autofac;
using TextEditor.Startup;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var window = container.Resolve<MainWindow>();
            
            window.Show();
        }
    }
}
