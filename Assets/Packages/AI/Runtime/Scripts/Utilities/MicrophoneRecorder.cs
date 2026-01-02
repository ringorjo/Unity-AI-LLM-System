using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.AI
{
    public class MicrophoneRecorder : MonoBehaviour
    {
        public virtual event Action<float> OnMicMaxSampleUpdated;
        public virtual event Action OnStartRecording;
        public virtual event Action OnStopRecording;
        public virtual event Action<string> OnBase64EncodeGenerated;
        public virtual event Action<byte[]> OnGetRecordData;

        [SerializeField]
        protected int _frequency = 24000;

        [SerializeField]
        private bool _initOnStart = true;
        [SerializeField]
        protected bool _enableStream;
        [SerializeField, ShowIf(nameof(_enableStream))]
        [Header("Streaming Settings")]
        protected int _bufferSize = 4096;
        private List<float> _streamingBuffer = new List<float>();

        [Header("Auto Recording"), HideIf(nameof(_enableStream))]
        [SerializeField]
        private bool _autoRecording = true;

        [Header("Mic Settings")]
        [SerializeField, Range(0, 1)]
        protected float _updateTime;
        [SerializeField, Range(0, 0.1f), HideIf(nameof(_enableStream))]
        protected float _minActivationThreshold = 0.05f;
        [SerializeField, Range(0, 0.01f), HideIf(nameof(_enableStream))]
        protected float _minRecordingTreshold = 0.01f;
        [SerializeField, HideIf(nameof(_enableStream))]
        protected float _keepAlive = 3;
        [SerializeField]
        protected bool _saveRecordOnStop;
        [SerializeField]
        private bool _useEcho;
        [SerializeField]
        private AudioClip _output;
        [SerializeField, ReadOnly]
        protected float maxSample;
        private AudioSource _audioSource;
        protected bool _isRecording;
        protected string _base64;
        private int _lastSamplePosition;
        private List<float> _recorderData = new List<float>();
        private byte[] _audioBuffer;
        protected float _keepAliveCurrentTime;
        private bool _allowRecord = false;
        private MicDataSenderMediator _micDataSenderMediator;

        public float MaxSample
        {
            get => maxSample;
        }
        public bool IsStreaming
        {
            get => _enableStream;
            set => ChangeMicSendData(value);
        }

        public bool IsMicEnabled
        {
            get => _allowRecord;
        }

        private void ChangeMicSendData(bool streaming)
        {
            _enableStream = streaming;
            _keepAliveCurrentTime = 0;
        }

        protected virtual void Reset()
        {
            _frequency = 24000;
            _updateTime = 0.3f;
            _minActivationThreshold = 0.03f;
            _minRecordingTreshold = 0.01f;
            _bufferSize = 4096;
            _saveRecordOnStop = true;
        }

        private void Awake()
        {
            _micDataSenderMediator = new MicDataSenderMediator(this);

#if UNITY_ANDROID
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
            {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
            }
#endif

        }

        protected virtual void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            StartMicService();
            if (_initOnStart)
                Init();
        }
        private void OnDestroy()
        {
            _micDataSenderMediator?.Unregister();
        }

        [Button]
        public void Init()
        {
            _allowRecord = true;
        }

        [Button]
        public void Dispose()
        {
            _allowRecord = false;
        }

        private void StartMicService()
        {
            _output = Microphone.Start(null, true, 1, _frequency);

            if (_output == null)
            {
                Debug.LogError("Microphone initialization failed.");
                return;
            }

            Debug.Log("Microphone initialization.");

            StartCoroutine(BufferUpdate());
        }

        private static void StopMicService()
        {
            if (Microphone.IsRecording(null))
                Microphone.End(null);
        }

        private IEnumerator BufferUpdate()
        {
            Debug.Log("BufferUpdate started.");

            while (Microphone.IsRecording(null))
            {
                if (_allowRecord)
                {
                    int currentPosition = Microphone.GetPosition(null);
                    int sampleCount = currentPosition - _lastSamplePosition;

                    if (sampleCount < 0)
                        sampleCount += _output.samples;

                    if (sampleCount > 0)
                    {
                        float[] samples = new float[sampleCount * _output.channels];
                        _output.GetData(samples, _lastSamplePosition);

                        maxSample = CalculateRMS(samples);
                        OnMicMaxSampleUpdated?.Invoke(maxSample);

                        if (_enableStream)
                            PerformStream(samples);
                        else
                            PerformRecordAndProcess(samples);

                        _lastSamplePosition = currentPosition;
                    }
                }
                yield return new WaitForSeconds(_updateTime);
            }

            Debug.Log("BufferUpdate stopped.");
        }

        protected virtual void PerformStream(float[] samples)
        {
            _streamingBuffer.AddRange(samples);
            if (_streamingBuffer.Count >= _bufferSize)
            {
                EncodeToBase64(_streamingBuffer);
                _streamingBuffer.Clear();
                PerformEcho();
            }
        }

        private float CalculateRMS(float[] samples)
        {
            float sum = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += samples[i] * samples[i];
            }
            return Mathf.Sqrt(sum / samples.Length);
        }

        protected virtual void PerformRecordAndProcess(float[] samples)
        {
            if (maxSample >= _minActivationThreshold)
            {
                if (!_isRecording && _autoRecording)
                {
                    StartRecord();
                }
            }

            if (_isRecording)
            {
                if (maxSample > _minRecordingTreshold)
                    _keepAliveCurrentTime = Time.time;

                _recorderData.AddRange(samples);
                if (Time.time - _keepAliveCurrentTime > _keepAlive && _autoRecording)
                {
                    StopRecord();
                }
            }
        }


        private void OnDisable()
        {
            StopMicService();
        }

        [Button]
        public virtual void StartRecord()
        {
            _isRecording = true;
            _recorderData.Clear();
            OnStartRecording?.Invoke();
            Debug.Log("Recording started.: " + maxSample);
        }

        [Button]
        public virtual void StopRecord()
        {
            _isRecording = false;

            if (_saveRecordOnStop)
                SaveRecordBuffer();
            EncodeToBase64(_recorderData);
            OnStopRecording?.Invoke();
            PerformEcho();

            Debug.Log("Recording stopped.");
        }
        public void RestoreValues()
        {
            _autoRecording = true;
            _enableStream = false;
        }
        protected virtual void EncodeToBase64(List<float> samples)
        {
            _base64 = Base64EncodeTool.AudioClipToBase64(samples.ToArray());

            if (!string.IsNullOrEmpty(_base64))
            {
                OnBase64EncodeGenerated?.Invoke(_base64);
            }
            else
                Debug.LogError("BASE64 string is null or empty.");
        }

        private void SaveRecordBuffer()
        {
            _audioBuffer = WavUtility.FromSamplesToBytes(_recorderData);
            if (_audioBuffer.Length == 0)
            {
                Debug.LogError("is no  posible save Byte data due AudioBuffer is Null ");
                return;
            }
            byte[] wavData = WavUtility.ConvertToWav(_audioBuffer, _frequency, 1);
            OnGetRecordData?.Invoke(wavData);
            _audioBuffer = new byte[0];
        }

        protected void PerformEcho()
        {
#if UNITY_EDITOR

            if (_useEcho)
            {
                AudioClip audioClip = Base64EncodeTool.Base64ToAudioClip(_base64, _frequency);
                _audioSource.clip = audioClip;
                _audioSource.Play();
            }
#endif
        }
    }
}