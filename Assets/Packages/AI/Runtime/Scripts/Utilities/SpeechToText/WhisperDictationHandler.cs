using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Xennial.Services;

namespace Services.AI
{
    public class WhisperDictationHandler : SerializedMonoBehaviour, ISpeechToTextHandler
    {
        public event Action<string> OnPartialTranscription;
        public event Action<string> OnFullTranscription;
        [SerializeField]
        private IRecieverMicDataCollaborator _micDataCollaborator;
        [SerializeField]
        private bool _initMicOnStart = true;
        [SerializeField, ReadOnly]
        private AudioClip _audioClip;
        [SerializeField]
        private AIConfigData _config;
        private string _trascriptionModel;
        private string _transcriptionLanguage;


        private void Start()
        {
            if (_micDataCollaborator != null)
                _micDataCollaborator.OnGetRecordData += RecieveByteData;
        }
        private void OnDestroy()
        {
            if (_micDataCollaborator != null)
                _micDataCollaborator.OnGetRecordData -= RecieveByteData;
        }
        private IEnumerator UploadAudio(byte[] data)
        {

            WWWForm form = new WWWForm();
            form.AddField("model", _trascriptionModel);
            form.AddField("language", _transcriptionLanguage);
            form.AddBinaryData("file", data, "audio.wav");
            var request = UnityWebRequest.Post($"{_config.Url}/audio/transcriptions", form);
            request.SetRequestHeader("Authorization", "Bearer " + _config.Token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DictationResponse dictationResponse = JsonConvert.DeserializeObject<DictationResponse>(request.downloadHandler.text);
                if (dictationResponse != null)
                {
                    OnPartialTranscription?.Invoke(dictationResponse.text);
                    OnFullTranscription?.Invoke(dictationResponse.text);

                    Debug.Log("Trascription: " + dictationResponse.text);
                }
            }
            else
            {
                Debug.LogError("Error: " + request.error + "\n" + request.downloadHandler.text);
            }
        }

        public void StartDictation()
        {
            ServiceLocator.Instance.Get<MicDataSenderMediator>().StartRecording();
        }

        public void StopDictation()
        {
            ServiceLocator.Instance.Get<MicDataSenderMediator>().StopRecordinfg();
        }


        public void Dispose() => Debug.Log($"{nameof(WhisperDictationHandler)} Is been Disposed");

        private void RecieveByteData(byte[] data)
        {
            if (_config == null)
            {
                Debug.LogError("AI Config is Null, please add configuration file first");
                return;

            }
            _trascriptionModel = _config.GetExtraDataByKey("TranscriptionModel");
            _transcriptionLanguage = _config.GetExtraDataByKey("TranscriptionLanguage");
            if (string.IsNullOrEmpty(_trascriptionModel))
            {
                Debug.LogError($"{nameof(WhisperDictationHandler)} Error: The trascription model does not exist please check Extra data in AIConfig file");
                return;
            }
            StartCoroutine(UploadAudio(data));
        }

        public void Init()
        {

        }

        private class DictationResponse
        {
            public string text;
        }
    }
}

