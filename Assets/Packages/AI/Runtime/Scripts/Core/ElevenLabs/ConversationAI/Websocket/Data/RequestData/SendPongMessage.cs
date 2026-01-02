
using Newtonsoft.Json;

namespace Services.AI.ElevenLabs.ConversationAI.Websocket.Data.RequestData
{
    public class SendPongMessage
    {
        public string type = "pong";
        public int event_id;

        public void SetEventId(int eventId)
        {
            event_id = eventId;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
