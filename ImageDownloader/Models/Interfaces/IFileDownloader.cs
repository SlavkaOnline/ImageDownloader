using System;
using System.Threading.Tasks;

namespace ImageDownloader.Models.Interfaces
{
    public interface IFileDownloader
    {
        void Cancel();
        Task<byte[]> Download(string Url, Action<double> progress);
    }
}