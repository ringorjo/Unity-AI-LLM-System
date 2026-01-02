using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Services.AI
{
    public class RunMessageCompleted : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _lastResponse;
        public string IdResponse => "thread.message.completed";
        private UserMessageSendResponse _userMessageSendResponse;
        public bool IsLastResponse => _lastResponse;

        public void Dispose()
        {
            _userMessageSendResponse = null;
        }

        public void PerformResponse(string data)
        {
            _userMessageSendResponse = JsonConvert.DeserializeObject<UserMessageSendResponse>(data);
            if (OnAPIResponse != null)
            {
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, _userMessageSendResponse.content[0].text.value, _lastResponse));
                Debug.Log("Semantic Comparation Response: " + _userMessageSendResponse.content[0].text.value);
            } 
        }
    }
}