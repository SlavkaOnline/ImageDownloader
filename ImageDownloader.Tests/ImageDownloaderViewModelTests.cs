using ImageDownloader.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Tests
{
    [TestFixture]
    public class ImageDownloaderViewModelTests
    {
        [Test]
        public async Task UpdateSourceImageAfterDownloading()
        {
            var imageDownloaderViewModel = new ImageDownloaderViewModel();
            imageDownloaderViewModel.Url = @"http://lifeandjoy.ru/uploads/posts/2015-07/1438256217_lifeandjoy.ru_02.jpg";
            await imageDownloaderViewModel.StartDownloadAsync();
            Assert.IsNotNull(imageDownloaderViewModel.SourceImage);
        }

    }
}
