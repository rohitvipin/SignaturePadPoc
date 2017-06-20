using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class UserDocumentSignature : ModelBase
    {
        [JsonProperty(PropertyName = "signingUserId")]
        public int SigningUserId { get; set; }

        [JsonProperty(PropertyName = "documentId")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "signaturebase64")]
        public string SignatureBase64 { get; set; }
    }
}