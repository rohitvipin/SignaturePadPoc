using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.DAL.Models;
using SignaturePadPoc.Entities;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ObservableCollection<DocumentEntity> _documentEntities = new ObservableCollection<DocumentEntity>();

        public MainPage()
        {
            InitializeComponent();
            ListView.ItemsSource = _documentEntities;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var userDocuments = (await RepositoryManager.UserDocumentRepositoryInstance
                .GetAsync(x => x.AssignedUserId == ApplicationContext.LoggedInUserId && x.IsCompleted == false))
                ?.Select(x => x.DocumentId);

            Reset((await RepositoryManager.DocumentRepositoryInstance.GetAsync(x => userDocuments.Any(y => y == x.DocumentId)))?.ToList());
        }

        private void Reset(IReadOnlyCollection<Document> documents)
        {
            _documentEntities.Clear();
            if (!(documents?.Count > 0))
            {
                return;
            }

            foreach (var document in documents)
            {
                _documentEntities.Add(new DocumentEntity
                {
                    Url = document.DocumentUrl,
                    Id = document.DocumentId
                });
            }
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
