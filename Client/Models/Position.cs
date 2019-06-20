using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Client.Models
{
    public class Position
    {
        [JsonProperty("x")]
        public int x;
        
        [JsonProperty("x")]
        public int y;
    }
}