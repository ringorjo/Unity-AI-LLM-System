using Meta.Voice.Audio;
using Meta.WitAi.TTS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class WitTextToSpeech : TTSSpeaker, ITextToSpeechHandler, ITextToSpeechExtension
    {
        private Queue<string> _chunks = new Queue<string>();
        private bool _isRunning;
        private UnityAudioPlayer _unityAudioPlayer;
        private Coroutine _speakCoroutine;
        private EventBusService _busService;

        private void Reset()
        {
            gameObject.AddComponent<UnityAudioPlayer>();
        }
        protected override void Start()
        {
            base.Start();

            _busService = ServiceLocator.Instance.Get<EventBusService>();

            if (GetComponent<UnityAudioPlayer>() == null)
            {
                gameObject.AddComponent<UnityAudioPlayer>();
            }
            _unityAudioPlayer = GetComponent<UnityAudioPlayer>();
        }
        public IEnumerator PerformSpeechAsync(string textToSpeak, Action OnSpeechEnd)
        {
            yield return SpeakAsync(textToSpeak);
        }

        public IEnumerator PerformSpeechAsync(List<string> chunks, Action OnSpeechEnd)
        {
            chunks.ForEach(chunk => _chunks.Enqueue(chunk));
            if (!_isRunning)
            {
                yield return StartCoroutine(SpeakAsyncServiceQueue());
            }
            OnSpeechEnd?.Invoke();
        }

        public void ProcessSpeech(string textToSpeak)
        {
            SpeakQueued(textToSpeak);
        }

        public void EnqueueTextChunks(string chunk, Action OnSpeechEnd = null)
        {
            _chunks.Enqueue(chunk);
            if (!_isRunning)
            {
                _speakCoroutine = StartCoroutine(SpeakAsyncServiceQueue(OnSpeechEnd));
            }
        }

        public void EnqueueTextChunks(List<string> chunks, Action OnSpeechEnd = null)
        {
            chunks.ForEach(chunk => EnqueueTextChunks(chunk, OnSpeechEnd));
        }

        private IEnumerator SpeakAsyncServiceQueue(Action OnSpeechEnd = null)
        {
            _isRunning = true;
            while (_chunks.Count > 0 && _isRunning)
            {
                string chunk = _chunks.Dequeue();
                yield return PerformSpeechAsync(chunk, OnSpeechEnd);
            }
            _isRunning = false;
            OnSpeechEnd?.Invoke();
        }


        [ContextMenu(nameof(TruncateSpeech))]
        public void TruncateSpeech()
        {
            if (_speakCoroutine != null)
            {
                StopCoroutine(_speakCoroutine);
                _speakCoroutine = null;
                _isRunning = false;
                _chunks.Clear();
                Stop();
                _unityAudioPlayer?.Stop();
            }
        }


    }
}

