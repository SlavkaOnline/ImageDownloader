using ImageDownloader.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private ImageDownloaderViewModel _firtsImageDownloaderViewModel;
        public ImageDownloaderViewModel FirtsImageDownloaderViewModel
        {
            get => _firtsImageDownloaderViewModel;
            set => this.RaiseAndSetIfChanged(ref _firtsImageDownloaderViewModel, value);
        }

        private ImageDownloaderViewModel _secondImageDownloaderViewModel;
        public ImageDownloaderViewModel SecondImageDownloaderViewModel
        {
            get => _secondImageDownloaderViewModel;
            set => this.RaiseAndSetIfChanged(ref _secondImageDownloaderViewModel, value);
        }

        private ImageDownloaderViewModel _thirdImageDownloaderViewModel;
        public ImageDownloaderViewModel ThirdImageDownloaderViewModel
        {
            get => _thirdImageDownloaderViewModel;
            set => this.RaiseAndSetIfChanged(ref _thirdImageDownloaderViewModel, value);
        }

        private double _totalDownloadingProgress;
        public double TotalDownloadingProgress
        {
            get => _totalDownloadingProgress;
            private set => this.RaiseAndSetIfChanged(ref _totalDownloadingProgress, value);
        }

        public ReactiveCommand DownloadAllCommand { get; private set; }

        public MainViewModel(ImageDownloaderViewModel firtsImageDownloaderViewModel, ImageDownloaderViewModel secondImageDownloaderViewModel, ImageDownloaderViewModel thirdImageDownloaderViewModel)
        {
            FirtsImageDownloaderViewModel = firtsImageDownloaderViewModel;
            SecondImageDownloaderViewModel = secondImageDownloaderViewModel;
            ThirdImageDownloaderViewModel = thirdImageDownloaderViewModel;
            Subscribe();
            InitCommands();
        }

        private void InitCommands()
        {
            DownloadAllCommand = ReactiveCommand.CreateFromTask(() => DownloadAllAsync(),
                                                                this.WhenAnyValue(vm => vm.FirtsImageDownloaderViewModel.DownloadingState,
                                                                              vm => vm.SecondImageDownloaderViewModel.DownloadingState,
                                                                              vm => vm.ThirdImageDownloaderViewModel.DownloadingState,
                                                                              (f, s, t) =>
                                                                                FirtsImageDownloaderViewModel.DownloadingState != DownloadingState.Downloading
                                                                                && SecondImageDownloaderViewModel.DownloadingState != DownloadingState.Downloading
                                                                                && ThirdImageDownloaderViewModel.DownloadingState != DownloadingState.Downloading)
                                                                                .Merge(
                                                                                        this.WhenAnyValue(vm => vm.FirtsImageDownloaderViewModel.Url,
                                                                                                          vm => vm.SecondImageDownloaderViewModel.Url,
                                                                                                          vm => vm.ThirdImageDownloaderViewModel.Url,
                                                                                                            (f, s, t) =>
                                                                                                            !(string.IsNullOrEmpty(f) || string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
                                                                                                           )
                                                                                        )
                                                                  );
        }


        private void Subscribe()
        {

            this.WhenAnyValue(vm => vm.FirtsImageDownloaderViewModel.DownloadingProgress,
                              vm => vm.SecondImageDownloaderViewModel.DownloadingProgress,
                              vm => vm.ThirdImageDownloaderViewModel.DownloadingProgress)
                .Subscribe(p =>
                {
                    TotalDownloadingProgress = (p.Item1 + p.Item2 + p.Item3) / CalculateActiveDownloads();
                });
        }

        private int CalculateActiveDownloads()
        {

            var sum = (FirtsImageDownloaderViewModel.DownloadingState != DownloadingState.Idle ? 1 : 0)
                       + (SecondImageDownloaderViewModel.DownloadingState != DownloadingState.Idle ? 1 : 0)
                       + (ThirdImageDownloaderViewModel.DownloadingState != DownloadingState.Idle ? 1 : 0);

            return sum;
        }


        public async Task DownloadAllAsync()
        {
            FirtsImageDownloaderViewModel.ClearState();
            SecondImageDownloaderViewModel.ClearState();
            ThirdImageDownloaderViewModel.ClearState();
            await Task.WhenAll(FirtsImageDownloaderViewModel.StartDownloadAsync(),
                                SecondImageDownloaderViewModel.StartDownloadAsync(),
                                ThirdImageDownloaderViewModel.StartDownloadAsync());

          
        }


    }
}
