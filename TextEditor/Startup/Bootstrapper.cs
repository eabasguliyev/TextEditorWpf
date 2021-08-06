using Autofac;
using Prism.Events;
using TextEditor.Snapshot;
using TextEditor.ViewModels;
using TextEditor.Views.Services;
using TextEditor.Views.Services.FileDialogServices;
using TextEditor.Views.Services.MessageDialogServices;

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
            containerBuilder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            containerBuilder.RegisterType<MessageDialogService>().As<IMessageDialogService>().SingleInstance();
            return containerBuilder.Build();
        }
    }
}