
using Newtonsoft.Json;

namespace Services.AI.ConversationAI.Websocket.Data.RequestData
{
    public class ConversationData
    {
        [JsonProperty("type")]
        public const string TYPE = "conversation_initiation_client_data";

        public ConversationData()
        {
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

