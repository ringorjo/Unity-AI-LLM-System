using System.Collections.Generic;

namespace Services.AI
{
    public class RealtimeWSServiceData : WebSocketServiceData
    {
        private string _token;
        private string _realtimeVersion;
        private string _realtimeUrl;
        private string _model;

        public RealtimeWSServiceData(string realtimeUrl, string token, string model, string realtimeVersion)
        {
            _realtimeUrl = realtimeUrl;
            _token = token;
            _model = model;
            _realtimeVersion = realtimeVersion;
        }

        protected override string _url => $"{_realtimeUrl}?model={_serviceURL}";

        protected override string _serviceURL => _model;
        protected override Dictionary<string, string> Headers
        {
            get
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", $"Bearer {_token}");
                headers.Add("OpenAI-Beta", _realtimeVersion);
                return headers;
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
    }
}