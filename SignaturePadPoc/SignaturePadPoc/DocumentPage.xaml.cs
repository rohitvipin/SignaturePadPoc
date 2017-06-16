using System;
using Xamarin.Forms;

namespace SignaturePadPoc
{
    public partial class DocumentPage : ContentPage
    {
        private DocumentEntity _selectedDocument;


        public DocumentPage()
        {
            InitializeComponent();
        }

        public DocumentPage(DocumentEntity selectedDocument) : this()
        {
            _selectedDocument = selectedDocument;
        }

        private async void Button_OnClicked(object sender, EventArgs e) => await Navigation.PushAsync(new SignaturePage(_selectedDocument));
    }
}