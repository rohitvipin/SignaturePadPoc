using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class Document : ModelBase
    {
        [JsonProperty(PropertyName = "documentId")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "documentUrl")]
        public string DocumentUrl { get; set; }
    }
}