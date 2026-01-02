using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Xennial.API;

namespace Services.AI
{
    public class CreateThread : Request<CreateThreadData>, ISetAIConfig, IAssistantConfigSeteable
    {
        [SerializeField]
        private string _threadId;
        [SerializeField]
        private bool _initThreadOnStart;
        private string _url;
        private string _token;
        private string _assistantVersion;
        private AIConfigData _config;
        private Func<AssistantRepositoryDataUtil> _repository;

        protected override object[] RequestParams
        {
            get { return new object[] { _url, _token, _assistantVersion }; }
        }

        private void Start()
        {
            Init();
            if (_initThreadOnStart)
                InitThread();
        }

        [Button]
        public void InitThread()
        {
            if (string.IsNullOrEmpty(_config.Token))
                throw new Exception("The token is null can not send Request");

            SendRequest();
        }

        protected override void OnResponseReceived(string response)
        {
            ThreadCreatedResponse threadCreatedResponse = JsonConvert.DeserializeObject<ThreadCreatedResponse>(response);

            if (threadCreatedResponse != null)
            {
                if (Application.isPlaying)
                {
                    _threadId = threadCreatedResponse.id;
                    _repository().AddDataToRepository(AssistanceVariables.THREAD_ID_KEY, _threadId);
                }
            }
        }

        public void OverrideIAConfig(AIConfigData aiconfig)
        {
            _config = aiconfig;
        }

        public void Init()
        {
            _config ??= AIConfigUtils.GetConfig();
            _url = _config.Url;
            _token = _config.Token;
            _assistantVersion = _config.Version;
        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }
    }
}
