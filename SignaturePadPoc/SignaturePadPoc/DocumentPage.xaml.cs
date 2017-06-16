using System;
using System.Threading.Tasks;
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
            WebView.Source = new UrlWebViewSource
            {
                Url = selectedDocument.Url
            };
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BusyIndicator.IsRunning = BusyIndicator.IsVisible = true;
            await Task.Delay(2500);
            BusyIndicator.IsRunning = BusyIndicator.IsVisible = false;
        }

        private async void Button_OnClicked(object sender, EventArgs e) => await Navigation.PushAsync(new SignaturePage(_selectedDocument));
    }
}