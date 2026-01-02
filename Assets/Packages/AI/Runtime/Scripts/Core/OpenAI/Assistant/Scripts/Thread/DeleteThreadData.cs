using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class DeleteThreadData : ServiceData
    {
        private string _token;
        private string _threadId;
        private string _version;

        public DeleteThreadData(string token, string threadId, string version)
        {
            _token = token;
            _threadId = threadId;
            _version = version;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                return param;
            }
        }
        protected override string BaseURL => "https://api.openai.com/v1";

        protected override string ServiceURL => $"/threads/{_threadId}";

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
        protected override ServiceType ServiceType => ServiceType.DELETE;
    }
}