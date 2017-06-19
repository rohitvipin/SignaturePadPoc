using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace SignaturePadPoc.DAL.Models
{
    public class ModelBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Version]
        public string Version { get; set; }
    }
}