using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Services.AI
{
    [Serializable]
    public class AssistanceCreateResponse
    {
        public string id;
        public string @object;
        public int created_at;
        public string name;
        public object description;
        public string model;
        public string instructions;
        [JsonIgnore]
        public List<Tools> tools;
        public double top_p;
        public double temperature;
        public string response_format;
    }
}