using SignaturePadPoc.Entities;
using Xamarin.Forms;
using System.IO;
using System;

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
            Image.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(selectedDocument.SignatureBase64)));
        }
    }
}