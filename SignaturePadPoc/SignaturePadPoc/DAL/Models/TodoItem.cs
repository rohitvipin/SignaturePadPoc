using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class TodoItem : ModelBase
    {
        [JsonProperty(PropertyName = "text")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Done { get; set; }
    }
}

