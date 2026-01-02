using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public abstract class AIHandlerBase : SerializedMonoBehaviour, IAIHandler, IAIHandlerExtention
    {
        public virtual event Action<AIAPIResponse> OnAISingleResponse;
        public virtual event Action<AIAPIResponse> OnAIStreamingResponse;

        public AIConfigData GetConfigData => _aiConfigData;


        [SerializeField]
        private bool _useAService;
        [SerializeField]
        private AIConfigData _aiConfigData;
        [SerializeField]
        [Title("API Requests")]
        protected List<IAPIRequestHandler> _responsabilityAPIRequests = new List<IAPIRequestHandler>();
        [Title("ISetAIConfig Implementation List")]

        [SerializeField]
        private List<ISetAIConfig> _apisOverrideConfigurationList;

        public List<ISetAIConfig> ApisOverrideConfigurationList { get => _apisOverrideConfigurationList; set => _apisOverrideConfigurationList = value; }




        #region Lifecycle
        protected virtual void Awake()
        {
            if (_useAService)
                Register();
            NotifyModulesConfig();

            InitRequestChain();
        }
        protected virtual void OnDestroy()
        {
            if (_useAService)
                Unregister();

            foreach (var handler in _responsabilityAPIRequests)
                handler.OnRequestCompleted -= UpdateResponse;

        }
        public void Register()
        {
            ServiceLocator.Instance.Register(this as IAIHandler);
        }

        public void Unregister()
        {
            ServiceLocator.Instance.Unregister(this as IAIHandler);

        }
        #endregion
        public void InjectConfig(AIConfigData config)
        {
            _aiConfigData = config;
            NotifyModulesConfig();
        }
        private void NotifyModulesConfig()
        {
            if (_aiConfigData.UpdateFromUrl)
            {
                _aiConfigData.UpdatePromptFromUrl(PerformOverrideConfiguration);
            }
            else
                PerformOverrideConfiguration();
        }
        private void InitRequestChain()
        {
            for (int i = 0; i < _responsabilityAPIRequests.Count; i++)
            {
                _responsabilityAPIRequests[i].OnRequestCompleted += UpdateResponse;
                if (i + 1 < _responsabilityAPIRequests.Count)
                {
                    _responsabilityAPIRequests[i].SetNextHandler(_responsabilityAPIRequests[i + 1]);
                }
            }
        }
        private void HandleSingleResponse(AIAPIResponse response)
        {
            OnAISingleResponse?.Invoke(response);
        }

        protected virtual void UpdateResponse(AIAPIResponse response)
        {
            if (response.IsSingleResponse)
                HandleSingleResponse(response);
            else
                OnAIStreamingResponse?.Invoke(response);
        }

        private void PerformOverrideConfiguration()
        {
            foreach (var api in _apisOverrideConfigurationList)
            {
                api.OverrideIAConfig(_aiConfigData);
            }
        }

        public void PerformOverrideConfiguration(AIConfigData aIConfig)
        {
            foreach (var api in _apisOverrideConfigurationList)
            {
                api.OverrideIAConfig(aIConfig);
            }
        }



        #region Editor Methods
        [Button]
        private void SetResponsabilityAPIRequestsList()
        {
            _responsabilityAPIRequests = transform.GetComponentsInChildren<IAPIRequestHandler>().ToList();
        }
        #endregion
        [Button]
        public abstract void Input(string input);




    }
}