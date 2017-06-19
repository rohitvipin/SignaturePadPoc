using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class TodoItem : ModelBase
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "isCompleted")]
        public bool IsCompleted { get; set; }
    }
}

