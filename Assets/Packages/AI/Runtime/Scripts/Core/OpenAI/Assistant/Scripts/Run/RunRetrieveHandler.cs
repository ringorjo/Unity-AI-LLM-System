using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using Services.AI;
using Xennial.API;

public class RunRetrieveHandler : Request<RunRetrieveData>, ISetAIConfig, IAssistantConfigSeteable
{
    [SerializeField]
    private RunCreatedReponse respone;
    private AIConfigData _config;
    private Func<AssistantRepositoryDataUtil> _assistantRepository;
    private string _url;
    private string _token;
    private string _runId;
    private string _threadId;
    private string _version;
    private Action _onRunCancelled;

    protected override object[] RequestParams
    {
        get { return new object[] { _url, _token, _version, _threadId, _runId }; }
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        _config ??= AIConfigUtils.GetConfig();
        _url = _config.Url;
        _token = _config.Token;
        _version = _config.Version;
    }

    public void UpdateRunStatus(Action OnRunCancelledCompleted)
    {

        _threadId = _assistantRepository().GetData<string>(AssistanceVariables.THREAD_ID_KEY);
        _runId = _assistantRepository().GetData<string>(AssistanceVariables.RUN_ID_KEY);
        if (_runId == null || _threadId == null)
        {
            string error = string.IsNullOrEmpty(_runId) ? "RunId is Null" : "threadId Is null";
            Debug.LogError(error);
            return;
        }
        _onRunCancelled = OnRunCancelledCompleted;
        StopAllCoroutines();
        StartCoroutine(UpdateRunState());
        // SendRequest();
    }

    public void OverrideIAConfig(AIConfigData aiconfig)
    {
        _config = aiconfig;
    }

    protected override void OnResponseReceived(string response)
    {
        respone = JsonConvert.DeserializeObject<RunCreatedReponse>(response);
    }
    private IEnumerator UpdateRunState()
    {
        while (respone.status != "cancelled")
        {
            Debug.Log("Run Status:" + respone.status);
            SendRequest();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config)
    {
        _assistantRepository = config;
    }
}
