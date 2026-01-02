using UnityEngine;

namespace Services.AI
{
    public class AIProcessorOutputResponseActionBase : AIOutputResponse
    {
        [SerializeReference]
        private IFunctionInvokeHandler _functionInvokeHandler;

        public override void ProcessAIResponse(AIAPIResponse response)
        {
            if (response.Type != AIResponseType.Json_extraData)
                return;

            _functionInvokeHandler?.InvokeFunction(response.Text);
        }
    }

}
