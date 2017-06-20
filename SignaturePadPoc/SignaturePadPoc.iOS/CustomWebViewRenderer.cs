using System.ComponentModel;
using Foundation;
using SignaturePadPoc;
using SignaturePadPoc.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace SignaturePadPoc.iOS
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }
            if (e.OldElement != null)
            {
                // Cleanup
            }
            if (e.NewElement == null)
            {
                return;
            }
            var customWebView = Element;
            LoadFile(customWebView.Uri);
            Control.ScalesPageToFit = true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CustomWebView.UriProperty.PropertyName)
            {
                LoadFile(Element?.Uri);
            }
        }

        private void LoadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            Control.LoadRequest(new NSUrlRequest(new NSUrl(path, false)));
        }
    }
}