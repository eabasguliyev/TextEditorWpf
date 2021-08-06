using Autofac;
using TextEditor.ViewModels;
using TextEditor.Views.Services;

namespace TextEditor.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<MainWindow>().AsSelf();
            containerBuilder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();

            containerBuilder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>();
            containerBuilder.RegisterType<SaveFileDialogService>().As<ISaveFileDialogService>();
            return containerBuilder.Build();
        }
    }
}