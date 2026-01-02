using System;
using UnityEngine;

namespace Services.AI
{
    public class RunExpetionResponse : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        public string IdResponse => "Exception";
        [SerializeField]
        private bool _lastResponse;

        public bool IsLastResponse => _lastResponse;
        public void PerformResponse(string data)
        {
            OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, data));
        }

        public void Dispose() => Debug.Log($"{nameof(RunExpetionResponse)} Is Disposed");
    }
}
