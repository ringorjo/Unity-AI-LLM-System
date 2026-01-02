using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class CreateAssistantData : ServiceData
    {
        private string _token;
        private string _instruction;
        private string _name;
        private List<Tools> _tools;
        private string _model;
        private string _assistanceVersion;

        public CreateAssistantData(string token, string instruction, string model, string version, string name, List<Tools> tools)
        {
            _token = token;
            _instruction = instruction;
            _model = model;
            _assistanceVersion = version;
            _name = name;
            _tools = tools;
        }

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("instructions", _instruction);
                body.Add("name", _name);
                body.Add("tools", _tools);
                body.Add("model", _model);
                return body;
            }
        }


        protected override string BaseURL => "https://api.openai.com/v1";
        protected override string ServiceURL => "/assistants";

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
