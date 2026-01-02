using Sirenix.OdinInspector;
using UnityEngine;
namespace Services.AI
{
    public class RealtimeResponseCreate : AIWSRequestHandlerBase
    {
        [SerializeField, PropertyOrder(-1)]
        private ResponseCreateData _responseCreate;

        #region Handlers
        [Button]
        public override void HandleRequest(string input = null)
        {
            _realtimeWS?.SendMessageToWebsocket(_responseCreate.GetJson());
        }
        #endregion
        public override void Init()
        {
            base.Init();
            _responseCreate.response.instructions = _config.SystemPrompt;
            if (_config.UseFunctionCallTemplate && _config.FunctionCallData != null)
                _responseCreate.response.tools = _config.FunctionCallData.tools;
        }
    }
}