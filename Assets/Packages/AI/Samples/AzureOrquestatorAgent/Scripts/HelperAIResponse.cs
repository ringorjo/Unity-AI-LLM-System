using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Services.AI.AIProcedureHelp
{
    public class HelperAIResponse : SerializedMonoBehaviour
    {
        [SerializeField]
        private HelperAIRequest _helperAIRequest;
        [SerializeField]
        private ITextToSpeechExtension _tts;

        private void Start()
        {
            //_helperAIRequest.OnHelperResponse += OnAIResponse;
        }
        private void OnDestroy()
        {
            //_helperAIRequest.OnHelperResponse -= OnAIResponse;
        }

        private void OnAIResponse(DefaultHelperAIResponse response)
        {
            PerformSpeech(response.text_to_speech);
        }

        private void PerformSpeech(string speech)
        {
            List<string> chunks = StringChunkBuilder.ChunkString(speech);
            _tts?.EnqueueTextChunks(chunks);
        }
    }
}
