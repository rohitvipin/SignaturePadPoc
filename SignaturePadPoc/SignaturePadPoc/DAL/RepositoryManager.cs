using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL.Models;
using SignaturePadPoc.DAL.Repositories;

namespace SignaturePadPoc.DAL
{
    public static class RepositoryManager
    {
        private static DocumentRepository _documentRepositoryInstance;
        private static UserDocumentRepository _userDocumentRepositoryInstance;
        private static UserDocumentSignatureRepository _userDocumentSignatureRepositoryInstance;

        public static bool IsInitialized { get; private set; }
        public static DateTime LastSuccessfulSyncDateTime = DateTime.MinValue;
        public static bool IsSyncing;

        static RepositoryManager()
        {
            Initialize();
        }

        public static void Initialize()
        {
            IsInitialized = true;
            var store = new MobileServiceSQLiteStore(Constants.OfflineDbPath);
            store.DefineTable<Document>();
            store.DefineTable<UserDocument>();
            store.DefineTable<UserDocumentSignature>();
            ApplicationContext.MobileServiceClientInstance.SyncContext.InitializeAsync(store);
        }

        public static DocumentRepository DocumentRepositoryInstance
        {
            get { return _documentRepositoryInstance ?? (_documentRepositoryInstance = new DocumentRepository()); }
            set { _documentRepositoryInstance = value; }
        }

        public static UserDocumentRepository UserDocumentRepositoryInstance
        {
            get { return _userDocumentRepositoryInstance ?? (_userDocumentRepositoryInstance = new UserDocumentRepository()); }
            set { _userDocumentRepositoryInstance = value; }
        }

        public static UserDocumentSignatureRepository UserDocumentSignatureRepositoryInstance
        {
            get { return _userDocumentSignatureRepositoryInstance ?? (_userDocumentSignatureRepositoryInstance = new UserDocumentSignatureRepository()); }
            set { _userDocumentSignatureRepositoryInstance = value; }
        }
    }
}