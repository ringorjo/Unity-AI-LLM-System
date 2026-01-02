using Newtonsoft.Json;
using System;
using Services.AI.ElevenLabs.ConversationAI;

namespace Services.AI
{
    public class ElevenLabs_ConversationIdCreated : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        public bool IsLastResponse => true;

        public string IdResponse => "conversation_initiation_metadata";

        public void Dispose()
        {
        }

        public void PerformResponse(string data)
        {
            ConversationalIdCreatedResponse conversationalIdCreatedResponse = JsonConvert.DeserializeObject<ConversationalIdCreatedResponse>(data);
            if (conversationalIdCreatedResponse == null)
                return;

            // Notify response
        }
    }
}