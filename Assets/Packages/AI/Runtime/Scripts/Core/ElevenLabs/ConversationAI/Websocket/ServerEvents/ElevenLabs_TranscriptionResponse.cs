using Newtonsoft.Json;
using System;
using UnityEngine;
using Services.AI.ElevenLabs.ConversationAI;
using Xennial.Services;

namespace Services.AI
{
    public class ElevenLabs_TranscriptionResponse : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _lastResponse;
        [SerializeField]
        private UserTranscriptionData _response;
        private IAIHandler _aiModel;

        public bool IsLastResponse => _lastResponse;
        public string IdResponse => "user_transcript";


        public void Dispose() { }

        public void PerformResponse(string data)
        {
            InitObjects();
            _response = JsonConvert.DeserializeObject<UserTranscriptionData>(data);
            if (_aiModel == null)
            {
                _aiModel = ServiceLocator.Instance.Get<IAIHandler>();
            }
            if (_response != null)
            {
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.UserTranscription, _response.user_transcript, _lastResponse));
            }
        }

        private void InitObjects()
        {

            if (_aiModel == null)
                _aiModel = ServiceLocator.Instance.Get<IAIHandler>();
        }
    }
}