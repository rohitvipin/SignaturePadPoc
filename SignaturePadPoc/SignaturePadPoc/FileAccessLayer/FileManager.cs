using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.Entities;

namespace SignaturePadPoc.FileAccessLayer
{
    public static class FileManager
    {
        public static async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var openAsync = (await FileSystem.Current.GetFileFromPathAsync(filePath))?.OpenAsync(FileAccess.Read);
            if (openAsync == null)
            {
                return null;
            }
            return await openAsync;
        }

        public static async Task SaveFileAsync(string fileName, MemoryStream inputStream)
        {
            var file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                inputStream.WriteTo(stream);
            }
        }

        public static async Task DownloadAllUserDocumentsAsync()
        {
            foreach (var userDocument in await RepositoryManager.UserDocumentRepositoryInstance.GetAsync(x => x.AssignedUserId == ApplicationContext.LoggedInUserId && x.IsDownloaded == false && x.IsCompleted == false))
            {
                var document = (await RepositoryManager.DocumentRepositoryInstance.GetAsync(x => x.DocumentId == userDocument.DocumentId))?.FirstOrDefault();
                if (document == null)
                {
                    continue;
                }
                var stream = await RestApiHelper.DownloadFileAsync(document.DocumentUrl);
                if (stream == null)
                {
                    continue;
                }
                await SaveFileAsync($"{document.DocumentId}.pdf", stream);
                userDocument.IsDownloaded = true;
                await RepositoryManager.UserDocumentRepositoryInstance.SaveAsync(userDocument);
            }
        }

        public static string GetFilePathFromRoot(string fileName) => Path.Combine(FileSystem.Current.LocalStorage.Path, fileName);

        public static async Task<bool> ExistsAsync(string fileName) => await FileSystem.Current.LocalStorage.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists;

        public static async Task DownloadDocumentsAsync(DocumentEntity documentEntity)
        {
            var userDocument = (await RepositoryManager.UserDocumentRepositoryInstance.GetAsync(x => x.DocumentId == documentEntity.Id))?.FirstOrDefault();
            if (userDocument == null)
            {
                return;
            }

            var stream = await RestApiHelper.DownloadFileAsync(documentEntity.Url);
            if (stream == null)
            {
                return;
            }
            await SaveFileAsync($"{documentEntity.Id}.pdf", stream);
            userDocument.IsDownloaded = true;
            await RepositoryManager.UserDocumentRepositoryInstance.SaveAsync(userDocument);
        }
    }
}