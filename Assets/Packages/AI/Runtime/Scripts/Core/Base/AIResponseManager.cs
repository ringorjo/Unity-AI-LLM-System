using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.AI
{
    public class AIResponseManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private IAIHandler _iaHandler;
        [SerializeField]
        private List<AIOutputResponse> _aiStreamResponses;
        [SerializeField]
        private List<AIOutputResponse> _aiSingleResponses;

        private void Reset()
        {
            _aiStreamResponses = transform.GetComponentsInChildren<AIOutputResponse>().ToList();
        }

        private void Start()
        {
            if (_iaHandler != null)
            {
                _iaHandler.OnAIStreamingResponse += OnPerformStreamingResponses;
                _iaHandler.OnAISingleResponse += OnPerformAISingleResponse;
            }
        }

        private void OnPerformAISingleResponse(AIAPIResponse response) => _aiSingleResponses.ForEach(r => r.ProcessAIResponse(response));

        private void OnPerformStreamingResponses(AIAPIResponse response)
        {
            foreach (var aiResponse in _aiStreamResponses)
            {
                if (aiResponse.Type == response.Type)
                {
                    aiResponse.ProcessAIResponse(response);
                }
            }
        }
        private void OnDestroy()
        {
            if (_iaHandler != null)
            {
                _iaHandler.OnAIStreamingResponse -= OnPerformStreamingResponses;
            }
        }

    }
}
