using Newtonsoft.Json;
using UnityEngine;

namespace Services.AI
{
    public class Tools
    {
        public string type;
        public string name;
        [TextArea(4, 10)]
        public string description;
        public Parameters parameters;

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
