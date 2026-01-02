using SnowKore.Services;
using System.Collections.Generic;

public class RunRetrieveData : ServiceData
{
    private string _url;
    private string _token;
    private string _threadId;
    private string _rundId;
    private string _version;

    public RunRetrieveData(string url, string token, string version, string threadId, string rundId)
    {
        _url = url;
        _token = token;
        _version = version;
        _threadId = threadId;
        _rundId = rundId;
    }

    protected override Dictionary<string, object> Body
    {
        get
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            return body;
        }
    }

    protected override string BaseURL => _url;
    protected override string ServiceURL => $"/threads/{_threadId}/runs/{_rundId}";

    protected override Dictionary<string, object> Params
    {
        get
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            return param;
        }
    }
    protected override Dictionary<string, string> Headers
    {
        get
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_token}");
            headers.Add("Content-Type", "application/json");
            headers.Add("OpenAI-Beta", _version);
            return headers;
        }
    }

    protected override ServiceType ServiceType => ServiceType.GET;
}
