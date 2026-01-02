using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;

namespace Services.AI
{
    [Serializable]
    public class InputBufferData
    {
        public string event_id;
        public string type;
        [ReadOnly]
        public string audio;

        public InputBufferData(string event_id, string type)
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