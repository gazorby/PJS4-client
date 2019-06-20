using System.Collections.Generic;
using Newtonsoft.Json;

namespace Client.Models
{
    public class ActionDescription
    {
        [JsonProperty("card_type")]
        public string cardType;
        
        [JsonProperty("fixed_params")]
        public List<string> fixedParams;
        
        [JsonProperty("var_params")]
        public List<string> varParams;
        
        [JsonProperty("return_name")]
        public string returnName;

    }
    public class Card
    {
        [JsonProperty("name")]
        public string name;
        
        [JsonProperty("type")]
        public string type;
        
        [JsonProperty("description")]
        public string description;
        
        [JsonProperty("requirements")]
        public Dictionary<string, int> requirements;
        
        [JsonProperty("actions")]
        public List<ActionDescription> actions;
    }
}