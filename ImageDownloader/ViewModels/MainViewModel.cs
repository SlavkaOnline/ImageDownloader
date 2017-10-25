using ImageDownloader.Enums;
using ImageDownloader.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class MainViewModel : ReactiveObject
    {

        #region property

        private ObservableCollection<ImageDownloaderViewModel> _imageDownloaderViewModels;
        public ObservableCollection<ImageDownloaderViewModel> ImageDownloaderViewModels
        {
            get => _imageDownloaderViewModels ?? (_imageDownloaderViewModels = new ObservableCollection<ImageDownloaderViewModel>());
            set => this.RaiseAndSetIfChanged(ref _imageDownloaderViewModels, value);
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

        public MainViewModel(IEnumerable<ImageDownloaderViewModel> imageDownloaderViewModels)
        {
            ImageDownloaderViewModels = new ObservableCollection<ImageDownloaderViewModel>(imageDownloaderViewModels);
            CountActiveDownloading = 0;
            TotalDownloadingProgress = 0;
            Subscribe();
            InitCommands();
        }

        private void InitCommands()
        {
            DownloadAllCommand = ReactiveCommand.CreateFromTask(() => DownloadAllAsync(),
                                                              this.WhenAnyValue(vm => vm.CountActiveDownloading, cout => cout == 0));
        }

        private void Subscribe()
        {

            foreach (var item in ImageDownloaderViewModels)
            {
                item.OnStarted.Subscribe(_ => CountActiveDownloading++);
                item.OnStoped.Subscribe(_ => CountActiveDownloading--);

                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "DownloadingProgress")
                    {
                        TotalDownloadingProgress = ImageDownloaderViewModels.Sum(it => it.DownloadingProgress) / (CountActiveDownloading != 0 ? CountActiveDownloading : 1);
                    }
                };

            }

            this.WhenAnyValue(vm => vm.TotalDownloadingProgress)
                .Where(p => Math.Abs(p - 100.0) < 0.1)
                .Subscribe(_ =>
                {
                    CountActiveDownloading = 0;
                    foreach (var item in ImageDownloaderViewModels)
                    {
                        item.ClearState();
                    }
                    TotalDownloadingProgress = -1;
                });
        }

        public async Task DownloadAllAsync()
        {
            CountActiveDownloading = 0;
            var downloadsList = new List<Task>();
            foreach (var item in ImageDownloaderViewModels)
            {
                downloadsList.Add(item.StartDownloadAsync());
            }
            await Task.WhenAll(downloadsList);
        }
    }
}
