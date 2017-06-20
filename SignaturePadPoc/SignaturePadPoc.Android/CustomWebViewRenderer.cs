using System.ComponentModel;
using SignaturePadPoc;
using SignaturePadPoc.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace SignaturePadPoc.Droid
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                return;
            }

            Control.Settings.AllowUniversalAccessFromFileURLs = true;
            LoadFile((Element as CustomWebView)?.Uri);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomWebView.UriProperty.PropertyName)
            {
                LoadFile((Element as CustomWebView)?.Uri);
            }
        }

        private void LoadFile(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return;
            }
            Control.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file=file://{uri}");
        }
    }
}