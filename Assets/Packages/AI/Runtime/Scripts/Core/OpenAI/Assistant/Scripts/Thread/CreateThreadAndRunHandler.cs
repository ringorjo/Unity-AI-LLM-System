using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SnowKore.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xennial.API;
namespace Services.AI
{
    [ShowOdinSerializedPropertiesInInspector]
    public class CreateThreadAndRunHandler : Request<CreateThreadAndRunData>, IAPIRequestHandler, ISetAIConfig, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        public event Action<AIAPIResponse> OnRequestCompleted;

        private const string ASSISTANT_KEY = "AssistantId";
        private const string COMPLETED_RUN_ID = "thread.run.completed";
        private const string DONE_RUN_ID = "done";

        [SerializeField, HideInInspector]
        private SerializationData serializationData;
        [SerializeField]
        private float _timeOutWait = 5f;
        [SerializeField]
        private float _maxAttemptsToReconect = 3f;
        [SerializeField, ReadOnly]
        private string _currentResponseType;
        [SerializeField, ReadOnly]
        private bool _isBusy;
        [SerializeField]
        private List<IAPIResponseHandler> _runResponseHandlers;

        private string _url;
        private string _token;
        private string _version;
        private string _assistantId;
        private int _attemptCount;


        private AssistantThread _request;
        private IAPIRequestHandler _nextHandler;
        private IAPIResponseHandler _responseHandler;
        private Dictionary<string, IAPIResponseHandler> _runResponsesDictionary = new Dictionary<string, IAPIResponseHandler>();
        private AIConfigData _config;

        public IAPIRequestHandler NextHandler => _nextHandler;
        public IAPIResponseHandler ResponseHandler => _responseHandler;

        protected override object[] RequestParams
        {
            get { return new object[] { _url, _token, _version, _assistantId, _request }; }
        }

        SerializationData ISupportsPrefabSerialization.SerializationData { get { return this.serializationData; } set { this.serializationData = value; } }

        private void Start()
        {
            Init();
            InitResponses();
        }

        private void OnDestroy()
        {
            foreach (var handler in _runResponseHandlers)
            {
                handler.OnAPIResponse -= OnRequestCompleted;
            }
        }
        private void InitResponses()
        {
            foreach (var handler in _runResponseHandlers)
            {
                _runResponsesDictionary.Add(handler.IdResponse, handler);
                handler.OnAPIResponse += OnRequestCompleted;
            }
        }
        private void PerformRunStreamResponse(OutputResponseData response)
        {
            if (_runResponsesDictionary.TryGetValue(response.type, out IAPIResponseHandler result))
            {
                _responseHandler = result;
                result.PerformResponse(response.data);
                if (result is RunRequiredActionResponse)
                    _nextHandler?.HandleRequest();
            }
        }
        private void CleanData()
        {
            _currentResponseType = string.Empty;
            _isBusy = false;
            RunSplitResponseUtil.CleanData();
        }

        public void HandleRequest(string input = null)
        {
            if (_isBusy)
                return;
            CleanData();
            if (_config.ExistKey(ASSISTANT_KEY))
                _assistantId = _config.GetExtraDataByKey(ASSISTANT_KEY);
            _request = new AssistantThread(new List<Message> { new(input) });
            StartCoroutine(TimeoutHandler());
            SendStreamRequest();
            _isBusy = true;
        }

        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        protected override void OnResponseReceived(string response)
        {

        }

        protected override void OnResponseReceived(string response, ResponseErrorData errorData)
        {
            if (errorData.Code == 200)
            {
                if (!string.IsNullOrEmpty(response))
                {
                    RunSplitResponseUtil.SplitResponse(response, PerformStream);
                }
                _attemptCount = 0;
            }
        }
        private void PerformStream(OutputResponseData response)
        {
            if (response != null)
            {
                if (response.type != "done")
                    _currentResponseType = response.type;
                PerformRunStreamResponse(response);
                if (IsRunThreadCompleted())
                    _isBusy = false;
            }
        }

        private bool IsRunThreadCompleted()
        {
            return _currentResponseType == COMPLETED_RUN_ID;
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
            _version = _config.Version;
        }

        private IEnumerator TimeoutHandler()
        {
            yield return new WaitForSeconds(_timeOutWait);
            if (!IsRunThreadCompleted())
            {
                _attemptCount++;
                CleanData();
                SendStreamRequest();
                _isBusy = true;
                Debug.Log("Request Time out");
                if (_attemptCount < _maxAttemptsToReconect)
                {
                    StartCoroutine(TimeoutHandler());
                    yield break;
                }
                _attemptCount = 0;
                Debug.LogError($"{nameof(CreateThreadAndRunHandler)}: Reconnect Attempts Exceeded");


            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }
    }
}