using SnowKore.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.AI
{
    public class ElevenLabsTTSServiceData : ServiceData
    {
        private string _url;
        private string _token;
        private string _voiceID;
        private string _audioFormat= "pcm_16000";
        private string _modelID;
        private string _input;

        public ElevenLabsTTSServiceData(AIConfigData aIConfigData, string voiceID, string input)
        {
            _url = aIConfigData.Url;
            _token = aIConfigData.Token;
            _voiceID = voiceID;
            _modelID = aIConfigData.AIModel;
            _input = input;
        }

        protected override string BaseURL => $"{_url}";
        protected override string ServiceURL => $"/{_voiceID}?output_format={_audioFormat}";

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("text", $"{_input}");
                body.Add("model_id", $"{_modelID}");
                return body;
            }
        }

        protected override Dictionary<string, object> Params
        {
            get
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                return param;
            }
        }
        protected override Dictionary<string, string> Headers
        {
            get
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("xi-api-key", $"{_token}");
                headers.Add("Content-Type", "application/json");
                return headers;
            }
        }
        protected override ServiceType ServiceType => ServiceType.POST;
    }
}

