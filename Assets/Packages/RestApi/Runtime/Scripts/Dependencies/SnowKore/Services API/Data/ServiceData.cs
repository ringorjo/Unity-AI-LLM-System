using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using SnowKore.Utils;

namespace SnowKore.Services
{
    public abstract class ServiceData
    {
        private Action<string> LastServiceResponse;
        private Action<byte[]> LastServiceResponseData;

        protected virtual string BaseURL => ServerURL.BaseUrl;
        protected abstract Dictionary<string, object> Body { get; }
        protected abstract string ServiceURL { get; }
        protected abstract Dictionary<string, object> Params { get; }
        protected abstract Dictionary<string, string> Headers { get; }
        protected abstract ServiceType ServiceType { get; }

        private byte[] JsonBody => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Body));

        private string RequestURL => BaseURL + ServiceURL;

        protected virtual bool ShouldSimulateResponse => false;

        public IEnumerator SendAsync(Action<string> ServiceResponse, Action OnConectionError = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Send request " + ServiceURL + " with body: \n" + JsonConvert.SerializeObject(Body));
#endif
            LastServiceResponse = ServiceResponse;
            using (UnityWebRequest request = GetRequest())
            {
                yield return request.SendWebRequest();
                ProcessResponse(request, ServiceResponse, OnConectionError);
            }
        }

        public IEnumerator SendAsync(Action<string> ServiceResponse, int timeout, Action OnConectionError = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Send request " + ServiceURL + " with body: \n" + JsonConvert.SerializeObject(Body));
#endif
            LastServiceResponse = ServiceResponse;
            using (UnityWebRequest request = GetRequest())
            {
                request.timeout = timeout;
                yield return request.SendWebRequest();
                ProcessResponse(request, ServiceResponse, OnConectionError);
            }
        }

        public IEnumerator SendAsync(Action<byte[]> ServiceResponse)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Send request " + ServiceURL + " with body: \n" + JsonConvert.SerializeObject(Body));
#endif
            LastServiceResponseData = ServiceResponse;
            using (UnityWebRequest request = GetRequest())
            {
                yield return request.SendWebRequest();
                ProcessResponse(request, ServiceResponse);
            }
        }

        public IEnumerator SendAsync(Action<string, ResponseErrorData> ServiceResponse, bool streamResponse)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Send request " + ServiceURL + " with body: \n" + JsonConvert.SerializeObject(Body));
#endif

            using (UnityWebRequest request = GetRequest())
            {
                request.timeout = 5;

                if (!streamResponse)
                {
                    yield return request.SendWebRequest();
                    ProcessResponse(request, ServiceResponse);
                }
                else
                {
                    string previousStreamData = string.Empty;
                    request.SendWebRequest();

                    while (!request.isDone || !string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        string currentData = request.downloadHandler.text;
                        if (!string.IsNullOrEmpty(currentData) && currentData != previousStreamData)
                        {
                            previousStreamData = currentData;
                            ProcessResponse(request, ServiceResponse);
                        }
                        yield return null;
                    }


                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"|{nameof(ServiceData)}| Request Error: {request.error}, Response: {request.downloadHandler.text}");
                        ProcessResponse(request, ServiceResponse);
                    }
                }
            }
        }


        public void Send(Action<string> ServiceResponse)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Send request " + ServiceURL + " with body: \n" + JsonConvert.SerializeObject(Body));
#endif
            LastServiceResponse = ServiceResponse;
            using (UnityWebRequest request = GetRequest())
            {
                request.SendWebRequest();
                while (!request.isDone) { }
                ProcessResponse(request, ServiceResponse);
            }
        }

        private void ProcessResponse(UnityWebRequest request, Action<string> ServiceResponse, Action OnConectionError = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD 
            Debug.Log("Request: " + request.url + ", Response Code: [" + request.responseCode + "]" + " Response Text: \n" + request.downloadHandler.text + "Response result: " + request.result);
#endif

            if (HasFailed(request))
            {
                bool hasConnection = request.result != UnityWebRequest.Result.ConnectionError;

                if (!hasConnection)
                    OnConectionError?.Invoke();
                else
                    ServiceResponse?.Invoke(request.downloadHandler.text);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("Service " + RequestURL + " Error:\n" + request.downloadHandler.text + "\nHasInternetConnection? " + hasConnection);
#endif
            }
            else
            {
                ServiceResponse?.Invoke(request.downloadHandler.text);
            }
        }

        private void ProcessResponse(UnityWebRequest request, Action<string, ResponseErrorData> ServiceResponse)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("Request: " + request.url + ", Response Code: [" + request.responseCode + "]" + " Response Text: \n" + request.downloadHandler.text);
#endif

            if (HasFailed(request))
            {
                bool hasConnection = request.result != UnityWebRequest.Result.ConnectionError;
                if (!hasConnection)
                    Debug.LogError("No Internet!");
                else
                    ServiceResponse?.Invoke(request.downloadHandler.text, new ResponseErrorData(request.responseCode, request.error));

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("Service " + RequestURL + " Error:\n" + request.downloadHandler.text + "\nHasInternetConnection? " + hasConnection);
#endif
            }
            else
            {
                ServiceResponse?.Invoke(request.downloadHandler.text, new ResponseErrorData(request.responseCode, request.error));
            }
        }


        private void ProcessResponse(UnityWebRequest request, Action<byte[]> ServiceResponse)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD 
            Debug.Log("Request: " + request.url + ", Response Code: [" + request.responseCode + "]" + " Response Text: \n" + request.downloadHandler.text);
#endif

            if (HasFailed(request))
            {
                bool hasConnection = request.result != UnityWebRequest.Result.ConnectionError;

                if (!hasConnection)
                    Debug.LogError("No Internet!");
                else
                    ServiceResponse?.Invoke(request.downloadHandler.data);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("Service " + RequestURL + " Error:\n" + request.downloadHandler.text + "\nHasInternetConnection? " + hasConnection);
#endif
            }
            else
            {
                ServiceResponse?.Invoke(request.downloadHandler.data);
            }
        }


        private UnityWebRequest GetRequest()
        {
            UnityWebRequest request = null;

            if (ServiceType == ServiceType.POST || ServiceType == ServiceType.PATCH || ServiceType == ServiceType.DELETE || ServiceType == ServiceType.PUT)
                request = GetRequestTypePost();

            if (ServiceType == ServiceType.GET)
                request = GetRequestTypeGet();

            if (request != null)
            {
                foreach (KeyValuePair<string, string> header in Headers)
                    request.SetRequestHeader(header.Key, header.Value);
            }

            return request;
        }

        private UnityWebRequest GetRequestTypePost()
        {
            UnityWebRequest request = UnityWebRequest.Post(RequestURL, new List<IMultipartFormSection>(), JsonBody);
            request.uploadHandler = new UploadHandlerRaw(JsonBody);
            request.method = ServiceType.ToString();
            return request;
        }

        private UnityWebRequest GetRequestTypeGet()
        {
            string finalUrl = RequestURL + Params.ToURL();
            return UnityWebRequest.Get(finalUrl);
        }

        private bool HasFailed(UnityWebRequest request)
        {
            bool isNetworkError = request.result == UnityWebRequest.Result.ConnectionError;
            bool isHttpError = request.result == UnityWebRequest.Result.ProtocolError;
            bool hasFailed = isNetworkError || isHttpError;
            return hasFailed;
        }
    }

    public class ServerURL
    {
        public static string BaseUrl;
    }

    public class ResponseErrorData
    {
        public long Code;
        public string Error;

        public ResponseErrorData(long code, string error)
        {
            this.Code = code;
            Error = error;
        }
    }

}