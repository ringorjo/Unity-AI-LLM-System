using System.Collections.Generic;
namespace Services.AI
{
    public class ConversationalAIWebsocketServiceData : WebSocketServiceData
    {
        private string _agentId;
        private string _model;
        private string _conversationalAIUrl;

        public ConversationalAIWebsocketServiceData(string url, string agentId, string model)
        {
            _conversationalAIUrl = url;
            _agentId = agentId;
            _model = model;
        }
        protected override string _serviceURL => _model;
        protected override string _url => $"{_conversationalAIUrl}/{_serviceURL}?agent_id={_agentId}";



        protected override Dictionary<string, string> Headers
        {
            get
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                return header;
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
