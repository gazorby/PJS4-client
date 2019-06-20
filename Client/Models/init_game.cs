using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Client.Models
{
    public class InitGame
    {
        [JsonProperty("heroes")]
        public List<Hero> heroes;
        
        [JsonProperty("board")]
        public List<string> board;
    }

    

    public class EquipInit
    {
        [JsonProperty("card_type")] 
        public string cardType;

        [JsonProperty("cards")] 
        public List<Card> cards;
    }

    public class EquipPhaseInit
    {
        [JsonProperty("bet_success")]
        public Dictionary<string, int> betSuccess;
    }

    public class EquipPhaseAsk
    {
        [JsonProperty("hero_id")] 
        public string heroId;
    }
    
    public class EquipPlayer
    {
        [JsonProperty("card_id")] 
        public string cardId;
    }

    public class PrepPhaseInit
    {
        [JsonProperty("hero_id")]
        public string heroId;

        [JsonProperty("available_positions")]
        public List<Position> availablePositions;
    }
    
    public class PrepPhaseAsk
    {
        [JsonProperty("selected_position")]
        public Position selectedPosition;
    }

    public class TurnActionInit
    {
        [JsonProperty("hero_id")]
        public string heroId;
        
        [JsonProperty("dice_result")]
        public Dictionary<string, int> diceResult;

        [JsonProperty("possible_actions")]
        public List<string> possibleActions;
        
        [JsonProperty("can_replay_dice")]
        public bool canReplayDice;
    }
    
    public class TurnPhaseActionUpdate
    {
        [JsonProperty("selected_action")]
        public string selectedAction;
    }
    
    public class TurnPhasePendingActionAsk
    {
        [JsonProperty("positions")]
        public List<Position> positions;
        
        [JsonProperty("heroes")]
        public List<Hero> heroes;
    }
    
    public class TurnPhasePendingActionUpdate
    {
        [JsonProperty("heroes")]
        public List<Hero> heroes;
    }

}