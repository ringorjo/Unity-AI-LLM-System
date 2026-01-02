using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class CreateThreadData : ServiceData
    {
        private string _token;
        private string _assistantVersion;
        private string _url;

        public CreateThreadData(string url, string token, string assistantVersion)
        {
            _token = token;
            _assistantVersion = assistantVersion;
            _url = url;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                return param;
            }
        }

        protected override string BaseURL => _url;
        protected override string ServiceURL => "/threads";

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
                headers.Add("Authorization", $"Bearer {_token}");
                headers.Add("Content-Type", "application/json");
                headers.Add("OpenAI-Beta", _assistantVersion);
                return headers;
            }
        }

        protected override ServiceType ServiceType => ServiceType.POST;
    }
}

