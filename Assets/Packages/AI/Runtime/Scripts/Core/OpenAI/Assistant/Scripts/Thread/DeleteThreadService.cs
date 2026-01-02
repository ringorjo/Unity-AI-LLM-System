using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Events;
using Xennial.API;
namespace Services.AI
{
    public class DeleteThreadService : Request<DeleteThreadData>, ISetAIConfig, IAssistantConfigSeteable
    {
        private string _token;
        [SerializeField]
        private string _threadId;
        private string _version;
        private AIConfigData _config;
        private Func<AssistantRepositoryDataUtil> _repository;

        public UnityEvent OnThreadDeleted;
        public Action OnThreaDeletedComplete;

        private void Start()
        {
            Init();
        }

        protected override object[] RequestParams
        {
            get { return new object[] { _token, _threadId, _version }; }
        }
        public void DeleteThread()
        {

            _threadId = _repository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
            Debug.Log("Deleting Thread");
            SendRequest();
        }

        protected override void OnResponseReceived(string response)
        {
            DeleteThreadResponse threadresponse = JsonConvert.DeserializeObject<DeleteThreadResponse>(response);
            if (threadresponse != null)
            {
                OnThreadDeleted?.Invoke();
                OnThreaDeletedComplete?.Invoke();
            }
        }

        public void OverrideIAConfig(AIConfigData aiconfig)
        {
            _config = aiconfig;
        }

        public void Init()
        {
            _config ??= AIConfigUtils.GetConfig();
            _token = _config.Token;
            _version = _config.Version;

        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }

        private class DeleteThreadResponse
        {
            public string id;
            public string @object;
            public bool deleted;
        }
    }
}
