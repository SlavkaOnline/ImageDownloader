using ImageDownloader.Models;
using ImageDownloader.Models.Interfaces;
using ImageDownloader.ViewModels;
using ImageDownloader.ViewModels.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var imageDownloaderViewModel = new ImageDownloaderViewModel(new FileDownloader(), new ViewFactory());
            imageDownloaderViewModel.Url = @"http://lifeandjoy.ru/uploads/posts/2015-07/1438256217_lifeandjoy.ru_02.jpg";
            await imageDownloaderViewModel.StartDownloadAsync();
            Assert.IsNotNull(imageDownloaderViewModel.SourceImage);
        }
        
        [Test]
        public async Task CreateExceptionMessageWhenDownloadFailed()
        {
            var mockFileDownLoader = Substitute.For<IFileDownloader>();
            mockFileDownLoader
                .Download(Arg.Any<string>(), Arg.Any<Action<double>>())
                .Throws(new WebException());

            var isCReated = false;
            var mockViewFactory = Substitute.For<IViewFactory>();
            mockViewFactory
                .When(x => x.CreateExceptionViewDialogAsync(Arg.Any<string>()))
                .Do( _ => isCReated = true);

            var imageDownloaderViewModel = new ImageDownloaderViewModel(mockFileDownLoader, mockViewFactory);
            await imageDownloaderViewModel.StartDownloadAsync();

            Assert.That(isCReated, Is.True);
        }

    }
}
