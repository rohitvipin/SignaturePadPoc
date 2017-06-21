using SignaturePadPoc.Entities;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
{
    public partial class ViewSignaturePage : ContentPage
    {
        public ViewSignaturePage()
        {
            InitializeComponent();
        }

        public ViewSignaturePage(DocumentEntity selectedDocument) : this()
        {
            Title = selectedDocument.Title;
            WebView.Source = new HtmlWebViewSource
            {
                Html = $"<html><head><title>Page Title</title></head><body><img src='data:image/png; base64,{selectedDocument.SignatureBase64}' alt='Signature Missing' style='width:75%; height: 75%;'/></body></html>"
            };
        }
    }
}