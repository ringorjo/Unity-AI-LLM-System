using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class DeleteRunData : ServiceData
    {
        private string _token;
        private string _threadId;
        private string _runId;
        private string _version;
        private string _url;

        public DeleteRunData(string url, string token, string threadId, string runId, string version)
        {
            _url = url;
            _token = token;
            _threadId = threadId;
            _runId = runId;
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
        protected override string BaseURL => _url;

        protected override string ServiceURL => $"/threads/{_threadId}/runs/{_runId}/cancel";

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
