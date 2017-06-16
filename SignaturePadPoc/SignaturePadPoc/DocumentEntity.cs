namespace SignaturePadPoc
{
    public class DocumentEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SignatureBase64 { get; set; }
        public string Url { get; set; }
    }
}
