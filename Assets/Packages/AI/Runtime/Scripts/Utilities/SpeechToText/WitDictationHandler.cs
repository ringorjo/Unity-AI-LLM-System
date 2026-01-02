#if XENNIAL_WIT 
using Oculus.Voice.Dictation;
using System;
using System.Text;
using UnityEngine;

namespace Services.AI
{
    public class WitDictationHandler : MonoBehaviour, ISpeechToTextHandler
    {
        public event Action<string> OnPartialTranscription;
        public event Action<string> OnFullTranscription;

        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private AppDictationExperience _dictation;
        private StringBuilder _fullTranscription;
        private StringBuilder _separator;
        private string _activeText;
        private string transcription;
        private string _activationSeparator = " ";



        private void Awake()
        {
            _separator = new StringBuilder();
            _fullTranscription = new StringBuilder();
            _separator.Append(_activationSeparator);
        }

        private void OnEnable()
        {
            _dictation = Instantiate(_prefab, transform).GetComponent<AppDictationExperience>();
            _dictation.DictationEvents.OnFullTranscription.AddListener(FullTranscriptionPerformed);
            _dictation.DictationEvents.OnPartialTranscription.AddListener(PartialTranscriptionPerformed);
        }
        private void OnDisable()
        {
            _dictation.DictationEvents.OnFullTranscription.RemoveListener(FullTranscriptionPerformed);
            _dictation.DictationEvents.OnPartialTranscription.RemoveListener(PartialTranscriptionPerformed);
            Destroy(_dictation.gameObject);
        }

        private void FullTranscriptionPerformed(string transcription)
        {
            _activeText = string.Empty;
            if (_fullTranscription.Length > 0)
            {
                _fullTranscription.Append(_separator);
            }
            _fullTranscription.Append(transcription);
            UpdateTranscription();

        }

        private void PartialTranscriptionPerformed(string transcription)
        {
            _activeText = transcription;
            UpdateTranscription();
        }

        private void UpdateTranscription()
        {
            var partialTranscription = new StringBuilder();
            partialTranscription.Append(_fullTranscription);
            if (!string.IsNullOrEmpty(_activeText))
            {
                if (partialTranscription.Length > 0)
                {
                    partialTranscription.Append(_separator);
                }
                partialTranscription.Append(_activeText);
            }
            transcription = partialTranscription.ToString();
            OnPartialTranscription?.Invoke(transcription);
        }

        public void StartDictation()
        {
            _dictation?.Activate();
            _fullTranscription.Clear();
            transcription = string.Empty;
        }

        public void StopDictation()
        {
            _dictation?.Deactivate();
            OnFullTranscription?.Invoke(transcription);
        }

        public void Init()
        {
            gameObject.SetActive(true);
            Debug.Log($"{nameof(WitDictationHandler)} Is been Inited");
        }

        public void Dispose()
        {
            gameObject.SetActive(false);
            Debug.Log($"{nameof(WitDictationHandler)} Is been Disposed");
        }
    }
}
#endif
