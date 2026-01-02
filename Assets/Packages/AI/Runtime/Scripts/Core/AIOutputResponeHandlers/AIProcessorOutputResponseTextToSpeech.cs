using System.Collections.Generic;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class AIProcessorOutputResponseTextToSpeech : AIOutputResponse
    {
        [SerializeField]
        private ITextToSpeechHandler _textToSpeech;
        private ITextToSpeechExtension _ttsExtension;
        private EventBusService _eventBusService;

        private void Start()
        {
            _ttsExtension = _textToSpeech as ITextToSpeechExtension;
            _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
            _eventBusService.Subscribe(nameof(AIEventEnums.TruncateSpeech), TruncateSpeech);
        }
        private void OnDestroy()
        {
            if (_eventBusService != null)
                _eventBusService.Unsubscribe(nameof(AIEventEnums.TruncateSpeech), TruncateSpeech);

        }

        private void Reset()
        {
            Type = AIResponseType.AIResponse;
        }
        public override void ProcessAIResponse(AIAPIResponse response)
        {
            if (response.Type != Type)
                return;
            List<string> chunks = StringChunkBuilder.ChunkString(response.Text);
            _ttsExtension?.EnqueueTextChunks(chunks, OnSpeechEnd);
        }

        private void OnSpeechEnd()
        {
            _eventBusService.Broadcast(nameof(OnSpeechEnd));
            Debug.Log("Termino El Speech");
        }

        public void TruncateSpeech()
        {
            _textToSpeech?.TruncateSpeech();
        }
    }
}