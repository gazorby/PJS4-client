using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Client
{
    public class Message
    {
        [JsonProperty("phase")]
        private String phase;
        
        [JsonProperty("type")]
        private String type;
        
        [JsonProperty("content")]
        private Dictionary<String, String> content;
        
        
        public Message()
        {
        }

        public void setHeader(String phase, String type)
        {
            this.phase = phase;
            this.type = type;
        }

        private Object format(Dictionary<String, String> content, String type, String phase=null)
        {
            String local = null;
            
            if (this.phase != null)
            {
                if (phase == null)
                {
                    local = this.phase;
                }
                else
                {
                    new Exception();
                }
            }
            else
            {
                local = phase;
            }

            return JsonConvert.SerializeObject(new
                {
                    header = JsonConvert.SerializeObject(new
                    {
                        phase = local,
                        type = type
                    }),
                    content = JsonConvert.SerializeObject(content)
                }
            );
        }

        public Object sendBet(String value)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, String>();
            dictionary.Add("value", value);
            return format(dictionary, "bet");
        }

        public String getType()
        {
            return type;
        }
    }
    
}