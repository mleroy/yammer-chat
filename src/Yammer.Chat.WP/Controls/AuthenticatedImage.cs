using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Yammer.Chat.Core;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Windows.Media;
using Yammer.Chat.Core.API;

namespace Yammer.Chat.WP.Controls
{
    [TemplatePart(Name = imagePartName, Type = typeof(Image))]
    public class AuthenticatedImage : Control
    {
        private const string imagePartName = "PART_Image";

        private Image image;

        #region Dependency properties

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(AuthenticatedImage), new PropertyMetadata(SourceChanged));

        public IHttpService HttpService
        {
            get { return (IHttpService)GetValue(HttpServiceProperty); }
            set { SetValue(HttpServiceProperty, value); }
        }

        public static readonly DependencyProperty HttpServiceProperty =
            DependencyProperty.Register("HttpService", typeof(IHttpService), typeof(AuthenticatedImage), new PropertyMetadata(null));

        public int DecodePixelWidth
        {
            get { return (int)GetValue(DecodePixelWidthProperty); }
            set { SetValue(DecodePixelWidthProperty, value); }
        }

        public static readonly DependencyProperty DecodePixelWidthProperty =
            DependencyProperty.Register("DecodePixelWidth", typeof(int), typeof(AuthenticatedImage), new PropertyMetadata(0));

        #endregion

        private static async void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await ((AuthenticatedImage)d).SetSource(e.NewValue as Uri);
        }

        public AuthenticatedImage()
        {
            DefaultStyleKey = typeof(AuthenticatedImage);
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.image = GetTemplateChild(imagePartName) as Image;

            await this.SetSource(this.Source);
        }

        private async Task SetSource(Uri uri)
        {
            if (this.image == null || uri == null || this.HttpService == null)
            {
                return;
            }

            try
            {
                using (var stream = await DownloadImage(uri))
                {
                    await SetImageSource(stream);
                }
            }
            catch (Exception)
            {
                this.image.Source = null;
            }
        }

        private async Task<Stream> DownloadImage(Uri uri)
        {
            var response = await this.HttpService.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

            return await response.Content.ReadAsStreamAsync();
        }

        private Task SetImageSource(Stream stream)
        {
            var tcs = new TaskCompletionSource<object>();

            var bitmapImage = new BitmapImage()
            {
                CreateOptions = BitmapCreateOptions.BackgroundCreation,
            };

            if (this.DecodePixelWidth > 0)
            {
                bitmapImage.DecodePixelWidth = this.DecodePixelWidth;
            }

            bitmapImage.ImageFailed += (s, e) => { tcs.SetException(e.ErrorException); };
            bitmapImage.ImageOpened += (s, e) => { tcs.SetResult(null); };

            bitmapImage.SetSource(stream);

            this.image.Source = bitmapImage;

            return tcs.Task;
        }
    }
}
