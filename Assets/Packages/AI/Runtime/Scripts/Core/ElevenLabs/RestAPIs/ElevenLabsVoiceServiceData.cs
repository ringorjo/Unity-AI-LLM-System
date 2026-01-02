using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class ElevenLabsVoiceServiceData : ServiceData
    {
        private string _url;
        private string _token;
        private string _serviceURL;

        public ElevenLabsVoiceServiceData(string url, string serviceURL, string token)
        {
            _url = url;
            _serviceURL = serviceURL;
            _token = token;
        }

        protected override string BaseURL => _url;
        protected override string ServiceURL => $"/{_serviceURL}";

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
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
        protected override ServiceType ServiceType => ServiceType.GET;
    }
}

