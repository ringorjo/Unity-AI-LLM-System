using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Xennial.Services;


namespace Services.AI
{
    public class ConversationalAIWebsocketRequest : WebSocketRequest<ConversationalAIWebsocketServiceData>, IWebsocketHandler
    {
        public bool WebSocketIsConnected => _connected;

        public const string AGENT_ID_LABEL = "AgentId";

        //public event Action OnWebsocketConnected;
        //public event Action<WebsocketDesconectionType> OnWebsocketClosed;
        //public event Action<string> OnWebSocketResponse;
        //public event Action<Color, string> OnWebSocketStatus;
        public WebSocketServiceData WebSocket => _websocketInstance;

        [SerializeField]
        private bool _connectOnStart;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _resiveMessagesLog;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _sendMessageLog;
        [SerializeField, OnValueChanged(nameof(OnToggleLogs))]
        private bool _allowAutoReconection;
        [SerializeField, ReadOnly]
        private bool _connected;


        private string _url;
        private string _model;
        private string _agentId;
        private bool _evalatingInternetConnection;

        protected override object[] RequestParams
        {
            get { return new object[] { _url, _agentId, _model }; }
        }

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
            _connected = true;
            OnToggleLogs();
            _eventBusService.Broadcast(nameof(WebsocketEventEnums.OnWebsocketConnected));
            _eventBusService.Broadcast(nameof(WebsocketEventEnums.OnWebSocketStatus), Color.green, "Online");
            // OnWebSocketStatus?.Invoke(Color.green, "Online");
            // OnWebsocketConnected?.Invoke();
            EvaluateConnection();
        }

        private void EvaluateConnection()
        {
            if (_evalatingInternetConnection)
                return;
            _evalatingInternetConnection = true;
            StartCoroutine(NetworkHelper.CheckConnection(3, () =>
            {
                OnHandleDesconection(WebsocketDesconectionType.NetworkLoss, "");
                _evalatingInternetConnection = false;

            }));
        }

        protected override void OnHandleDesconection(WebsocketDesconectionType type, string message)
        {
            base.OnHandleDesconection(type, message);

            _connected = false;
            switch (type)
            {
                case WebsocketDesconectionType.Error:
                    _eventBusService?.Broadcast(nameof(WebsocketEventEnums.OnWebSocketStatus), Color.red, "Disconnected due to Internal Error");
                    //OnWebSocketStatus?.Invoke(Color.red, "Disconnected due to Internal Error");
                    break;
                case WebsocketDesconectionType.NetworkLoss:
                    _eventBusService?.Broadcast(nameof(WebsocketEventEnums.OnWebSocketStatus), Color.red, "Offline");
                    // OnWebSocketStatus?.Invoke(Color.red, "Offline");
                    break;
            }
            _eventBusService?.Broadcast(nameof(WebsocketEventEnums.OnWebsocketClosed), type);
           // OnWebsocketClosed?.Invoke(type);
#if UNITY_EDITOR
            Debug.Log("disconnection type: " + type.ToString());
#endif
        }


        protected override void OnWebsocketMessageResponse(string message)
        {
            _eventBusService?.Broadcast(nameof(WebsocketEventEnums.OnWebsocketResponse), message);
            //OnWebSocketResponse?.Invoke(message);
        }


        #endregion

        #region Websocket Methods


        [Button]
        public async Task StartConnection()
        {
            if (!_connected)
            {
                await ConnectToWebsocket();

            }
        }

        [Button]
        public async Task CloseConnection()
        {
            await CloseWebsocketConnection();
            _connected = false;

        }

        public async void SendMessageToWebsocket(string message)
        {
            if (!_connected)
            {
                await ConnectToWebsocket(() => _websocketInstance?.SendMessageToWebsocket(message));
                return;
            }
            _websocketInstance?.SendMessageToWebsocket(message);

        }

        #endregion


        #region Setup Methods
        public override void Init()
        {
            _url = _webscoketConfiguration.Url;
            _model = _webscoketConfiguration.AIModel;
            if (!_webscoketConfiguration.ExistKey(AGENT_ID_LABEL))
                throw new Exception($"The key {AGENT_ID_LABEL} is not defined in the AIConfigData extra data.");
            _agentId = _webscoketConfiguration.GetExtraDataByKey(AGENT_ID_LABEL);
        }

        private void OnToggleLogs()
        {
            if (Application.isPlaying && _websocketInstance != null)
            {
                UpdateLogSetting(ref _websocketInstance.SendMessagesLog, _sendMessageLog);
                UpdateLogSetting(ref _websocketInstance.ReciveMessageLog, _resiveMessagesLog);
                UpdateLogSetting(ref _websocketInstance.AllowReconect, _allowAutoReconection);
            }
        }

        private void UpdateLogSetting(ref bool currentValue, bool newValue)
        {
            if (currentValue != newValue)
                currentValue = newValue;
        }


        public void ForceCloseConnection()
        {
            _websocketInstance.ForceDisconnection();
        }
        #endregion

    }
}

