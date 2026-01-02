using Newtonsoft.Json;
namespace Services.AI
{
    public class ConversationCreateData
    {
        public string event_id = "ConversationCreate";
        public string type = "conversation.item.create";
        public string previous_item_id;
        public ItemConversation item;

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}