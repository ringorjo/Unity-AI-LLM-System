using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI.AIProcedureHelp
{
    public class HelperAIServiceData : ServiceData
    {
        private string _url;
        private string _serviceUrl;
        private string _sessionId;
        private string _clientId;
        private string _input;
        private string _contentType;
        private string _base64Audio;
        private string _moduleId;

        public HelperAIServiceData()
        {
        }

        protected override string BaseURL => $"{_url}";

        public HelperAIServiceData WithUrl(string url)
        {
            _url = url;
            return this;
        }

        public HelperAIServiceData WithServiceUrl(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            return this;
        }

        public HelperAIServiceData WithSessionId(string sessionId)
        {
            _sessionId = sessionId; return this;

        }

        public HelperAIServiceData WithModuleId(string moduleId)
        {
            _moduleId = moduleId;
            return this;
        }

        public HelperAIServiceData WithClientId(string clientId)
        {
            _clientId = clientId; return this;
        }
        public HelperAIServiceData WithContentType(string contentType)
        {
            _contentType = contentType; return this;

        }
        public HelperAIServiceData SetAudioData(string base64)
        {
            _base64Audio = base64;
            return this;
        }

        public HelperAIServiceData SetInputRequest(string input)
        {
            _input = input;
            return this;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("sessionId", _sessionId);
                body.Add("clientId", _clientId);
                body.Add("moduleId", _moduleId);
                body.Add("audio", _base64Audio);
                body.Add("contentType", _contentType);
                body.Add("input", _input);
                return body;
            }
        }

        protected override string ServiceURL => $"/{_serviceUrl}";

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
                headers.Add("Content-Type", "application/json");
                return headers;
            }
        }

        protected override ServiceType ServiceType => ServiceType.POST;
    }

}
