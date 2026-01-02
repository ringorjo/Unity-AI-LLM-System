using Newtonsoft.Json;
using System;

namespace Services.AI
{
    [Serializable]
    public class ApendBufferCommit
    {
        public string event_id;
        public string type;

        public ApendBufferCommit(string event_id, string type)
        {
            this.event_id = event_id;
            this.type = type;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}