using System;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeConversationCreated : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        [SerializeField]
        private bool _lastResponse;
        public string IdResponse => "conversation.item.created";
        public bool IsLastResponse => _lastResponse;

        public void Dispose() => Debug.Log($"{nameof(RealtimeConversationCreated)} is disposed");

        public void PerformResponse(string data)
        {

#if UNITY_EDITOR
            Debug.Log($"{nameof(IAPIResponseHandler)}: {IdResponse} Result: {data} ");
#endif

            OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.None, data));
        }
    }
}
