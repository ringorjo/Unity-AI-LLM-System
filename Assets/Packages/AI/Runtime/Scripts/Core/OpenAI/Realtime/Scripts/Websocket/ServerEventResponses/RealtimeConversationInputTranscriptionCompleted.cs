using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeConversationInputTranscriptionCompleted : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _lastResponse;
        private TranscriptionResponse _transcriptionResponse;
        public bool IsLastResponse => _lastResponse;
        public string IdResponse => "conversation.item.input_audio_transcription.completed";

        public void Dispose()
        {
            _transcriptionResponse = null;
        }

        public void PerformResponse(string data)
        {
            _transcriptionResponse = JsonConvert.DeserializeObject<TranscriptionResponse>(data);
            if (_transcriptionResponse != null)
            {
#if UNITY_EDITOR
                Debug.Log($"{nameof(IAPIResponseHandler)}: {IdResponse} Result: {_transcriptionResponse.transcript} ");
#endif
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.UserTranscription, _transcriptionResponse.transcript, _lastResponse));
            }
        }
    }
}

