using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;
namespace Services.AI
{
    public class Response
    {
        public List<string> modalities;
        [TextArea(5, 10)]
        public string instructions;
        [JsonConverter(typeof(StringEnumConverter))]
        public OpenAIVoices voice;
        public string output_audio_format;
        public List<Tools> tools;
        public string tool_choice;
        public double temperature;
        public int max_output_tokens;
    }
}