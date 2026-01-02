using Newtonsoft.Json;
using System.Collections.Generic;

namespace Services.AI
{
    public class AssistantThread
    {
        public List<Message> messages;

        public AssistantThread(List<Message> messages)
        {
            this.messages = messages;
        }

        public string GetJson()
        {

            return JsonConvert.SerializeObject(messages);
        }
    }

}

