using Newtonsoft.Json;
using System;
using UnityEngine;
using Services.AI.ElevenLabs.ConversationAI;
using Xennial.Services;

namespace Services.AI
{
    public class ElevenLabs_AgentResponse : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _lastResponse;
        [SerializeField]
        private AgentResponseData _response;
        public bool IsLastResponse => _lastResponse;
        public string IdResponse => "agent_response";
        private AIAPIResponse _aiResponse;
        private IAIHandler _aiModel;

        public void Dispose() => _response = null;

        public void PerformResponse(string data)
        {
            InitObjects();
            _response = JsonConvert.DeserializeObject<AgentResponseData>(data);
            if (_response != null)
            {
                _aiResponse.UpdateText(_response.agent_response);
                OnAPIResponse?.Invoke(_aiResponse);
            }
        }

        private void InitObjects()
        {
            if (_aiResponse == null)
                _aiResponse = new AIAPIResponse(AIResponseType.AIResponse, string.Empty, _lastResponse);

            if (_aiModel == null)
                _aiModel = ServiceLocator.Instance.Get<IAIHandler>();
        }
    }
}