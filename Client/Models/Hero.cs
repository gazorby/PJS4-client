using System.Collections.Generic;
using Newtonsoft.Json;

namespace Client.Models
{
    public class Hero
    {
        [JsonProperty("name")]
        public string name;
        
        [JsonProperty("alive")]
        public bool alive;
        
        [JsonProperty("life_points")]
        public int life_points;

        [JsonProperty("armor_points")]
        public int armor_points;
        
        [JsonProperty("fatigue_points")]
        public int fatigue_points;
        
        [JsonProperty("armor_points_activated")]
        public bool armor_points_activated;
        
        [JsonProperty("position")]
        public Position position;
        
        [JsonProperty("cards")]
        public List<Card> cards;
        
        [JsonProperty("generics")]
        public List<Card> generics;
        
        [JsonProperty("dice")]
        public Dictionary<string, int> dice;
    }
}