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
        public String Phase { get; set; }
        
        [JsonProperty("type")]
        public String Type { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }
        
        
        public Message()
        {
        }

        public void setHeader(String phase, String type)
        {
            Phase = phase;
            Type = type;
        }

    }
    
}