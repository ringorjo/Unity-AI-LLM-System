using Sirenix.OdinInspector;
using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;
namespace Services.AI
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AIProcessorOutputStreamResponseAudioBase : AIOutputResponse
    {
        public UnityEvent<float[], int> OnSamplesUpdated;
        [Header("Audio Settings")]
        [SerializeField]
        private int _inputSampleRate = 24000;
        private int _outputSampleRate;
        [SerializeField, ReadOnly]
        private bool _hasData;
        private ConcurrentQueue<float> _audioBuffer = new ConcurrentQueue<float>();
        private float[] _samples;

        protected virtual void Start()
        {
            _outputSampleRate = AudioSettings.outputSampleRate;
            Debug.Log($"Output Sample Rate: {_outputSampleRate}");  
        }

        private void Update()
        {
            if (!_hasData)
                return;

            if (_audioBuffer.Count >= 0)
            {
                OnSamplesUpdated.Invoke(_samples, 0);

                if (_audioBuffer.Count == 0)
                {
                    OnSamplesUpdated.Invoke(new float[0], 0);
                    _hasData = false;
                }
            }
        }

        private void Reset()
        {
            Type = AIResponseType.Audio;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_audioBuffer.Count <= 0) { return; }
            for (var i = 0; i < data.Length; i += channels)
            {
                if (_audioBuffer.TryDequeue(out var sample))
                {
                    for (var j = 0; j < channels; j++)
                    {
                        int sum = i + j;
                        if (sum < data.Length)
                            data[sum] = sample;
                        else
                            break;
                    }
                }
            }
            _samples = data;
            if (!_hasData)
                _hasData = true;
        }

        public override void ProcessAIResponse(AIAPIResponse response)
        {
            if (response.Type != Type)
                return;

            Debug.Log($"{nameof(AIProcessorOutputStreamResponseAudioBase)}:  Processing Audio Chunk Response");
            OnPerformAudioEncode(response);
        }
        protected virtual void OnPerformAudioEncode(AIAPIResponse response)
        {
            try
            {
                float[] pcmDecodedSamples = WavUtility.PCMDecode(Convert.FromBase64String(response.Text));
                float[] samples = WavUtility.Resample(pcmDecodedSamples, null, _inputSampleRate, _outputSampleRate);
                foreach (float pcmSample in samples)
                    _audioBuffer.Enqueue(pcmSample);
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(AIProcessorOutputStreamResponseAudioBase)}:  Equeue Chunks Error details: {e.Message}");
            }
        }
    }
}