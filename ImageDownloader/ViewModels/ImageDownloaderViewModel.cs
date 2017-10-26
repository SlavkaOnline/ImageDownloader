using ImageDownloader.Enums;
using ImageDownloader.Helpers;
using ImageDownloader.Models;
using ImageDownloader.Models.Interfaces;
using ImageDownloader.ViewModels.Interfaces;
using ReactiveUI;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace ImageDownloader.ViewModels
{
    public class ImageDownloaderViewModel : ReactiveObject
    {

        private readonly IFileDownloader _fileDownloader;
        private readonly IViewFactory _viewFactory;

        #region property
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
        private DownloadingState DownloadingState
        {
            get => _downloadingState;
            set => this.RaiseAndSetIfChanged(ref _downloadingState, value);
        }

        private Subject<object> _onStarted;
        public IObservable<object> OnStarted => _onStarted.AsObservable();

        private Subject<object> _onStoped;
        public IObservable<object> OnStoped => _onStoped.AsObservable();
        #endregion


        #region commands

        public ReactiveCommand StartDownloadCommand { get; private set; }
        public ReactiveCommand StopDownloadCommand { get; private set; }

        #endregion

        public ImageDownloaderViewModel(IFileDownloader fileDownloader, IViewFactory viewFactory)
        {
            SourceImage = null;
            DownloadingState = DownloadingState.Idle;
            _fileDownloader = fileDownloader;
            _viewFactory = viewFactory;
            _onStarted = new Subject<object>();
            _onStoped = new Subject<object>();
            InitCommands();
        }

        private void InitCommands()
        {
            StartDownloadCommand = ReactiveCommand.CreateFromTask(() => StartDownloadAsync(),
                                                                   this.WhenAnyValue(vm => vm.DownloadingState,
                                                                    state => state != DownloadingState.Downloading ));

            StopDownloadCommand = ReactiveCommand.Create(() => StopDownload(),
                                                        this.WhenAnyValue(vm => vm.DownloadingState,
                                                        state => state == DownloadingState.Downloading));
        }

        public async Task StartDownloadAsync()
        {
            try
            {
                _onStarted.OnNext(null);
                DownloadingState = DownloadingState.Downloading;
                SourceImage = null;
                var dataBytes = await _fileDownloader.Download(Url, p => DownloadingProgress = p);
                if (dataBytes.Length != 0)
                {
                    SourceImage = Converter.FromBytesToImage(dataBytes);
                }
            }
            catch (WebException e)
            {
                _onStoped.OnNext(null);
                await _viewFactory.CreateExceptionViewDialogAsync(e.Message);
            }
            catch (ArgumentNullException e)
            {
                _onStoped.OnNext(null);
                await _viewFactory.CreateExceptionViewDialogAsync("Введите URL");
            }
            finally
            {
                DownloadingState = DownloadingState.Idle;
            }
        }

        public void StopDownload()
        {
            _fileDownloader.Cancel();
            _onStoped.OnNext(null);
            DownloadingProgress = 0.0;
        }

        public void ClearState()
        {
            DownloadingState = DownloadingState.Idle;
            DownloadingProgress = 0;
        }
    }
}
