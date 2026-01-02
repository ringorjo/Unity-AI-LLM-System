using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Xennial.API;

namespace Services.AI
{
    public class AssistantMessageHandler : Request<SendUserMessageData>, IAPIRequestHandler, ISetAIConfig, IAssistantConfigSeteable
    {
        public event Action<AIAPIResponse> OnRequestCompleted;

        [SerializeField]
        private string _message;
        [SerializeField]
        private bool _sendRequestOnStart;
        private IAPIResponseHandler _responseHander;
        private UserMessageSendResponse userMessageSendResponse;
        private string _threadId;
        private string _assistantVersion;
        private IAPIRequestHandler _nextHandler;
        private string _url;
        private string _token;

        public IAPIRequestHandler NextHandler => _nextHandler;
        private AIConfigData _config;
        private Func<AssistantRepositoryDataUtil> _repository;

        protected override object[] RequestParams
        {
            get
            {
                return new object[]
            {
                _url,
                _token,
                _message,
                _threadId,
                _assistantVersion,
            };
            }
        }

        public IAPIResponseHandler ResponseHandler => _responseHander;

        private void Start()
        {
            Init();
            if (_sendRequestOnStart)
                Invoke(nameof(InitRequest), 1);
        }
        private void InitRequest()
        {
            SendUserMessage(_message);
        }

        [Button]
        private void SendUserMessage()
        {
            if (_repository().DataExist(AssistanceVariables.THREAD_ID_KEY))
            {
                _threadId = _repository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
                SendRequest();
                Debug.Log($"Message {_message} Send with thread Id ' {_threadId}");
            }
        }

        private void SendUserMessage(string message)
        {
            _message = message;
            SendUserMessage();
        }
        protected override void OnResponseReceived(string response)
        {
            userMessageSendResponse = JsonConvert.DeserializeObject<UserMessageSendResponse>(response);
            if (userMessageSendResponse != null)
            {
                _nextHandler?.HandleRequest();
                OnRequestCompleted?.Invoke(new AIAPIResponse(AIResponseType.None, response));
            }
        }

        public void HandleRequest(string input = null)
        {
            SendUserMessage(input);
        }

        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
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