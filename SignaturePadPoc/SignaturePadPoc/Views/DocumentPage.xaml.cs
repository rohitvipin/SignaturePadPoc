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

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                SetBusyIndicator(true);
                PdfViewer.Uri = $"{FileManager.GetFilePathFromRoot(_selectedDocument.Id)}.pdf";
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