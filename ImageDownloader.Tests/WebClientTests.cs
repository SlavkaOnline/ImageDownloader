using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ImageDownloader.Helpers;

namespace ImageDownloader.Tests
{
    [TestFixture]
    public class WebClientTests
    {
        [Test]
        public async Task ProgressAfterDowloadEqual100()
        {
            var uri = @"http://lifeandjoy.ru/uploads/posts/2015-07/1438256217_lifeandjoy.ru_02.jpg";
            var progress = 0.0;
            using (var client = new WebClient())
            {
                var bytes = await client.DownloadDataWithProgressAsync(uri, new Progress<double>(p => progress = p));
            }
            Assert.AreEqual(100.0, progress);
        }

        [Test]
        public void ThrowExceptionWithNullProgress()
        {
            using (var client = new WebClient())
            {
                Assert.That(async () => await client.DownloadDataWithProgressAsync("", null), Throws.TypeOf<ArgumentNullException>());
            }
        }

        [Test]
        public void WebClientIsNotBusyAfterCancel()
        {
            var uri = @"http://lifeandjoy.ru/uploads/posts/2015-07/1438256217_lifeandjoy.ru_02.jpg";
            var cancelationTokenSource = new CancellationTokenSource();
            using (var client = new WebClient())
            {
                var task = client.DownloadDataWithProgressAsync(uri, new Progress<double>(_ => { }), cancelationTokenSource.Token);
                Assert.That(client.IsBusy, Is.True);
                cancelationTokenSource.Cancel();
                Assert.That(client.IsBusy, Is.False);
                Assert.That(async () => await task, Throws.TypeOf<WebException>());
            }
        }
    }
}
