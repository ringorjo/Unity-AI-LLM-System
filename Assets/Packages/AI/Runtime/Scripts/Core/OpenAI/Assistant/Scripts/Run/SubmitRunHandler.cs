using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Services.AI
{
    public class SubmitRunHandler : SerializedMonoBehaviour, IAPIRequestHandler, ISetAIConfig, IAssistantConfigSeteable
    {
        public event Action<AIAPIResponse> OnRequestCompleted;

        [HideInInspector]
        public Action<OutputResponseData> OnRunStreamingResponseUpdated;

        [HideInInspector]
        public Action OnTimeoutResponseHandler;
        private IAPIResponseHandler _responseHander;
        [SerializeField]
        private List<OutputResponseData> _responses = new List<OutputResponseData>();

        private string _url;
        private string _token;
        private string _version;
        private string _threadId;
        private string _runId;
        [SerializeField]
        private List<string> _runFunctionCallId;
        string jsonData;
        private List<ToolOutput> _toolOutput;
        private IAPIRequestHandler _nextHandler;
        private AIConfigData _config;
        private Func<AssistantRepositoryDataUtil> _repository;

        public IAPIRequestHandler NextHandler => _nextHandler;

        public IAPIResponseHandler ResponseHandler => _responseHander;



        private void Start()
        {
            Init();
        }
        public void Init()
        {
            _config ??= AIConfigUtils.GetConfig();
            _token = _config.Token;
            _version = _config.Version;
        }

        private IEnumerator SendRequest()
        {
            _url = $"{_config.Url}/threads/{_threadId}/runs/{_runId}/submit_tool_outputs";
            jsonData = JsonConvert.SerializeObject(new SubmitRunData(_toolOutput));

            using (UnityWebRequest request = new UnityWebRequest(_url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Authorization", "Bearer " + _token);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("OpenAI-Beta", _version);
                request.timeout = 10;
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
                    Debug.LogError($"|{nameof(RunHandler)}| Request Error : {request.error} {request.downloadHandler.text}");
                    if (request.error == "Request timeout")
                    {
                        OnTimeoutResponseHandler?.Invoke();
                    }
                }
                else
                {
                    OnRequestCompleted?.Invoke(new AIAPIResponse(AIResponseType.None, string.Empty));
                }
            }
        }
        private void PerformStream(OutputResponseData response)
        {
            _responses.Add(response);
            OnRunStreamingResponseUpdated?.Invoke(response);
        }
        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public void HandleRequest(string input = null)
        {
            _threadId = _repository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
            _runId = _repository().GetData<string>(AssistanceVariables.RUN_ID_KEY);
            _runFunctionCallId = new List<string>(_repository().GetData<List<string>>(AssistanceVariables.RUN_CALL_ID));
            _toolOutput = new List<ToolOutput>();
            foreach (var item in _runFunctionCallId)
            {
                _toolOutput.Add(new ToolOutput(item, "true"));
            }
            RunSplitResponseUtil.CleanData();
            _responses.Clear();
            StartCoroutine(SendRequest());
        }

        public void OverrideIAConfig(AIConfigData aiconfig)
        {
            _config = aiconfig;
        }

        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
        {
            _repository = config;
        }

        private class SubmitRunData
        {
            public List<ToolOutput> tool_outputs;
            public bool stream;

            public SubmitRunData(List<ToolOutput> tool_outputs)
            {
                this.tool_outputs = tool_outputs;
                stream = true;
            }
        }
    }
}
