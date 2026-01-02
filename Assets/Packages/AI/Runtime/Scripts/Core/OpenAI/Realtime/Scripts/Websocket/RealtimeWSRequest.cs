using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class RealtimeWSRequest : WebSocketRequest<RealtimeWSServiceData>, IWebsocketHandler
    {
        public event Action<string> OnWebSocketResponse;
        public event Action<Color, string> OnWebSocketStatus;
        public event Action OnWebsocketConnected;
        public event Action<WebsocketDesconectionType> OnWebsocketClosed;

        [SerializeField]
        private bool _connectOnStart;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _resiveMessagesLog;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _sendMessageLog;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _allowAutoReconection;
        private string _url;
        private string _model;
        private string _token;
        private string _realtimeVersion;
        private bool _connected;
        protected override object[] RequestParams
        {
            get { return new object[] { _url, _token, _model, _realtimeVersion }; }
        }

        public WebSocketServiceData WebSocket => _websocketInstance;

        public bool WebSocketIsConnected => _connected;

        #region LifeCycle
        private void Awake()
        {
            Register();
        }

        protected override void Start()
        {
            base.Start();
            if (_connectOnStart)
                Invoke(nameof(StartConnection), 1f);

        }
        protected async override void OnDestroy()
        {
            Unregister();
            await CloseConnection();
            base.OnDestroy();
        }

        public void Register()
        {
            ServiceLocator.Instance.Register(this as IWebsocketHandler);
        }

        public void Unregister()
        {
            ServiceLocator.Instance.Unregister(this as IWebsocketHandler);
        }
        #endregion

        #region Websocket Events

        protected override void OnWebSocketConnectionOpen()
        {
            base.OnWebSocketConnectionOpen();
            OnToggleLogs();
            OnWebSocketStatus?.Invoke(Color.green, "Connected");
            OnWebsocketConnected?.Invoke();
            _connected = true;
        }

        protected override void OnHandleDesconection(WebsocketDesconectionType type, string message)
        {
            base.OnHandleDesconection(type, message);
            switch (type)
            {
                case WebsocketDesconectionType.Error:
                    OnWebSocketStatus?.Invoke(Color.red, "Disconnected");
                    break;
                case WebsocketDesconectionType.NetworkLoss:
                    OnWebSocketStatus?.Invoke(Color.red, "Disconnected");
                    break;
            }
            OnWebsocketClosed?.Invoke(type);
        }


        protected override void OnWebsocketMessageResponse(string message)
        {
            // OnWebSocketResponse?.Invoke(message);
            _eventBusService?.Broadcast(nameof(WebsocketEventEnums.OnWebsocketResponse), message);

        }
        #endregion

        private void OnToggleLogs()
        {
            if (Application.isPlaying && _websocketInstance != null)
            {
                UpdateLogSetting(ref _websocketInstance.SendMessagesLog, _sendMessageLog);
                UpdateLogSetting(ref _websocketInstance.ReciveMessageLog, _resiveMessagesLog);
                UpdateLogSetting(ref _websocketInstance.AllowReconect, _allowAutoReconection);
            }
        }

        [Button]
        public async Task StartConnection()
        {
            await ConnectToWebsocket();
        }

        [Button]
        public async Task CloseConnection()
        {
            await CloseWebsocketConnection();
            _connected = false;

        }
        public override void Init()
        {
            _token = _webscoketConfiguration.Token;
            _realtimeVersion = _webscoketConfiguration.Version;
            _url = _webscoketConfiguration.Url;
            _model = _webscoketConfiguration.AIModel;
        }


        public void SendMessageToWebsocket(string message)
        {
            _websocketInstance?.SendMessageToWebsocket(message);
        }
        private void UpdateLogSetting(ref bool currentValue, bool newValue)
        {
            if (currentValue != newValue)
                currentValue = newValue;
        }

        public void ForceCloseConnection()
        {
            _websocketInstance?.ForceDisconnection();
        }




    }
}
