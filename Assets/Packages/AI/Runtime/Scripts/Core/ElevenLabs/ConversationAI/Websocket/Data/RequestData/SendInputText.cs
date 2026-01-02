using Newtonsoft.Json;
using Services.AI.ElevenLabs.ConversationAI.Websocket.Base;


namespace Services.AI.ElevenLabs.ConversationAI.Websocket.Data.RequestData
{
    public class SendInputText : WebsocketMessage
    {
        public override string Type
        {
            get => "user_message";
        }

        [JsonProperty("text")]
        public string Text;
        public void SetText(string text)
        {
            Text = text;
        }
    }
}
