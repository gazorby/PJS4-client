using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Client.Models
{
    public class LobbyPhase
    {
        [JsonProperty("commands")]
        public List<string> commands;
    }
    
    public class LobbyCreate
    {
        [JsonProperty("id")]
        public string id;
    }
    
    public class LobbyJoin
    {
        [JsonProperty("id")]
        public string id;
    }
    
    public class Error
    {
        [JsonProperty("message")]
        public string message;
    }
    
    public class GamePhaseInit
    {
        [JsonProperty("heroes")]
        public List<Hero> heroes;
        
        [JsonProperty("board")]
        public List<string> board;

        [JsonProperty("id")]
        public string id;
    }

    

    public class EquipPhaseInit
    {
        [JsonProperty("card_type")] 
        public string cardType;

        [JsonProperty("available_cards")] 
        public List<Card> availableCards;
    }

    public class EquipPhaseBetSuccess
    {
        [JsonProperty("bet_success")]
        public Dictionary<string, int> betSuccess;
    }

    public class EquipPhaseAsk
    {
        [JsonProperty("hero_id")] 
        public string heroId;
    }
    
    public class EquipPhasePlayer
    {
        [JsonProperty("hero_id")] 
        public string heroId;
        
        [JsonProperty("card_type")] 
        public string cardType;
        
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
    
    public class PrepPhasePosition
    {
        [JsonProperty("hero_id")]
        public string heroId;
        
        [JsonProperty("selected_position")]
        public Position selectedPosition;
    }

    public class TurnPhaseInit
    {
        public class Actions
        {
            [JsonProperty("cards")]
            public List<string> cardsType;

            [JsonProperty("generics")]
            public List<string> genericsType;

            [JsonProperty("can_replay_dice")]
            public bool canReplayDice;
        }
        
        [JsonProperty("hero")]
        public Hero hero;

        [JsonProperty("actions")]
        public Actions actions;
    }
    
    public class TurnPhaseReplayUpdate
    {
        [JsonProperty("action")]
        public string action;
    }
    
    public class TurnPhaseEndUpdate
    {
        [JsonProperty("action")]
        public string action;
    }
    
    public class TurnPhaseActionUpdate
    {
        [JsonProperty("action")]
        public string action;

        [JsonProperty("card")]
        public string card;
        
        [JsonProperty("type")]
        public string type;
    }
    
    public class TurnPhasePendingHero
    {
        [JsonProperty("hero_id")]
        public string heroId;

        [JsonProperty("card_id")] 
        public string cardId;
        
        [JsonProperty("heroes")]
        public List<string> heroesId;
        
        [JsonProperty("nb_to_choose")]
        public int nbToChoose;
    }
    
    public class TurnPhasePendingPositions
    {
        [JsonProperty("hero_id")]
        public string heroId;

        [JsonProperty("card_id")] 
        public string cardId;
        
        [JsonProperty("positions")]
        public List<Position> positions;
    }
    
    public class TurnPhasePendingHeroUpdate
    {
        [JsonProperty("hero_id")]
        public string heroId;

        [JsonProperty("card_id")] 
        public string cardId;
        
        [JsonProperty("heroes")]
        public List<string> heroes;
    }
    
    public class TurnPhasePendingPositionUpdate
    {
        [JsonProperty("hero_id")]
        public string heroId;

        [JsonProperty("card_id")] 
        public string cardId;
        
        [JsonProperty("position")]
        public Position position;
    }
    
    public class TurnPhasePlayer
    {
        [JsonProperty("heroes")]
        public List<Hero> heroes;
    }

    public class TurnPhaseDice
    {
        [JsonProperty("phase")]
        public string phase;

        [JsonProperty("type")] 
        public string msg_type;
        
        [JsonProperty("content")]
        public Dictionary<string, int> content;

        public TurnPhaseDice(Dictionary<string, int> content)
        {
            this.phase = "TURN";
            this.msg_type = "TURN_PHASE_DICE";
            this.content = content;
        }
    }

    public class TurnPhaseAction
    {
        [JsonProperty("phase")]
        public string phase;

        [JsonProperty("type")] 
        public string msg_type;
        
        [JsonProperty("content")]
        public Dictionary<string, string> content;
        
        public TurnPhaseAction(Dictionary<string, string> content)
        {
            this.phase = "TURN";
            this.msg_type = "TURN_PHASE_ACTION";
            this.content = content;
        }
    }
    
    public class EndTurn
    {
        [JsonProperty("phase")]
        public string phase;

        [JsonProperty("type")] 
        public string msg_type;
        
        [JsonProperty("content")]
        public Dictionary<string, string> content;
        
        public EndTurn()
        {
            this.phase = "TURN";
            this.msg_type = "END_TURN";
            this.content = new Dictionary<string, string>()
            {
                { "action", "END_TURN" }
            };
        }
    }

}