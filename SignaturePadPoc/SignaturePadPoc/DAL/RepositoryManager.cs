namespace SignaturePadPoc.DAL
{
    public static class RepositoryManager
    {
        private static DocumentRepository _documentRepositoryInstance;
        private static UserDocumentRepository _userDocumentRepositoryInstance;
        private static UserDocumentSignatureRepository _userDocumentSignatureRepositoryInstance;

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
            get { return _userDocumentSignatureRepositoryInstance ?? (_userDocumentSignatureRepositoryInstance = new UserDocumentSignatureRepository()); ; }
            set { _userDocumentSignatureRepositoryInstance = value; }
        }
    }
}