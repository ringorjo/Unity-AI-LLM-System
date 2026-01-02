using Meta.WitAi.Json;


namespace Services.AI.ElevenLabs.ConversationAI
{
    public class PingResponse
    {
        [JsonProperty("ping_event")]
        public PingResponseData PingEventData;
    }

    public class PingResponseData
    {
        [JsonProperty("event_id")]
        public int EventId;
        [JsonProperty("ping_ms")]
        public int Ping;
    }
}

