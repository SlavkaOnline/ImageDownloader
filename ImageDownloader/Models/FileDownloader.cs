using ImageDownloader.Helpers;
using ImageDownloader.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageDownloader.Models
{
    public class FileDownloader : IFileDownloader
    {
        private CancellationTokenSource _cancelationTokenSource;

        public async Task<byte[]> Download(string Url, Action<double> progress)
        {
            try
            {
                _cancelationTokenSource = new CancellationTokenSource();
                var bytes = new byte[0];
                using (var webClient = new WebClient())
                {
                    bytes = await webClient.DownloadDataWithProgressAsync(Url, new Progress<double>(progress), _cancelationTokenSource.Token);
                }
                return bytes;
            }
            catch(WebException e)
            {
                if (!_cancelationTokenSource.IsCancellationRequested)
                    throw;
            }
            return new byte[0];
        }

        public void Cancel()
        {
            if (_cancelationTokenSource is null)
                throw new InvalidOperationException("Загрузка не была запущена");
            _cancelationTokenSource.Cancel();
        }
    }
}

