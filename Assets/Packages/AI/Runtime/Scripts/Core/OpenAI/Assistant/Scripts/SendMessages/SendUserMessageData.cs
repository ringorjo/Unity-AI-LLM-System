using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class SendUserMessageData : ServiceData
    {
        private const string ROLE = "user";
        private string _token;
        private string _content;
        private string _threadId;
        private string _assistanceVersion;
        private string _url;

        public SendUserMessageData(string url, string token, string content, string threadId, string assistanceVersion)
        {
            _token = token;
            _content = content;
            _threadId = threadId;
            _assistanceVersion = assistanceVersion;
            _url = url;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("role", ROLE);
                body.Add("content", _content);
                return body;
            }
        }


        protected override string BaseURL => _url;
        protected override string ServiceURL => $"/threads/{_threadId}/messages";

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
                headers.Add("OpenAI-Beta", _assistanceVersion);
                return headers;
            }
        }
        protected override ServiceType ServiceType => ServiceType.POST;
    }
}