using Newtonsoft.Json;
using System;
using UnityEngine;
using Services.AI.ElevenLabs.ConversationAI;

namespace Services.AI
{
    public class ElevenLabs_ServerVadValueUpdate : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        public string IdResponse => "vad_score";
        public bool IsLastResponse => true;

        [SerializeField]
        private float _vadSpeechScore = 0.9f;
        private AIAPIResponse _response;
        private VadScore _vadScore;
        public void Dispose() { }

        public void PerformResponse(string data)
        {
            _vadScore = JsonConvert.DeserializeObject<VadScore>(data);
            if (_response == null)
            {
                _response = new AIAPIResponse(AIResponseType.Json_extraData, "", IsLastResponse);
            }
            if (_vadScore != null)
            {
                _response.UpdateText(_vadScore.vad_score.ToString());
                if (_vadScore.vad_score > _vadSpeechScore)
                {
                    OnAPIResponse.Invoke(_response);
                }
            }

        }
    }
}