using Newtonsoft.Json;

namespace Services.AI
{
    public class SessionUpdateData
    {
        public string event_id="SessionUpdate";
        public string type= "session.update";
        public RealtimeSession session;

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}