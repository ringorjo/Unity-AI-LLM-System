using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class SubmitRunData : ServiceData
    {
        private string _token;
        private string _runid;
        private string _threadId;
        private string _version;
        private string _url;
        private List<ToolOutput> _toolOutputs;

        public SubmitRunData(string runid, string threadId, List<ToolOutput> toolOutputs)
        {
            _url = AIConfigUtils.GetConfig().Url;
            _token = AIConfigUtils.GetConfig().Token;
            _version = AIConfigUtils.GetConfig().Version;
            _runid = runid;
            _threadId = threadId;
            _toolOutputs = toolOutputs;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("tool_outputs", _toolOutputs);
                return body;
            }
        }


        protected override string BaseURL => _url;
        protected override string ServiceURL => $"/threads/{_threadId}/runs/{_runid}/submit_tool_outputs";

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
