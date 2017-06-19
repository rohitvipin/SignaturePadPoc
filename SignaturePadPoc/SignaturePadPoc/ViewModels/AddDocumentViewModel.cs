using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.DAL.Models;

namespace SignaturePadPoc.ViewModels
{
    public class AddDocumentViewModel : ViewModelBase
    {
        private string _userId;
        private bool _isBusy;
        public Document Document { get; set; } = new Document();

        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                if (IsValid(Document) == false)
                {
                    return false;
                }

                IsBusy = true;
                await RepositoryManager.DocumentRepositoryInstance.SaveAsync(Document);
                var userId = UserId.ToIntSafe();
                if (userId > 0)
                {
                    var userDocument = (await RepositoryManager.UserDocumentRepositoryInstance.GetAsync(x => x.DocumentId == Document.DocumentId && x.AssignedUserId == userId.Value))?.FirstOrDefault();

                    if (userDocument == null)
                    {
                        await RepositoryManager.UserDocumentRepositoryInstance.SaveAsync(new UserDocument
                        {
                            AssignedUserId = userId.Value,
                            DocumentId = Document.DocumentId,
                            IsCompleted = false
                        });
                    }
                }
                IsBusy = false;

                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return false;
            }
        }

        private static bool IsValid(Document document)
        {
            if (document == null)
            {
                return false;
            }

            if (document.DocumentId <= 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(document.DocumentUrl) || string.IsNullOrWhiteSpace(document.Title) || string.IsNullOrWhiteSpace(document.Description))
            {
                return false;
            }

            return true;
        }
    }
}