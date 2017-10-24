using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ReactiveUI;
using ImageDownloader.ViewModels;
using ImageDownloader.Enums;

namespace ImageDownloader.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {

        [Test]
        public void CalculateTotalProgressWhenAllDownloading()
        {
            var firstProgress = 33.4;
            var secondProgress = 45.6;
            var thirdProgress = 32.6;
            var expectedProgress = (firstProgress + secondProgress + thirdProgress) / 3;

            var firstImageDownloaderViewModel = new ImageDownloaderViewModel();
            var secondImageDownloaderViewModel = new ImageDownloaderViewModel();
            var thirdtImageDownloaderViewModel = new ImageDownloaderViewModel();

            var mainViewModel = new MainViewModel(firstImageDownloaderViewModel,
                                                  secondImageDownloaderViewModel,
                                                  thirdtImageDownloaderViewModel);

            firstImageDownloaderViewModel.DownloadingState = DownloadingState.Downloading;
            secondImageDownloaderViewModel.DownloadingState = DownloadingState.Downloading;
            thirdtImageDownloaderViewModel.DownloadingState = DownloadingState.Downloading;

            firstImageDownloaderViewModel.DownloadingProgress = firstProgress;
            secondImageDownloaderViewModel.DownloadingProgress = secondProgress;
            thirdtImageDownloaderViewModel.DownloadingProgress = thirdProgress;

            Assert.AreEqual(expectedProgress, mainViewModel.TotalDownloadingProgress, 2);
        }
    }
}
