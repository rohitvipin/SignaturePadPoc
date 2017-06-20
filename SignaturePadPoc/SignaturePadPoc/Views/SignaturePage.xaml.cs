using System;
using System.IO;
using System.Linq;
using SignaturePad.Forms;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.DAL.Models;
using SignaturePadPoc.Entities;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
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
            SetBusyIndicator(true);

            using (var stream = await SignaturePad.GetImageStreamAsync(SignatureImageFormat.Png))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    _selectedDocument.SignatureBase64 = Convert.ToBase64String(ms.ToArray());
                }
            }

            await RepositoryManager.UserDocumentSignatureRepositoryInstance.SaveAsync(new UserDocumentSignature
            {
                DocumentId = _selectedDocument.Id,
                SignatureBase64 = _selectedDocument.SignatureBase64,
                SigningUserId = ApplicationContext.LoggedInUserId
            });

            var userDocument = (await RepositoryManager.UserDocumentRepositoryInstance.GetAsync(x => x.DocumentId == _selectedDocument.Id))?.FirstOrDefault();
            if (userDocument == null)
            {
                return;
            }
            userDocument.IsCompleted = true;
            await RepositoryManager.UserDocumentRepositoryInstance.SaveAsync(userDocument);

            SetBusyIndicator(false);

            await Navigation.PopToRootAsync(true);
        }

        private void SetBusyIndicator(bool isBusy) => BusyIndicator.IsRunning = BusyIndicator.IsVisible = isBusy;
    }
}