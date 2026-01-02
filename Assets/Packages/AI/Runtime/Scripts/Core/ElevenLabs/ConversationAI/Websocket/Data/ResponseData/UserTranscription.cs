using Newtonsoft.Json;
namespace Services.AI.ElevenLabs.ConversationAI
{
    public class UserTranscription
    {
        [JsonProperty("user_transcription_event")]
        public UserTranscriptionData UserTranscriptionData;
    }

    public class UserTranscriptionData
    {
        public string user_transcript;
    }
}

