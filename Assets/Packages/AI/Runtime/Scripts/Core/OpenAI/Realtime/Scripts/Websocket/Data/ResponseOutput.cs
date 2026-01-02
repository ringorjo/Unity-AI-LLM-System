using System;
using System.Collections.Generic;

namespace Services.AI
{
    [Serializable]
    public class ResponseOutput
    {

        public string type;
        public string event_id;
        public string response_id;
        public int output_index;
        public Item item;



        [Serializable]
        public class Item
        {
            public string id;
            public string @object;
            public string type;
            public string status;
            public string role;
            public List<Content> content;
            public string name;
            public string call_id;
            public string arguments;
        }
        [Serializable]
        public class Content
        {
            public string type;
            public string text;
        }
    }

    [Serializable]
    public class RealtimeResponseDone
    {
        public string type;
        public string event_id;
    }
    [Serializable]
    public class RealtimeResponse
    {
        public string @object;
        public string id;
        public string status;
        public object status_details;
        public List<Output> output;
        public string conversation_id;
        public List<string> modalities;
        public string voice;
        public object custom_voice_id;
        public string output_audio_format;
        public double temperature;
        public int max_output_tokens;
        public UsageToken usage;
        public string metadata;
    }
    [Serializable]
    public class Output
    {
        public string id;
        public string @object;
        public string type;
        public string status;
        public string name;
        public string role;
        public string call_id;
        public string arguments;
    }
    [Serializable]
    public class UsageToken
    {
        public int total_tokens;
        public int input_tokens;
        public int output_tokens;
        public InputTokenDetails input_token_details;
        public CachedTokenDetails output_token_details;
    }
    [Serializable]
    public class InputTokenDetails
    {
        public int text_tokens;
        public int audio_tokens;
        public int cached_tokens;
        public CachedTokenDetails cached_tokens_details;
    }
    [Serializable]
    public class CachedTokenDetails
    {
        public int text_tokens;
        public int audio_tokens;
    }
}
