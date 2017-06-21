using System;
using SignaturePadPoc.Entities;
using SignaturePadPoc.FileAccessLayer;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
{
    public partial class DocumentPage : ContentPage
    {
        private readonly DocumentEntity _selectedDocument;

        public DocumentPage()
        {
            InitializeComponent();
        }

        public DocumentPage(DocumentEntity selectedDocument) : this()
        {
            _selectedDocument = selectedDocument;
            Title = _selectedDocument.Title;
            Button.Text = _selectedDocument.IsCompleted ? "View Signature" : "Accept";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            SetBusyIndicator(true);

            var fileName = $"{_selectedDocument.Id}.pdf";
            if (await FileManager.ExistsAsync(fileName) == false)
            {
                await FileManager.DownloadDocumentsAsync(_selectedDocument);
            }
            PdfViewer.Uri = FileManager.GetFilePathFromRoot(fileName);

            SetBusyIndicator(false);
        }

        private void SetBusyIndicator(bool isBusyIndicatorIsVisible) => BusyIndicator.IsRunning = BusyIndicator.IsVisible = isBusyIndicatorIsVisible;

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (_selectedDocument.IsCompleted)
            {
                await Navigation.PushAsync(new ViewSignaturePage(_selectedDocument));
            }
            else
            {
                await Navigation.PushAsync(new SignaturePage(_selectedDocument));
            }
        }
    }
}