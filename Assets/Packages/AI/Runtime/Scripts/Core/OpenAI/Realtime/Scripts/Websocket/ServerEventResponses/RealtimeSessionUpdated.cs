using System;
using UnityEngine;
namespace Services.AI
{
    public class RealtimeSessionUpdated : IAPIResponseHandler
    {
        [SerializeField]
        private bool _lastResponse;
        public bool IsLastResponse => _lastResponse;

        public string IdResponse => "session.updated";

        public event Action<AIAPIResponse> OnAPIResponse;

        public void Dispose() => Debug.Log($"{nameof(RealtimeSessionUpdated)} is disposed");

        public void PerformResponse(string data)
        {
            OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, "SessionUpdated", _lastResponse));
        }
    }
}

