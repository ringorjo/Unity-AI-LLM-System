using System;

namespace Services.AI
{
    [Serializable]
    public class TranscriptionResponse
    {
        public string event_id;
        public string type;
        public string response_id;
        public string item_id;
        public int output_index;
        public int content_index;
        public string transcript;
    }
}
