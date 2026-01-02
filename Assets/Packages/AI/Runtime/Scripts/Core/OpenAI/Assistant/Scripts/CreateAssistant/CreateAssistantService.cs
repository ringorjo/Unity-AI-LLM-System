using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Xennial.API;

namespace Services.AI
{
    public class CreateAssistantService : Request<CreateAssistantData>, IAssistantConfigSeteable
    {
        [SerializeField]
        [MultiLineProperty(10)]
        private string _systemInstruction;
        [SerializeField]
        private string _assistantName;
        [SerializeField]
        private FunctionCallData _tools;
        [SerializeField, ReadOnly]
        private string _assistantId;

        private string _token;
        private string _model;
        private string _version;
        private AIConfigData _assistantOpenAIConfig;
        private Func<AssistantRepositoryDataUtil> _assistantRepository;


        protected override object[] RequestParams
        {
            get { return new object[] { _token, _systemInstruction, _model, _version, _assistantName, _tools.tools }; }
        }

        private void Start()
        {
            _assistantRepository().AddDataToRepository(AssistanceVariables.ASSISTANT_ID_KEY, _assistantId);
        }

        [Button]
        private void CreateAssistance()
        {
            Setup();
            SendRequest();
        }

        private void Setup()
        {
            _assistantOpenAIConfig = Resources.Load<AIConfigData>("AssistantOpenAIConfig");
            _token = _assistantOpenAIConfig.Token;
            _model = _assistantOpenAIConfig.AIModel;
            _version = _assistantOpenAIConfig.Version;
        }

        protected override void OnResponseReceived(string response)
        {
            AssistanceCreateResponse assistantCreateResponse = JsonConvert.DeserializeObject<AssistanceCreateResponse>(response);
            if (assistantCreateResponse != null)
            {
                Debug.Log("Assistant Create with id: " + assistantCreateResponse.id);
                _assistantId = assistantCreateResponse.id;
            }
        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _assistantRepository = config;
        }
    }
}
