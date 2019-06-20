using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Client.Models
{
    public class Position
    {
        [JsonProperty("x")]
        public int x;
        
        [JsonProperty("y")]
        public int y;

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}