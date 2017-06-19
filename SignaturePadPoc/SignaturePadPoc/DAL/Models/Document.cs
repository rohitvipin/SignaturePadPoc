using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class Document : ModelBase
    {
        [JsonProperty(PropertyName = "documentId")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "documentUrl")]
        public string DocumentUrl { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}