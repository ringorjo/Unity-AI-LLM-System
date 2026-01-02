using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Xennial.Services;
namespace Services.AI
{
    public abstract class WebSocketRequest<T> : MonoBehaviour, ISetAIConfig where T : WebSocketServiceData
    {
        protected WebSocketServiceData _websocketInstance;
        private readonly SemaphoreSlim _connectionBlock = new SemaphoreSlim(1);
        private bool _isconnecting;
        protected AIConfigData _webscoketConfiguration;
        protected EventBusService _eventBusService;

        protected abstract object[] RequestParams
        {
            get;
        }

        protected virtual void Start()
        {
          
            if (ServiceLocator.Instance.Exist<EventBusService>())
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
            else
                Debug.LogError("EventBus Service is not register in the ServiceLocator");
        }
        protected virtual void OnDestroy() => UnsuscribeEvents();
       
        protected abstract void OnWebsocketMessageResponse(string message);

        protected virtual void OnWebSocketConnectionOpen() => PerformLog("Websocket Connected");
       
        protected virtual void OnHandleDesconection(WebsocketDesconectionType type, string message) => PerformLog($"Websocket disconnected reason: {type}");
        
        protected async Task ConnectToWebsocket(Action OnConnected = null)
        {

            await _connectionBlock.WaitAsync();
            try
            {

                if (_websocketInstance.IsConnected || _isconnecting)
                    return;
                _isconnecting = true;
                PerformLog("Websocket Connecting");
                if (await _websocketInstance.Connect())
                {
                    OnConnected?.Invoke();
                    _connectionBlock.Release();
                    await _websocketInstance.WebsocketMessageReceived();
                }
            }
            finally
            {
                _connectionBlock.Release();
                _isconnecting = false;
            }

        }

        protected async Task CloseWebsocketConnection()
        {
            if (_websocketInstance != null)
            {
                await _websocketInstance.CloseWebSocket();
            }
        }

        protected void SendMessageToServer(string message)=> _websocketInstance.SendMessageToWebsocket(message);
        

        public virtual void OverrideIAConfig(AIConfigData aiconfig)
        {
            _webscoketConfiguration = aiconfig;
            Init();
            CreateInstance();

        }

        private void CreateInstance()
        {
            if (_websocketInstance != null)
                UnsuscribeEvents();

            _websocketInstance = (T)Activator.CreateInstance(typeof(T), RequestParams);

            SuscribeEvent();
        }

        private void SuscribeEvent()
        {
            if (_websocketInstance == null)
                return;

            Debug.Log("Me suscribo");
            _websocketInstance.OnDesconection += OnHandleDesconection;
            _websocketInstance.OnConnectionOpen += OnWebSocketConnectionOpen;
            _websocketInstance.OnMessageReceived += OnWebsocketMessageResponse;
        }

        private void UnsuscribeEvents()
        {
            if (_websocketInstance == null)
                return;

            _websocketInstance.OnDesconection -= OnHandleDesconection;
            _websocketInstance.OnConnectionOpen -= OnWebSocketConnectionOpen;
            _websocketInstance.OnMessageReceived -= OnWebsocketMessageResponse;
        }
        public abstract void Init();

        private void PerformLog(string message)=> Debug.Log(message);
       

        private TValue GetServiceValue<TService, TValue>(Func<TService, TValue> service, TValue defaululValue = default) where TService : IService
        {
            if (ServiceLocator.Instance.Exist<TService>())
            {
                return service(ServiceLocator.Instance.Get<TService>());
            }
            return defaululValue;
        }
    }
}