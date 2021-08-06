using Autofac;
using TextEditor.Snapshot;
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

            containerBuilder.RegisterGeneric(typeof(SnapshotCare<>)).As(typeof(ISnapshotCare<>));

            return containerBuilder.Build();
        }
    }
}