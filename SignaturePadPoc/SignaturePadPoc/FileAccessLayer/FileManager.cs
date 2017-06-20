using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;

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

        public static string GetFilePathFromRoot(int selectedDocumentId) => Path.Combine(FileSystem.Current.LocalStorage.Path, selectedDocumentId.ToString());
    }
}