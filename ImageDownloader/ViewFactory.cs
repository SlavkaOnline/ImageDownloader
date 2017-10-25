using ImageDownloader.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader
{
    public class ViewFactory : IViewFactory
    {
        public Task CreateExceptionViewDialogAsync(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        public Task CreateInfoViewDialog(string message)
        {
            throw new NotImplementedException();
        }
    }
}
