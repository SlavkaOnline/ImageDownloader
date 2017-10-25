using ImageDownloader.Enums;
using ImageDownloader.Helpers;
using ImageDownloader.Models;
using ImageDownloader.Models.Interfaces;
using ImageDownloader.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace ImageDownloader.ViewModels
{
    public class ImageDownloaderViewModel : ReactiveObject
    {

        private readonly IFileDownloader _fileDownloader;
        private readonly IViewFactory _viewFactory;



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
            private set => this.RaiseAndSetIfChanged(ref _downloadingProgress, value);
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
            private set => this.RaiseAndSetIfChanged(ref _downloadingState, value);
        }

       


        public ReactiveCommand StartDownloadCommand { get; private set; }
        public ReactiveCommand StopDownloadCommand { get; private set; }

        public ImageDownloaderViewModel(IFileDownloader fileDownloader, IViewFactory viewFactory)
        {
            SourceImage = null;
            _fileDownloader = fileDownloader;
            _viewFactory = viewFactory;
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
            try
            {
                DownloadingState = DownloadingState.Downloading;
                var dataBytes = await _fileDownloader.Download(Url, p => DownloadingProgress = p);
                if (dataBytes.Length != 0)
                {
                    SourceImage = Converter.FromBytesToImage(dataBytes);
                }
                DownloadingState = DownloadingState.Downloaded;
            }
            catch(WebException e)
            {
                await _viewFactory.CreateExceptionViewDialogAsync(e.Message);
                DownloadingState = DownloadingState.Idle;
            }
        }

        public void StopDownload()
        {
            _fileDownloader.Cancel();
            DownloadingState = DownloadingState.Idle;
            DownloadingProgress = 0.0;
        }

        public void ClearState()
        {
            DownloadingState = DownloadingState.Idle;
            DownloadingProgress = 0;
        }
    }
}
