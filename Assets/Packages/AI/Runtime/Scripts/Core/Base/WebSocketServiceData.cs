using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Services.AI
{
    public abstract class WebSocketServiceData
    {
        public event Action OnConnectionOpen;
        public event Action<WebsocketDesconectionType, string> OnDesconection;
        public event Action<string> OnMessageReceived;

        public bool SendMessagesLog;
        public bool ReciveMessageLog;
        public bool AllowReconect;
        public bool IsConnected;

        protected virtual string _url => string.Empty;
        protected virtual string _serviceURL => string.Empty;
        protected abstract Dictionary<string, string> Headers { get; }

        protected abstract Dictionary<string, object> Params { get; }

        protected ClientWebSocket _websocketClient;
        private WebSocketState _webSocketState = WebSocketState.Closed;
        private CancellationTokenSource _receiveCancellationTokenSource = new CancellationTokenSource();
        private string _messageReceived;
        private CancellationToken _token;
        private byte[] _buffer = new byte[1024 * 16];
        private StringBuilder _messageBuffer = new StringBuilder();
        private WebsocketDesconectionType _negotiateDetectionType = WebsocketDesconectionType.None;

        private const float TIME_OUT_CONECTION = 3;

        public async Task<bool> Connect(bool showErrors = true)
        {
            CancellationTokenSource connectCancelationToken = new CancellationTokenSource(TimeSpan.FromSeconds(TIME_OUT_CONECTION));
            bool hasInternet = await NetworkHelper.HasInternetConnection();
            if (!hasInternet)
                return false;

            _websocketClient?.Dispose();
            _websocketClient = new ClientWebSocket();

            foreach (var header in Headers)
            {
                _websocketClient.Options.SetRequestHeader(header.Key, header.Value);
            }

            try
            {

                ShowLogs($"Contecting to URL: {_url}");
                await _websocketClient.ConnectAsync(new Uri(_url), connectCancelationToken.Token);
                _webSocketState = WebSocketState.Open;
                OnConnectionOpen?.Invoke();
                IsConnected = true;
                ShowLogs($"Connection with URL {_url} has been opened");
                return true;
            }
            catch (WebSocketException ex)
            {
                _webSocketState = WebSocketState.Closed;
                if (showErrors)
                {
                    HandleDesconection($"Unaviable to Connect: {ex.Message}");
                }
                return false;
            }
            catch (OperationCanceledException ex)
            {
                HandleDesconection($"Timeout Error: {ex.Message}");
                Debug.LogWarning($"{nameof(WebSocketServiceData)}: WebsocketMessageReceived task canceled.");
                return false;
            }
        }

        public async void SendMessageToWebsocket(string message)
        {
            try
            {
                if (_websocketClient != null || _websocketClient.State == WebSocketState.Open)
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    ArraySegment<byte> messageBuffer = new ArraySegment<byte>(messageBytes);
                    await _websocketClient?.SendAsync(messageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    if (SendMessagesLog)
                        ShowLogs($"Message send {message}");
                    return;
                }
                HandleDesconection($"Websocket client is not connected. Cannot send message");
                await Task.CompletedTask;

            }
            catch (WebSocketException ex)
            {
                if (AllowReconect)
                {
                    Reconnect();
                }
                HandleDesconection($"Unable to send message due to a failed connection to the server: {ex.Message}");

            }
            catch (Exception ex)
            {
                HandleDesconection($"Exeception Error: {ex.Message}");
            }

        }

        public void ForceDisconnection()
        {
            if (_websocketClient != null)
            {
                _negotiateDetectionType = WebsocketDesconectionType.ByRequest;
                OnDesconection?.Invoke(_negotiateDetectionType, null);
                _websocketClient.Abort();
                _webSocketState = WebSocketState.Closed;
                IsConnected = false;
                ShowLogs($"Websocket connection forcefully closed.");
            }
        }

        public async Task CloseWebSocket()
        {
            if (_websocketClient != null && _websocketClient.State == WebSocketState.Open)
            {
                await _websocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Websocket Connection", CancellationToken.None);
                _negotiateDetectionType = WebsocketDesconectionType.ByRequest;
                OnDesconection?.Invoke(_negotiateDetectionType, null);
                _webSocketState = WebSocketState.Closed;
            }
        }

        public async Task WebsocketMessageReceived()
        {
            _receiveCancellationTokenSource = new CancellationTokenSource();
            _token = _receiveCancellationTokenSource.Token;
            _buffer = new byte[1024 * 16];
            _messageBuffer = new StringBuilder();
            while (_websocketClient.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _websocketClient.ReceiveAsync(new ArraySegment<byte>(_buffer), _token);
                    var chunk = Encoding.UTF8.GetString(_buffer, 0, result.Count);
                    _messageBuffer.Append(chunk);

                    if (result.EndOfMessage)
                    {
                        var jsonResponse = _messageBuffer.ToString();
                        _messageBuffer.Clear();

                        if (jsonResponse.Trim().StartsWith("{"))
                        {
                            OnMessageReceived?.Invoke(jsonResponse);
                            if (ReciveMessageLog)
                                ShowLogs($"Message received {jsonResponse}");
                        }
                    }
                }
                catch (WebSocketException ex)
                {
                    if (AllowReconect)
                    {
                        _webSocketState = WebSocketState.Connecting;
                        Reconnect();
                    }
                    HandleDesconection($"Unable to receive message due to a failed connection to the server: {ex.Message}");
                }

                catch (OperationCanceledException)
                {
                    Debug.LogWarning($"{nameof(WebSocketServiceData)}: WebsocketMessageReceived task canceled.");
                    return;
                }
            }
        }

        private async void HandleDesconection(string error)
        {
            IsConnected = false;
            bool hasInternet = await NetworkHelper.HasInternetConnection();
            if (hasInternet)
            {
                if (_negotiateDetectionType != WebsocketDesconectionType.ByRequest)
                    ShowLogError(WebsocketDesconectionType.Error, error);
                return;
            }
            _receiveCancellationTokenSource?.Cancel();
            _negotiateDetectionType = WebsocketDesconectionType.NetworkLoss;
            ShowLogError(_negotiateDetectionType, " Internet conection Lost");


        }

        private async void Reconnect()
        {
            _webSocketState = WebSocketState.Connecting;
            while (_webSocketState != WebSocketState.Open)
            {
                ShowLogError(WebsocketDesconectionType.Reconection, $"Reconecting");
                bool _isConnected = await Connect(false);
                if (_isConnected)
                    await WebsocketMessageReceived();
                await Task.Delay(3000);
            }
        }

        private void ShowLogError(WebsocketDesconectionType type, string message)
        {
            Debug.LogError($"{nameof(WebSocketServiceData)}: {message}");
            OnDesconection?.Invoke(type, message);
        }


        private void ShowLogs(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"{nameof(WebSocketServiceData)}: {message}");
#endif
        }
    }
}