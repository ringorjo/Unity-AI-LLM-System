using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Xennial.API;

namespace Services.AI
{
    public class ElevenLabsGetVoices : Request<ElevenLabsVoiceServiceData>
    {
        [SerializeField]
        private AIConfigData _config;
        [SerializeField]
        private ElevenLabVoices _elevenLabVoicesData;
        private ElevenLabsVoiceResponse _voicesResponse;
        private string _url;
        private string _token;
        private string _serviceURL;
        protected override object[] RequestParams
        {
            get { return new object[] { _url, _serviceURL, _token }; }
        }

        protected override void OnResponseReceived(string response)
        {

            _voicesResponse = JsonConvert.DeserializeObject<ElevenLabsVoiceResponse>(response);

            if (_voicesResponse != null)
            {
                _voicesResponse.voices.ForEach(voice => _elevenLabVoicesData.AddVoice(voice));
            }

        }

        [Button]
        private void GetVoices()
        {
            if (_config == null)
            {
                Debug.LogError("AIConfigData is not set. Please assign it in the inspector.");
                return;
            }
            if (string.IsNullOrEmpty(_url))
            {
                _url = _config.Url;
                _serviceURL = _config.AIModel;
                _token = _config.Token;
            }

            SendRequest();
        }

    }
}

