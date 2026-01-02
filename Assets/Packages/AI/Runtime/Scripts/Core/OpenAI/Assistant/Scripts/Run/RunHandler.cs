using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Services.AI
{
    public class RunHandler : SerializedMonoBehaviour, IAPIRequestHandler, ISetAIConfig, IAssistantConfigSeteable
    {
        public event Action<AIAPIResponse> OnRequestCompleted;

        private const string FAILED_RUN_ID = "thread.run.failed";

        [SerializeField]
        private int _timeout = 5;
        [SerializeField, ReadOnly]
        private string _currentRunTypeId;
        [SerializeField]
        private DeleteRunHandler _DeleteRunHandler;
        [SerializeField]
        private List<IAPIResponseHandler> _runResponseHandlers;
        [SerializeField]
        private List<OutputResponseData> _responses = new List<OutputResponseData>();

        private string _url;
        private string _token;
        private string _assistantId;
        private string _threadId;
        private string _version;
        private IAPIRequestHandler _nextHandler;
        private Dictionary<string, IAPIResponseHandler> _runResponsesDictionary = new Dictionary<string, IAPIResponseHandler>();
        private AIConfigData _config;
        private Func<AssistantRepositoryDataUtil> _repository;
        private IAPIResponseHandler _currentResponse;
        public IAPIRequestHandler NextHandler => _nextHandler;

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
        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }
        private void InitResponses()
        {
            foreach (var handler in _runResponseHandlers)
            {
                _runResponsesDictionary.Add(handler.IdResponse, handler);
                handler.OnAPIResponse += OnRequestCompleted;
            }
        }
        [Button]
        private void CreateRun()
        {
            if (string.IsNullOrEmpty(_assistantId))
            {
                Debug.LogError("assistantId is null");
                return;
            }

            _threadId = _repository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
            CleanData();
            if (!string.IsNullOrEmpty(_threadId) && !string.IsNullOrEmpty(_assistantId))
            {
                StartCoroutine(SendPostRequest());
            }
        }

        private void CancelRun()
        {
            _DeleteRunHandler.CancelRun();
        }

        private void PerformRunStreamResponse(OutputResponseData response)
        {
            UpdateCurrentRunType(response.type);

            if (!_runResponsesDictionary.TryGetValue(response.type, out IAPIResponseHandler handler))
                return;

            SetCurrentResponse(handler);

            handler.PerformResponse(response.data);

            if (handler is RunRequiredActionResponse)
                _nextHandler?.HandleRequest();
        }

        private void UpdateCurrentRunType(string newType)
        {
            if (_currentRunTypeId == newType)
                return;

            _currentResponse?.Dispose();
            _currentRunTypeId = newType;
        }

        private void SetCurrentResponse(IAPIResponseHandler handler)
        {
            if (_currentResponse == handler)
                return;

            _currentResponse = handler;

            if (handler is IAssistantConfigSeteable configSeteable)
            {
                configSeteable.SetAssistantConfig(_repository);
            }
        }

        private IEnumerator SendPostRequest()
        {
            string jsonData = $@"{{
            ""assistant_id"": ""{_assistantId}"",
            ""stream"": true
        }}";

            using (UnityWebRequest request = new UnityWebRequest($"{_url}/{_threadId}/runs", "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Authorization", "Bearer " + _token);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("OpenAI-Beta", _version);
                request.timeout = _timeout;
                request.SendWebRequest();
                while (!request.isDone)
                {

                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        RunSplitResponseUtil.SplitResponse(request.downloadHandler.text, PerformStream);
                    }
                    yield return null;
                }

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"|{nameof(RunHandler)}| Request Error : {request.error}");
                    if (_currentRunTypeId == FAILED_RUN_ID)
                    {
                        StartCoroutine(TimeoutHandler());
                        Debug.LogError($"|{nameof(RunHandler)}| Cancel Request and Start Again");
                    }

                }
            }
        }
        private void PerformStream(OutputResponseData response)
        {
            _responses.Add(response);
            PerformRunStreamResponse(response);
        }
        public void Init()
        {
            _config ??= AIConfigUtils.GetConfig();
            _url = $"{_config.Url}/threads";
            _token = _config.Token;
            _version = _config.Version;
            _assistantId = _repository().GetData<string>(AssistanceVariables.ASSISTANT_ID_KEY);
        }

        private void CleanData()
        {
            _responses.Clear();
            _currentRunTypeId = string.Empty;
            RunSplitResponseUtil.CleanData();
        }
        public void HandleRequest(string input = null)
        {
            CreateRun();
        }

        public void OverrideIAConfig(AIConfigData aiconfig)
        {
            _config = aiconfig;
        }

        private IEnumerator TimeoutHandler()
        {
            CancelRun();
            yield return new WaitForSeconds(0.3f);
            CreateRun();
        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }
    }
}