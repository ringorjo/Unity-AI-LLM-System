using System;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeSessionCreated : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        [SerializeField]
        private bool _isLastResponse;

        public bool IsLastResponse => _isLastResponse;

        public string IdResponse => "session.created";

        public void Dispose() => Debug.Log($"{nameof(RealtimeSessionCreated)} is Dispoed");

        public void PerformResponse(string data)
        {
            OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, "SessionCreated", _isLastResponse));
        }
    }
}

