
using Newtonsoft.Json;

namespace Services.AI.ElevenLabs.ConversationAI.Websocket.Base
{
    public abstract class WebsocketMessage
    {
        [JsonProperty("type")]
        public abstract string Type { get; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

