using Sirenix.OdinInspector;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class RealtimeSessionUpdate : AIWSRequestHandlerBase
    {
        [SerializeField, PropertyOrder(-1)]
        private SessionUpdateData _sessionUpdate;
        [SerializeField, PropertyOrder(-2)]
        private bool _initMicOnSessionUpdated;
        private const string SESSION_CREATED = "SessionCreated";

        [Button]
        public override void HandleRequest(string input = null)
        {
            _realtimeWS.SendMessageToWebsocket(_sessionUpdate.GetJson());
        }
        protected override void OnHandlerRequestResponse(AIAPIResponse response)
        {

            if (response.Type != AIResponseType.AIResponse)
                return;

            if (response.Text == SESSION_CREATED)
            {
                HandleRequest();
            }
           
        }

        public override void Init()
        {
            base.Init();
            _sessionUpdate.session.instructions = _config.SystemPrompt;
            if (_config.UseFunctionCallTemplate && _config.FunctionCallData != null)
                _sessionUpdate.session.tools = _config.FunctionCallData.tools;
        }
    }
}
