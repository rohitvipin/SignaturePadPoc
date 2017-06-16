using System;
using System.IO;
using SignaturePad.Forms;
using Xamarin.Forms;

namespace SignaturePadPoc
{
    public partial class SignaturePage : ContentPage
    {
        private readonly DocumentEntity _selectedDocument;

        public SignaturePage()
        {
            InitializeComponent();
        }

        public SignaturePage(DocumentEntity selectedDocument) : this()
        {
            _selectedDocument = selectedDocument;
            Title = _selectedDocument.Title;
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            using (var stream = await SignaturePad.GetImageStreamAsync(SignatureImageFormat.Png))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    _selectedDocument.SignatureBase64 = Convert.ToBase64String(ms.ToArray());
                }
            }

            await Navigation.PopToRootAsync(true);
        }
    }
}