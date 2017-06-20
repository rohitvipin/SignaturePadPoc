using System;
using System.Diagnostics;
using System.IO;
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
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                SetBusyIndicator(true);
                var fileName = $"{_selectedDocument.Id}.pdf";
                var filePath = FileManager.GetFilePathFromRoot(fileName);
                if (await FileManager.ExistsAsync(filePath) == false)
                {
                    await FileManager.DownloadDocumentsAsync(_selectedDocument);
                }
                PdfViewer.Uri = filePath;
                PdfViewer.Navigated += PdfViewer_OnNavigated;
                PdfViewer.Navigating += PdfViewer_Navigating;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void PdfViewer_Navigating(object sender, EventArgs e) => SetBusyIndicator(true);

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PdfViewer.Navigated -= PdfViewer_OnNavigated;
            PdfViewer.Navigating -= PdfViewer_Navigating;
        }

        private void PdfViewer_OnNavigated(object sender, EventArgs args) => SetBusyIndicator(false);

        private void SetBusyIndicator(bool isBusyIndicatorIsVisible) => BusyIndicator.IsRunning = BusyIndicator.IsVisible = isBusyIndicatorIsVisible;

        private async void Button_OnClicked(object sender, EventArgs e) => await Navigation.PushAsync(new SignaturePage(_selectedDocument));
    }
}