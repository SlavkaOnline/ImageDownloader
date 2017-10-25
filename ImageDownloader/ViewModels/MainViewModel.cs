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
        #region property

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

        private int _countActiveDownloading;
        private int CountActiveDownloading
        {
            get => _countActiveDownloading;
            set => this.RaiseAndSetIfChanged(ref _countActiveDownloading, value);
        }

        #endregion

        #region commands

        public ReactiveCommand DownloadAllCommand { get; private set; }

        #endregion

        public MainViewModel(ImageDownloaderViewModel firtsImageDownloaderViewModel, ImageDownloaderViewModel secondImageDownloaderViewModel, ImageDownloaderViewModel thirdImageDownloaderViewModel)
        {
            FirtsImageDownloaderViewModel = firtsImageDownloaderViewModel;
            SecondImageDownloaderViewModel = secondImageDownloaderViewModel;
            ThirdImageDownloaderViewModel = thirdImageDownloaderViewModel;
            CountActiveDownloading = 0;
            TotalDownloadingProgress = 0;
            Subscribe();
            InitCommands();
        }

        private void InitCommands()
        {
            DownloadAllCommand = ReactiveCommand.CreateFromTask(() => DownloadAllAsync(),
                                                                this.WhenAnyValue(vm => vm.CountActiveDownloading,
                                                                                  vm => vm.FirtsImageDownloaderViewModel.Url,
                                                                                  vm => vm.SecondImageDownloaderViewModel.Url,
                                                                                  vm => vm.ThirdImageDownloaderViewModel.Url,
                                                                                  (c, f, s, t) => !(string.IsNullOrEmpty(f) || string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) && (c == 0)));
        }

        private void Subscribe()
        {
            FirtsImageDownloaderViewModel.OnStarted.Subscribe(_ => CountActiveDownloading++);
            FirtsImageDownloaderViewModel.OnStoped.Subscribe(_ => CountActiveDownloading--);
            SecondImageDownloaderViewModel.OnStarted.Subscribe(_ => CountActiveDownloading++);
            SecondImageDownloaderViewModel.OnStoped.Subscribe(_ => CountActiveDownloading--);
            ThirdImageDownloaderViewModel.OnStarted.Subscribe(_ => CountActiveDownloading++);
            ThirdImageDownloaderViewModel.OnStoped.Subscribe(_ => CountActiveDownloading--);

            this.WhenAnyValue(vm => vm.FirtsImageDownloaderViewModel.DownloadingProgress,
                              vm => vm.SecondImageDownloaderViewModel.DownloadingProgress,
                              vm => vm.ThirdImageDownloaderViewModel.DownloadingProgress)
                .Subscribe(p =>
                {
                    TotalDownloadingProgress = (p.Item1 + p.Item2 + p.Item3) / (_countActiveDownloading != 0 ? _countActiveDownloading : 1);
                });

            this.WhenAnyValue(vm => vm.TotalDownloadingProgress)
                .Where(p => Math.Abs(p - 100.0) < 0.1)
                .Subscribe(_ =>
                {
                    CountActiveDownloading = 0;
                    FirtsImageDownloaderViewModel.ClearState();
                    SecondImageDownloaderViewModel.ClearState();
                    ThirdImageDownloaderViewModel.ClearState();
                    TotalDownloadingProgress = -1;
                });
        }

        public async Task DownloadAllAsync()
        {
            CountActiveDownloading = 0;
            await Task.WhenAll(FirtsImageDownloaderViewModel.StartDownloadAsync(),
                                SecondImageDownloaderViewModel.StartDownloadAsync(),
                                ThirdImageDownloaderViewModel.StartDownloadAsync());
        }
    }
}
