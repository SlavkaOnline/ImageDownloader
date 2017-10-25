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
using ImageDownloader.Models;
using NSubstitute;
using ImageDownloader.Models.Interfaces;
using ImageDownloader.Tests.CaseData;

namespace ImageDownloader.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {

        [Test, TestCaseSource(typeof(MainViewModelTestDataCase), "TestCasesForThreeDownoading")]     
        public async Task<double> CalculateTotalProgressWhenAllDownloading(double firstProgress, double secondProgress, double thirdProgress)
        {
            var mockfirtFileDownloader = Substitute.For<IFileDownloader>();
            mockfirtFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(firstProgress));

            var mockSecondFileDownloader = Substitute.For<IFileDownloader>();
            mockSecondFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(secondProgress));

            var mockThirdFileDownloader = Substitute.For<IFileDownloader>();
            mockThirdFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(thirdProgress));

            var firstImageDownloaderViewModel = new ImageDownloaderViewModel(mockfirtFileDownloader, new ViewFactory());
            var secondImageDownloaderViewModel = new ImageDownloaderViewModel(mockSecondFileDownloader, new ViewFactory());
            var thirdImageDownloaderViewModel = new ImageDownloaderViewModel(mockThirdFileDownloader, new ViewFactory());

            var mainViewModel = new MainViewModel(firstImageDownloaderViewModel,
                                                  secondImageDownloaderViewModel,
                                                  thirdImageDownloaderViewModel);
            await mainViewModel.DownloadAllAsync();
            return mainViewModel.TotalDownloadingProgress;
        }

        [Test, TestCaseSource(typeof(MainViewModelTestDataCase), "TestCasesForTwoDownoading")]
        public async Task<double> CalculateTotalProgressWhenTwoDownloading(double firstProgress, double secondProgress)
        {

            var mockfirtFileDownloader = Substitute.For<IFileDownloader>();
            mockfirtFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(firstProgress));

            var mockSecondFileDownloader = Substitute.For<IFileDownloader>();
            mockSecondFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(secondProgress));


            var firstImageDownloaderViewModel = new ImageDownloaderViewModel(mockfirtFileDownloader, new ViewFactory());
            var secondImageDownloaderViewModel = new ImageDownloaderViewModel(mockSecondFileDownloader, new ViewFactory());
            var mainViewModel = new MainViewModel(firstImageDownloaderViewModel,
                                                  secondImageDownloaderViewModel,
                                                  new ImageDownloaderViewModel(new FileDownloader(), new ViewFactory()));
            await Task.WhenAll(firstImageDownloaderViewModel.StartDownloadAsync(), secondImageDownloaderViewModel.StartDownloadAsync());

            return mainViewModel.TotalDownloadingProgress;
        }

        [Test]
        public async Task ClearStatesDownloadingWhenAllComlited()
        {
            var percentage = 100.0d;
            var mockfirtFileDownloader = Substitute.For<IFileDownloader>();
            mockfirtFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(percentage));

            var mockSecondFileDownloader = Substitute.For<IFileDownloader>();
            mockSecondFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(percentage));

            var mockThirdFileDownloader = Substitute.For<IFileDownloader>();
            mockThirdFileDownloader
                .When(x => x.Download(Arg.Any<string>(), Arg.Any<Action<double>>()))
                .Do(x => x.Arg<Action<double>>().Invoke(percentage));

            var firstImageDownloaderViewModel = new ImageDownloaderViewModel(mockfirtFileDownloader, new ViewFactory());
            var secondImageDownloaderViewModel = new ImageDownloaderViewModel(mockSecondFileDownloader, new ViewFactory());
            var thirdImageDownloaderViewModel = new ImageDownloaderViewModel(mockThirdFileDownloader, new ViewFactory());

            var mainViewModel = new MainViewModel(firstImageDownloaderViewModel,
                                                  secondImageDownloaderViewModel,
                                                  thirdImageDownloaderViewModel);
            await mainViewModel.DownloadAllAsync();

            Assert.AreEqual(30.00, mainViewModel.TotalDownloadingProgress, 2);
            //Assert.AreEqual(DownloadingState.Idle, firstImageDownloaderViewModel.DownloadingState);
            //Assert.AreEqual(DownloadingState.Idle, secondImageDownloaderViewModel.DownloadingState);
            //Assert.AreEqual(DownloadingState.Idle, thirdImageDownloaderViewModel.DownloadingState);
        }

    }
}
