using Autofac;
using ImageDownloader.Models;
using ImageDownloader.Models.Interfaces;
using ImageDownloader.ViewModels;
using ImageDownloader.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var builder = new ContainerBuilder();
            builder.RegisterType<FileDownloader>().As<IFileDownloader>();
            builder.RegisterType<ViewFactory>().As<IViewFactory>();

            builder.RegisterType<ImageDownloaderViewModel>();
            builder.Register(c => new List<ImageDownloaderViewModel>()
            {
                c.Resolve<ImageDownloaderViewModel>(),
                c.Resolve<ImageDownloaderViewModel>(),
                c.Resolve<ImageDownloaderViewModel>()
            }).As<IEnumerable<ImageDownloaderViewModel>>();

            builder.RegisterType<MainViewModel>();
            var container = builder.Build();

            var mainWindow = new MainWindow()
            {
                DataContext = container.Resolve<MainViewModel>()
            };
            mainWindow.Show();
        }
    }
}
