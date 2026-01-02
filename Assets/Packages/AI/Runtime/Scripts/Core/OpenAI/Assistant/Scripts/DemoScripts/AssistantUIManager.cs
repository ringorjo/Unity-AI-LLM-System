using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services.AI
{
    public class AssistantUIManager : MonoBehaviour
    {
        [SerializeField]
        private Button _speakBtn;
        [SerializeField]
        private TextMeshProUGUI _transcription;
        [SerializeField]
        private TextMeshProUGUI _btnLabel;
        [SerializeField]
        private AssistantAIHandler _assistantAIHandler;
        private bool _isSpeaking;

        private void Start()
        {
            _speakBtn.onClick.AddListener(OnCLic);
            _assistantAIHandler.SpeechToText.OnPartialTranscription += OnTranscriptionUpdated;
        }

        private void OnTranscriptionUpdated(string message)
        {
            _transcription.text = message;
        }

        private void OnDestroy()
        {
            _speakBtn.onClick.RemoveListener(OnCLic);
            _assistantAIHandler.SpeechToText.OnPartialTranscription -= OnTranscriptionUpdated;
        }

        private void OnCLic()
        {
            _btnLabel.text = !_isSpeaking ? "Stop" : "Speak";

            if (!_isSpeaking)
                _assistantAIHandler.SpeechToText.StartDictation();
            else
                _assistantAIHandler.SpeechToText.StopDictation();
            _isSpeaking = !_isSpeaking;
        }
    }
}
