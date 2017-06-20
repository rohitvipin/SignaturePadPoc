using System.IO;
using System.Net;
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
            Control.LoadRequest(new NSUrlRequest(new NSUrl(Path.Combine(NSBundle.MainBundle.BundlePath, $"Content/{WebUtility.UrlEncode(customWebView.Uri)}"), false)));
            Control.ScalesPageToFit = true;
        }
    }
}