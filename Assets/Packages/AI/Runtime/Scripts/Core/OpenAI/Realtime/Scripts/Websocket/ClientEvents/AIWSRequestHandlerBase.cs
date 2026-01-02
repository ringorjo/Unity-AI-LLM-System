using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public abstract class AIWSRequestHandlerBase : SerializedMonoBehaviour, IAPIRequestHandler, ISetAIConfig
    {
        public virtual event Action<AIAPIResponse> OnRequestCompleted;
        [Title("Response Handlers")]
        [SerializeField]
        private List<IAPIResponseHandler> _responseHandlers;

        protected IWebsocketHandler _realtimeWS;
        protected Dictionary<string, IAPIResponseHandler> _responses = new Dictionary<string, IAPIResponseHandler>();
        protected IAPIResponseHandler _currentResponseHandler= default;
        protected IAPIRequestHandler _nextHandler;
        [SerializeField, ReadOnly]
        protected AIConfigData _config;
        private EventBusService _eventBusService;


        public IAPIRequestHandler NextHandler => _nextHandler;

        #region Lifecycle
        protected virtual void Start()
        {
            _realtimeWS = ServiceLocator.Instance.Get<IWebsocketHandler>();
            // _realtimeWS.OnWebSocketResponse += OnWBResponse;
            foreach (var handler in _responseHandlers)
            {
                _responses.Add(handler.IdResponse, handler);
                handler.OnAPIResponse += OnHandlerRequestResponse;
            }

            if (ServiceLocator.Instance.Exist<EventBusService>())
            {
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
                _eventBusService.Subscribe<string>(nameof(WebsocketEventEnums.OnWebsocketResponse), OnWBResponse);
            }
            else
            {
                Debug.LogError($"{nameof(AIWSRequestHandlerBase)} Error: EventBus Service is not register in the ServiceLocator ");
            }
        }

        protected virtual void OnDestroy()
        {
            //_realtimeWS.OnWebSocketResponse -= OnWBResponse;
            if (_eventBusService != null)
                _eventBusService.Unsubscribe<string>(nameof(WebsocketEventEnums.OnWebsocketResponse), OnWBResponse);

            foreach (var handler in _responseHandlers)
            {
                handler.OnAPIResponse -= OnHandlerRequestResponse;
            }
        }

        public virtual void Init()
        {
            if (_config == null)
                _config = AIConfigUtils.GetConfig();
        }
        #endregion

        #region Response Methods
        protected virtual void OnWBResponse(string message)
        {
            BaseJsonStructure baseJsonStructure = JsonConvert.DeserializeObject<BaseJsonStructure>(message);
            if (baseJsonStructure != null)
            {
                OutputResponseData outputResponseData = new OutputResponseData();
                outputResponseData.type = baseJsonStructure.type;
                outputResponseData.data = message;
                PerformResponse(outputResponseData);
            }
        }
        protected virtual void PerformResponse(OutputResponseData outputResponseData)
        {
            if (_responses.TryGetValue(outputResponseData.type, out IAPIResponseHandler result))
            {
                result.PerformResponse(outputResponseData.data);
                if (result != _currentResponseHandler)
                {
                    _currentResponseHandler?.Dispose();
                    Debug.Log($"Switched Response Handler to: {result.IdResponse}");
                }
                _currentResponseHandler = result;

            }
        }

        protected virtual void OnHandlerRequestResponse(AIAPIResponse response)
        {
            OnRequestCompleted?.Invoke(response);
        }
        #endregion

        public virtual void OverrideIAConfig(AIConfigData aiconfig)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(gameObject, "Override AI Config");
#endif
            _config = aiconfig;
            Init();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif

        }

        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public abstract void HandleRequest(string input = null);
    }
}