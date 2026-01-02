using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xennial.API;

namespace Services.AI
{
    [RequireComponent(typeof(AudioSource))]
    public class OpenAITextToSpeech : Request<SpeechData>, ITextToSpeechHandler, ITextToSpeechExtension
    {
        [SerializeField]
        private OpenAIVoices voice;
        [SerializeField]
        private int _inputSampleRate = 24000;
        [SerializeField]
        private int _outputSampleRate = 46000;
        [SerializeField]
        private AIConfigData _config;
        private string _token;
        [SerializeField]
        private string _input;
        private bool _isPlaying = false;
        private Queue<string> _chunks = new Queue<string>();
        private ConcurrentQueue<float> _audioBuffer = new ConcurrentQueue<float>();
        private bool _isRunning;
        protected override object[] RequestParams
        {
            get { return new object[] { _input, voice.ToString(), _token }; }
        }
        protected override void OnResponseReceived(string response) { }

        [Button]
        public void StartSpeech()
        {
            SendRequestForFile();
        }
        private void Start()
        {
            Setup();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            try
            {
                if (_audioBuffer.Count <= 0) { return; }
                for (var i = 0; i < data.Length; i += channels)
                {
                    if (_audioBuffer.TryDequeue(out var sample))
                    {
                        for (var j = 0; j < channels; j++)
                        {
                            data[i + j] = sample;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(OpenAITextToSpeech)}:  Dequeue Chunks Error details: {e.Message}");
            }
        }

        public void StartSpeech(string input)
        {
            _input = input;
            StartCoroutine(PerformSpeechAsync(input));
        }

        protected override void OnResponseReceived(byte[] dataRecieved)
        {
            float[] samples = WavUtility.Resample(WavUtility.PCMDecode(dataRecieved), null, _inputSampleRate, _outputSampleRate);
            foreach (float pcmSample in samples)
                _audioBuffer.Enqueue(pcmSample);
            _isPlaying = true;

        }
        public IEnumerator PerformSpeechAsync(string textToSpeak, Action OnSpeechEnd = null)
        {
            ProcessSpeech(textToSpeak);

            while (!_isPlaying)
            {
                yield return new WaitForSeconds(0.1f);
            }

            _isPlaying = false;
        }

        public void ProcessSpeech(string textToSpeak)
        {
            _input = textToSpeak;
            SendRequestForFile();
        }

        private void Setup()
        {
            if (_config == null)
            {
                Debug.LogError("IAConfig Is null");
                return;
            }
            _token = _config.Token;
        }

        public void EnqueueTextChunks(string chunk, Action OnSpeechEnd = null)
        {
            _chunks.Enqueue(chunk);
            if (!_isRunning)
            {
                StartCoroutine(SpeakAsyncServiceQueue());
            }
        }

        private IEnumerator SpeakAsyncServiceQueue()
        {
            _isRunning = true;
            while (_chunks.Count > 0)
            {
                string chunk = _chunks.Dequeue();
                yield return PerformSpeechAsync(chunk);
            }
            _isRunning = false;
        }

        public void EnqueueTextChunks(List<string> chunks, Action OnSpeechEnd = null)
        {
            chunks.ForEach(chunk => EnqueueTextChunks(chunk));
        }

        public void TruncateSpeech()
        {
            _audioBuffer.Clear();
        }

        public IEnumerator PerformSpeechAsync(List<string> chunks, Action OnSpeechEnd = null)
        {
            chunks.ForEach(chunk => _chunks.Enqueue(chunk));
            if (!_isRunning)
            {
                yield return StartCoroutine(SpeakAsyncServiceQueue());
            }
        }
    }
}


