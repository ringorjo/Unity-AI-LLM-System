using System;
using UnityEngine;
using SnowKore.Services;
using System.Collections;

namespace Xennial.API
{
    public abstract class Request<T> : MonoBehaviour where T : ServiceData
    {
        protected bool IsNetworkReachable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }
        [SerializeField]
        protected T _requestData;

        protected abstract object[] RequestParams
        {
            get;
        }

        [ContextMenu(nameof(SendRequest))]
        public void SendRequest(Action OnRequestError = null)
        {
            StartCoroutine(SendRequestCoroutine(OnRequestError));
        }

        [ContextMenu(nameof(BuildSendRequest))]
        public void BuildSendRequest(int timeOut = 10)
        {
            StartCoroutine(BuildSendRequestCoroutine(timeOut));
        }

        public void SendStreamRequest()
        {
            StartCoroutine(SendRequestStreamCoroutine());
        }


        public void SendRequestForFile()
        {
            StartCoroutine(SendRequestDataCoroutine());
        }

        private IEnumerator SendRequestCoroutine(Action OnRequestError = null)
        {
            OnRequestSent();
            ServiceData service = (T)Activator.CreateInstance(typeof(T), RequestParams);
            yield return service.SendAsync(Response, OnRequestError);
        }

        private IEnumerator SendRequestStreamCoroutine()
        {
            OnRequestSent();
            ServiceData service = (T)Activator.CreateInstance(typeof(T), RequestParams);
            yield return service.SendAsync(OnResponseReceived, true);
        }


        private IEnumerator SendRequestDataCoroutine()
        {
            OnRequestSent();
            ServiceData service = (T)Activator.CreateInstance(typeof(T), RequestParams);
            yield return service.SendAsync(ResponseData);
        }

        private IEnumerator BuildSendRequestCoroutine(int timeout = 10)
        {
            OnRequestSent();
            yield return _requestData.SendAsync(Response, timeout);
        }

        private void Response(string value)
        {
            OnResponseReceived(value);
        }

        private void ResponseData(byte[] dataRecieved)
        {
            OnResponseReceived(dataRecieved);
        }

        protected virtual void OnRequestSent() { }
        protected abstract void OnResponseReceived(string response);
        protected virtual void OnResponseReceived(byte[] dataRecieved) { }
        protected virtual void OnResponseReceived(string response, ResponseErrorData errorData) { }

    }
}