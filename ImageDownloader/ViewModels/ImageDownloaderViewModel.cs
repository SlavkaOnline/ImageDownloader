using ImageDownloader.Enums;
using ImageDownloader.Helpers;
using ImageDownloader.Models;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace ImageDownloader.ViewModels
{
    public class ImageDownloaderViewModel : ReactiveObject
    {
        private BitmapImage _sourceImage;
        public BitmapImage SourceImage
        {
            get => _sourceImage;
            set => this.RaiseAndSetIfChanged(ref _sourceImage, value);
        }

        private double _downloadingProgress;
        public double DownloadingProgress
        {
            get => _downloadingProgress;
            set => this.RaiseAndSetIfChanged(ref _downloadingProgress, value);
        }

        private string _url;
        public string Url
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        private DownloadingState _downloadingState;
        public DownloadingState DownloadingState
        {
            get => _downloadingState;
            set => this.RaiseAndSetIfChanged(ref _downloadingState, value);
        }

        private FileDownloader _fileDownloader;


        public ReactiveCommand StartDownloadCommand { get; private set; }
        public ReactiveCommand StopDownloadCommand { get; private set; }

        public ImageDownloaderViewModel()
        {
            SourceImage = null;
            _fileDownloader = new FileDownloader();
            DownloadingState = DownloadingState.Idle;
            Subscribe();
            InitCommands();
        }

        private void Subscribe()
        {
            this.WhenAnyValue(vm => vm.DownloadingState, state => state == DownloadingState.Idle)
                .Subscribe(isIdle =>
                {
                    if (isIdle)
                    {
                        DownloadingProgress = 0.0;
                    }
                });
        }

        private void InitCommands()
        {
            StartDownloadCommand = ReactiveCommand.CreateFromTask(() => StartDownloadAsync(),
                                                                   this.WhenAnyValue(vm => vm.DownloadingState,
                                                                    state => state != DownloadingState.Downloading)
                                                                    .Merge(
                                                                    this.WhenAnyValue(vm => vm.Url,
                                                                                     url => !string.IsNullOrEmpty(url))));
            StopDownloadCommand = ReactiveCommand.Create(() => StopDownload(),
                                                        this.WhenAnyValue(vm => vm.DownloadingState,
                                                        state => state == DownloadingState.Downloading));
        }

        public async Task StartDownloadAsync()
        {
            DownloadingState = DownloadingState.Downloading;
            var dataBytes = await _fileDownloader.Download(Url, p => DownloadingProgress = p);
            if (dataBytes.Length != 0)
            {
                SourceImage = Converter.FromBytesToImage(dataBytes);
            }
            DownloadingState = DownloadingState.Downloaded;
        }

        public void StopDownload()
        {
            _fileDownloader.Cancel();
            DownloadingState = DownloadingState.Idle;
            DownloadingProgress = 0.0;
        }
    }
}
