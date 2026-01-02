using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Services.AI
{
    public class RunCreationResponse : IAPIResponseHandler, IAssistantConfigSeteable
    {
        [SerializeField]
        private bool _lastResponse;
        public event Action<AIAPIResponse> OnAPIResponse;
        public string IdResponse => "thread.run.created";
        private RunCreatedReponse _runCreatedReponse;
        private Func<AssistantRepositoryDataUtil> _repository;

        public bool IsLastResponse => _lastResponse;


        public void Dispose()
        {
            _runCreatedReponse = null;
        }

        public void PerformResponse(string data)
        {
            Debug.Log("RunCreationResponse data: " + data);
            _runCreatedReponse = JsonConvert.DeserializeObject<RunCreatedReponse>(data);
            if (_runCreatedReponse != null)
            {
                _repository().AddDataToRepository(AssistanceVariables.RUN_ID_KEY, _runCreatedReponse.id);
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.None, data));
            }
        }
        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }
    }
}
