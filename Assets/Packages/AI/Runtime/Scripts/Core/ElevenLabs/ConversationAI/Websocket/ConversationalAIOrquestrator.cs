using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class ConversationalAIOrquestrator : MonoBehaviour, IRecieverMicDataCollaborator
    {
        public event Action<string> OnBase64EncodeGenerated;
        public event Action<byte[]> OnGetRecordData;
        public event Action<float> OnSamplesUpdated;
        public event Action<bool> OnCollaboratorStateChanged;

        [SerializeField]
        private ConnectionStatus _connectionStatus;
        [SerializeField]
        private AIResponseType _evaluateDetectionMode;
        [SerializeField]
        private int _timeToKeepSessionAlive = 10;
        [SerializeField, ReadOnly]
        private int _sessionAliveCounter = 0;

        private MicrophoneRecorder _microphoneService;
        private IWebsocketHandler _websocketService;
        private IAIHandler _aiHandler;
        private string _initialAudioBuffer;
        private WaitForSeconds _wait;
        private EventBusService _eventBusService;

        private bool _lastMicState = false;
        private void Start()
        {
            _wait = new WaitForSeconds(1f);
            _aiHandler = ServiceLocator.Instance.Get<IAIHandler>();
            _websocketService = ServiceLocator.Instance.Get<IWebsocketHandler>();
            ServiceLocator.Instance.Get<MicDataSenderMediator>().SetDefaultCollaborator(this);

            if (ServiceLocator.Instance.Exist<EventBusService>())
            {
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
                _eventBusService.Subscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnConnectionIsStreaming);
                _eventBusService.Subscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnWebSocketConnectionClosedDueExeption);

            }
            else
            {
                Debug.LogError($"{nameof(ConversationalAIOrquestrator)} Error: EventBus Service is not register in the ServiceLocator ");
            }

            //_websocketService.OnWebsocketConnected += OnConnectionIsStreaming;
            //_websocketService.OnWebsocketClosed += OnWebSocketConnectionClosedDueExeption;
            _aiHandler.OnAISingleResponse += OnLastInteractionRecieved;
        }

        private void OnDestroy()
        {

            _websocketService?.CloseConnection();
            //_websocketService.OnWebsocketConnected -= OnConnectionIsStreaming;
            //_websocketService.OnWebsocketClosed -= OnWebSocketConnectionClosedDueExeption;

            if (_eventBusService != null)
            {
                _eventBusService.Unsubscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnConnectionIsStreaming);
                _eventBusService.Unsubscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnWebSocketConnectionClosedDueExeption);

            }

            if (_aiHandler != null)
                _aiHandler.OnAISingleResponse -= OnLastInteractionRecieved;
        }

        public void EnableCollaborator(MicrophoneRecorder microphone)
        {
            OnCollaboratorStateChanged?.Invoke(true);
            if (_microphoneService == null)
                _microphoneService = microphone;

            if (_lastMicState)
                _microphoneService.Init();
        }

        public void MicStarted()
        {

        }

        public void MicEnded()
        {
            _connectionStatus = ConnectionStatus.Preconnecting;
            _websocketService.StartConnection();
        }

        public void DisposeCollaborator()
        {
            OnCollaboratorStateChanged?.Invoke(false);
            _microphoneService.IsStreaming = false;
            _lastMicState = _microphoneService.IsMicEnabled;
            StopAllCoroutines();
            CloseConnection();
        }

        private void OnWebSocketConnectionClosedDueExeption(WebsocketDesconectionType type)
        {
            if (type != WebsocketDesconectionType.ByRequest)
            {
                StopAllCoroutines();
                _microphoneService.IsStreaming = false;
                _connectionStatus = ConnectionStatus.Closed;
            }
        }



        private IEnumerator EvaluateAliveConnection()
        {
            while (_sessionAliveCounter < _timeToKeepSessionAlive)
            {
                yield return _wait;
                _sessionAliveCounter++;
            }
            CloseConnection();
            StopAllCoroutines();
        }

        private void OnLastInteractionRecieved(AIAPIResponse response)
        {
            if (response.Type != _evaluateDetectionMode)
            {
                return;
            }
            _sessionAliveCounter = 0;
        }

        private void CloseConnection()
        {
            if (_websocketService.WebSocketIsConnected)
                _websocketService.ForceCloseConnection();
            _microphoneService.IsStreaming = false;
            _connectionStatus = ConnectionStatus.Closed;
            Debug.Log("Session closed due to inactivity.");
        }

        private void OnConnectionIsStreaming()
        {
            Debug.Log("OnConnectionIsStreaming");

            if (!string.IsNullOrEmpty(_initialAudioBuffer))
            {
                OnBase64EncodeGenerated?.Invoke(_initialAudioBuffer);
                _initialAudioBuffer = string.Empty;
            }
            if (_connectionStatus != ConnectionStatus.Streaming)
            {
                _sessionAliveCounter = 0;
                _microphoneService.IsStreaming = true;
                _connectionStatus = ConnectionStatus.Streaming;
                StartCoroutine(EvaluateAliveConnection());
            }
        }

        public void RecieveBase64Data(string base64)
        {
            if (_connectionStatus != ConnectionStatus.Streaming)
            {
                _initialAudioBuffer = base64;
                return;
            }
            OnBase64EncodeGenerated?.Invoke(base64);
        }

        public void RecieveByteData(byte[] data)
        {
            OnGetRecordData?.Invoke(data);
        }

        public void ReciveSamplesUpdate(float samples)
        {
            OnSamplesUpdated?.Invoke(samples);
        }
    }

    public enum ConnectionStatus
    {
        Closed,
        Preconnecting,
        Streaming
    }
}

