using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.AI
{
    public class RunRequiredActionResponse : IAPIResponseHandler, IAssistantConfigSeteable
    {
        [SerializeField]
        private bool _lastResponse;
        public event Action<AIAPIResponse> OnAPIResponse;
        private RunCreatedReponse _runCreatedResponse;
        private List<ToolCall> _functionsCallOuput = new List<ToolCall>();
        private List<string> _functionsIds = new List<string>();
        private Func<AssistantRepositoryDataUtil> _repository;

        public string IdResponse => "thread.run.requires_action";

        public bool IsLastResponse => _lastResponse;

        public void Dispose()
        {
            _runCreatedResponse = null;
            _functionsIds.Clear();
            _functionsCallOuput.Clear();
        }

        public void PerformResponse(string data)
        {
            _runCreatedResponse = JsonConvert.DeserializeObject<RunCreatedReponse>(data);
            if (_runCreatedResponse?.required_action?.submit_tool_outputs?.tool_calls != null)
            {
                _functionsCallOuput = _runCreatedResponse.required_action.submit_tool_outputs.tool_calls;
                _functionsIds = _functionsCallOuput.Select(call =>
               {
                   OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.Json_extraData, call.function.arguments, _lastResponse));
                   return call.id;
               }).ToList();

                _repository()?.AddDataToRepository(AssistanceVariables.RUN_CALL_ID, _functionsIds);
            }
        }
        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }
    }
}