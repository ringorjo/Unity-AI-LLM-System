using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeOutputTranscriptionResponseDone : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _lastResponse;
        public string IdResponse => "response.audio_transcript.done";
        private TranscriptionResponse _transcriptionResponse;
        public bool IsLastResponse => _lastResponse;

        public void Dispose()
        {
            _transcriptionResponse = null;
        }

        public void PerformResponse(string data)
        {
            _transcriptionResponse = JsonConvert.DeserializeObject<TranscriptionResponse>(data);
            if (_transcriptionResponse != null)
            {
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, _transcriptionResponse.transcript, _lastResponse));
#if UNITY_EDITOR
                Debug.Log($"{nameof(IAPIResponseHandler)}: {IdResponse} Result: {_transcriptionResponse.transcript} ");
#endif
            }
        }
    }
}