using System.Collections.Generic;
using Xamarin.Forms;

namespace SignaturePadPoc
{
    public partial class MainPage : ContentPage
    {
        private readonly List<DocumentEntity> _documentEntities = new List<DocumentEntity>();

        public MainPage()
        {
            InitializeComponent();

            for (var i = 0; i < 25; i++)
            {
                _documentEntities.Add(new DocumentEntity
                {
                    Id = i,
                    SubTitle = $"Subtitle {i}",
                    Title = $"Title {i}"
                });
            }

            ListView.ItemsSource = _documentEntities;
        }

        private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e?.SelectedItem == null)
            {
                return;
            }

            var selectedDocument = e.SelectedItem as DocumentEntity;

            if (selectedDocument != null)
            {
                await Navigation.PushAsync(new DocumentPage(selectedDocument));
            }

            ListView.SelectedItem = null;
        }
    }
}
