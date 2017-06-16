using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignaturePadPoc
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
            WebView.Navigating += WebView_Navigating;
            WebView.Navigated += WebView_Navigated;

            _selectedDocument = selectedDocument;
            Title = _selectedDocument.Title;

            WebView.Source = new UrlWebViewSource
            {
                Url = selectedDocument.Url
            };
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e) => SetBusyIndicator(false);

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e) => SetBusyIndicator(true);

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetBusyIndicator(true);
        }

        private void SetBusyIndicator(bool isBusyIndicatorIsVisible) => BusyIndicator.IsRunning = BusyIndicator.IsVisible = isBusyIndicatorIsVisible;

        private async void Button_OnClicked(object sender, EventArgs e) => await Navigation.PushAsync(new SignaturePage(_selectedDocument));
    }
}