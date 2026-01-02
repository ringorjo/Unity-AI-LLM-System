using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeSession
    {
        public List<string> modalities;
        [TextArea(5, 10)]
        public string instructions;
        [JsonConverter(typeof(StringEnumConverter))]
        public OpenAIVoices voice;
        public string input_audio_format;
        public string output_audio_format;
        public InputAudioTranscription input_audio_transcription;
        public TurnDetection turn_detection;
        public List<Tools> tools;
        public string tool_choice;
        public double temperature;
        public int max_response_output_tokens;
    }
}





