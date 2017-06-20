using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class UserDocument : ModelBase
    {
        [JsonProperty(PropertyName = "assignedUserId")]
        public int AssignedUserId { get; set; }

        [JsonProperty(PropertyName = "documentId")]
        public int DocumentId { get; set; }

        [JsonProperty(PropertyName = "isDownloaded")]
        public bool IsDownloaded { get; set; }

        [JsonProperty(PropertyName = "isCompleted")]
        public bool IsCompleted { get; set; }
    }
}