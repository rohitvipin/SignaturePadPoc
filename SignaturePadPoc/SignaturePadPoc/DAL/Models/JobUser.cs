using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class JobUser : ModelBase
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
