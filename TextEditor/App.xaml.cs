using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Configuration;
using TextEditor.Startup;
using TextEditor.ViewModels;

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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("AppSettings.json", false).Build();

            container.Resolve<SqlConnection>().ConnectionString = configuration.GetConnectionString("Master");

            var window = container.Resolve<MainWindow>();

            window.DataContext = container.Resolve<IMainWindowViewModel>();

            window.Show();
        }
    }
}
