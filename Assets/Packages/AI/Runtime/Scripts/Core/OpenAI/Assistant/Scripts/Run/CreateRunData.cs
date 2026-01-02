using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class CreateRunData : ServiceData
    {
        private List<Tools> _tools;
        private AssistantData _data;
        public CreateRunData(AssistantData data, List<Tools> tools)
        {
            _data = data;
            _tools = tools;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("assistant_id", _data.AssistantId);
                body.Add("stream", true);
                return body;
            }
        }

        protected override string BaseURL => "https://api.openai.com/v1";
        protected override string ServiceURL => $"/threads/{_data.ThreadId}/runs";

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
                headers.Add("Authorization", $"Bearer {_data.Token}");
                headers.Add("Content-Type", "application/json");
                headers.Add("OpenAI-Beta", _data.AssistantVersion);
                return headers;
            }
        }
        protected override ServiceType ServiceType => ServiceType.POST;
    }
}
