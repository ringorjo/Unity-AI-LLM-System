using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class CreateThreadAndRunData : ServiceData
    {
        private string _url;
        private string _token;
        private string _version;
        private string _assistantId;
        private AssistantThread _request;

        public CreateThreadAndRunData(string url, string token, string version, string assistantId, AssistantThread request)
        {
            _url = url;
            _token = token;
            _version = version;
            _assistantId = assistantId;
            _request = request;
        }

        protected override string BaseURL => _url;
        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("assistant_id", _assistantId);
                body.Add("thread", _request);
                body.Add("stream", true);
                return body;
            }
        }
        protected override string ServiceURL => $"/threads/runs";

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
                headers.Add("OpenAI-Beta", _version);
                return headers;
            }
        }

        protected override ServiceType ServiceType => ServiceType.POST;
    }
}

