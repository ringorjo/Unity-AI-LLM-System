using System;
using System.Collections;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkHelper
{
    public static async Task<bool> HasInternetConnection(int timeout = 1000)
    {
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                var response = await client.GetAsync("https://www.google.com/generate_204", cts.Token);
                return response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
        }
        catch
        {
            return false;
        }
    }

    public static IEnumerator CheckConnection(int timeout, Action OnConnectionFail=null)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(timeout);
        while (Application.internetReachability != NetworkReachability.NotReachable)
        {
            yield return waitForSeconds;
        }
        OnConnectionFail?.Invoke();

    }
}