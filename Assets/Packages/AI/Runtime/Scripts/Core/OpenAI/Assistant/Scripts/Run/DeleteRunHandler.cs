using Newtonsoft.Json;
using System;
using UnityEngine;
using Xennial.API;
namespace Services.AI
{
    public class DeleteRunHandler : Request<DeleteRunData>, ISetAIConfig, IAssistantConfigSeteable
    {

        private string _threadId;
        private string _runId;
        private AIConfigData _aiConfig;
        private Func<AssistantRepositoryDataUtil> _repository;
        private string _url;
        private string _token;
        private string _version;

        protected override object[] RequestParams
        {
            get { return new object[] { _url, _token, _threadId, _runId, _version }; }
        }

        private void Start()
        {
            Init();
        }
        protected override void OnResponseReceived(string response)
        {
            RunCreatedReponse runCreatedReponse = JsonConvert.DeserializeObject<RunCreatedReponse>(response);

        }

        public void CancelRun()
        {
            _threadId = _repository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
            _runId = _repository().GetData<string>(AssistanceVariables.RUN_ID_KEY);
            SendRequest();
        }

        public void OverrideIAConfig(AIConfigData aiconfig)
        {
            _aiConfig = aiconfig;
        }

        public void Init()
        {
            _aiConfig ??= AIConfigUtils.GetConfig();
            _url = _aiConfig.Url;
            _token = _aiConfig.Token;
            _version = _aiConfig.Version;

        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
          

        }
    }
}
