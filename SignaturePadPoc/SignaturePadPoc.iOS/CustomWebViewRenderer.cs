using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private const string AppScheme = "SignaturePadPoc_app_scheme:print";

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

            Control.ShouldStartLoad += (view, request, type) =>
            {
                if (string.IsNullOrWhiteSpace(Element?.Uri))
                {
                    return true;
                }

                if (!request.Url.AbsoluteString.Contains(AppScheme))
                {
                    return true;
                }

                var printInfo = UIPrintInfo.PrintInfo;
                var printer = UIPrintInteractionController.SharedPrintController;
                printInfo.Duplex = UIPrintInfoDuplex.LongEdge;
                printInfo.OutputType = UIPrintInfoOutputType.General;
                printer.PrintInfo = printInfo;
                printer.PrintingItem = new NSUrl(Element?.Uri, false);
                printer.ShowsPageRange = false;
                printer.Present(true, (controller, completed, error) =>
                {
                    Debug.WriteLine(completed ? "Printing completed" : $"Printing did not complete : {controller} {error}");
                });

                return false;
            };
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

            Control.LoadRequest(new NSUrlRequest(new NSUrl(Path.Combine(NSBundle.MainBundle.BundlePath, "Content/pdfjs/web/viewer.html"), false)));
            Control.LoadFinished += Control_LoadFinished;
        }

        private void Control_LoadFinished(object sender, System.EventArgs e)
        {
            Control.LoadFinished -= Control_LoadFinished;
            Control.EvaluateJavascript($"DEFAULT_URL='{Element?.Uri}'; window.location.href='{Path.Combine(NSBundle.MainBundle.BundlePath, "Content/pdfjs/web/viewer.html")}?file=file://{Element?.Uri}'; ");
        }
    }
}