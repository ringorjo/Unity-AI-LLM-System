using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Services.AI
{
    public class RunTextToSpeechResponse : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        [SerializeField]
        private bool _lastResponse;
        [SerializeField]
        private int _initBuffer = 5;
        public string IdResponse => "thread.message.delta";
        private string _speechText;
        private ChunkMessageResponse _chunkMessage;
        public bool IsLastResponse => _lastResponse;
        [SerializeField, ReadOnly]
        private bool _initBufferUsed;
        private int _bufferCount;

        public void Dispose()
        {
            _initBufferUsed = false;
            _bufferCount = 0;
            _speechText = string.Empty;
            _chunkMessage = null;
        }

        public void PerformResponse(string data)
        {
            _chunkMessage = JsonConvert.DeserializeObject<ChunkMessageResponse>(data);
            if (_chunkMessage != null)
            {
                AppendMessage(_chunkMessage.delta.content[0].text.value);
                return;
            }
            AppendMessage(data);
        }

        private void AppendMessage(string chunkMessage)
        {
            bool isEndOfSentence = Regex.IsMatch(chunkMessage, @"[.?!]");

            if (!_initBufferUsed)
            {
                HandleInitialBuffer(chunkMessage);
                return;
            }

            _speechText += chunkMessage;

            if (isEndOfSentence)
                FlushSpeechText();
        }

        private void HandleInitialBuffer(string chunkMessage)
        {
            _speechText += chunkMessage;
            _bufferCount++;

            if (_bufferCount >= _initBuffer)
            {
                _initBufferUsed = true;
                FlushSpeechText();
            }
        }

        private void FlushSpeechText()
        {
            if (!string.IsNullOrEmpty(_speechText))
            {
#if UNITY_EDITOR
                Debug.Log("Chunk: " + _speechText);
#endif
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.AIResponse, _speechText));
                _speechText = string.Empty;
            }
        }
    }
}
