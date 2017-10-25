using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels.Interfaces
{
    public interface IViewFactory
    {
        Task CreateExceptionViewDialogAsync(string message);
    }
}
