using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.Entities;
using SignaturePadPoc.FileAccessLayer;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
{
    public partial class CompletedPage : ContentPage
    {
        private readonly ObservableCollection<DocumentEntity> _documentEntities = new ObservableCollection<DocumentEntity>();

        public CompletedPage()
        {
            InitializeComponent();
            ListView.ItemsSource = _documentEntities;
            ListView.RefreshCommand = new Command(async t => await RefreshListDataAsync());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshListDataAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _documentEntities.Clear();
        }

        private async Task RefreshListDataAsync()
        {
            ListView.IsRefreshing = true;

            _documentEntities.Clear();

            var userDocuments = (await RepositoryManager.UserDocumentRepositoryInstance.GetAsync(x => x.AssignedUserId == ApplicationContext.LoggedInUserId && x.IsCompleted)).ToList();

            if (userDocuments?.Count > 0)
            {
                foreach (var userDocument in userDocuments)
                {
                    var document = (await RepositoryManager.DocumentRepositoryInstance.GetAsync(x => x.DocumentId == userDocument.DocumentId))?.FirstOrDefault();
                    if (document != null)
                    {
                        var userDocumentSignature = (await RepositoryManager.UserDocumentSignatureRepositoryInstance.GetAsync(x => x.DocumentId == userDocument.DocumentId))?.FirstOrDefault();
                        
                        _documentEntities.Add(new DocumentEntity
                        {
                            Url = document.DocumentUrl,
                            Id = document.DocumentId,
                            Title = document.Title,
                            SubTitle = document.Description,
                            IsCompleted = userDocument.IsCompleted,
                            SignatureBase64 = userDocumentSignature?.SignatureBase64
                        });
                    }
                }
            }
            else
            {
                _documentEntities.Clear();
            }

            ListView.IsRefreshing = false;
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

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            ListView.IsRefreshing = true;
            await RepositoryManager.UserDocumentRepositoryInstance.ForceSyncAsync();
            await RepositoryManager.DocumentRepositoryInstance.ForceSyncAsync();
            await FileManager.DownloadAllUserDocumentsAsync();
            await RefreshListDataAsync();
        }
    }
}
