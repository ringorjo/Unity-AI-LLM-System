using Newtonsoft.Json;
namespace Services.AI
{
    public class ResponseCreateData
    {
        public string event_id;
        public string type;
        public Response response;
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}