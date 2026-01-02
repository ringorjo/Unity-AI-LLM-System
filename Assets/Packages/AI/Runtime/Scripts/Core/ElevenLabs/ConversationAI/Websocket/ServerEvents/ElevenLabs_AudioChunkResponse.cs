using Newtonsoft.Json;
using System;
using UnityEngine;
using Services.AI.ElevenLabs.ConversationAI;

namespace Services.AI
{
    public class ElevenLabs_AudioChunkResponse : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        public bool IsLastResponse => _lastResponse;
        public string IdResponse => "audio";

        [SerializeField]
        private bool _lastResponse;
        [SerializeField]
        private AudioEvent _audioEventResponse;
        private AIAPIResponse _aiApiResponse;

        public void Dispose() => _audioEventResponse = null;

        public void PerformResponse(string data)
        {
            try
            {
                if (_aiApiResponse == null)
                {
                    _aiApiResponse = new AIAPIResponse(AIResponseType.Audio, string.Empty);
                }

                _audioEventResponse = JsonConvert.DeserializeObject<AudioEvent>(data);

                if (_audioEventResponse != null)
                {
                    _aiApiResponse?.UpdateText(_audioEventResponse.audio_base_64);
                    OnAPIResponse?.Invoke(_aiApiResponse);
                }
            }
            catch (JsonException e)
            {
                Debug.LogError($"Failed to deserialize audio event: {e.Message}");
            }
        }
    }

}
