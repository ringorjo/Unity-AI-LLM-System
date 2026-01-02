using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Services.AI.ElevenLabs.ConversationAI.Websocket.Data.RequestData;
using Xennial.Services;

namespace Services.AI
{
    public class SendUserInputData : SerializedMonoBehaviour, IAPIRequestHandler
    {
        public IAPIRequestHandler NextHandler => _nextHandler;

        public event Action<AIAPIResponse> OnRequestCompleted;

        [SerializeField]
        private IRecieverMicDataCollaborator _recieverMicDataCollaborator;

        [SerializeField]
        private List<IAPIResponseHandler> _responsesHandler;

        private IAPIRequestHandler _nextHandler;
        private SendInputText _inputText;
        private SendInputAudio _inputAudio;
        private IWebsocketHandler _websocket;
        private Dictionary<string, IAPIResponseHandler> _responses = new Dictionary<string, IAPIResponseHandler>();
        private AIAPIResponse _userTrancriptionResponse;
        private EventBusService _eventBusService;

        private void Start()
        {
            if (ServiceLocator.Instance.Exist<EventBusService>())
            {
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
                _eventBusService.Subscribe<string>(nameof(WebsocketEventEnums.OnWebsocketResponse), OnWBResponse);

            }
            else
            {
                Debug.LogError($"{nameof(SendUserInputData)} Error: EventBus Service is not register in the ServiceLocator ");
            }


            _inputText = new SendInputText();
            _inputAudio = new SendInputAudio();

            _userTrancriptionResponse = new AIAPIResponse(
                AIResponseType.UserTranscription,
                string.Empty,
                true
            );
            _websocket = ServiceLocator.Instance.Get<IWebsocketHandler>();

            // _websocket.OnWebSocketResponse += OnWBResponse;

            if (_recieverMicDataCollaborator != null)
                _recieverMicDataCollaborator.OnBase64EncodeGenerated += PerformBase64Input;

            foreach (var handler in _responsesHandler)
            {
                _responses.Add(handler.IdResponse, handler);
                handler.OnAPIResponse += OnHandlerRequestResponse;
            }
        }
        private void OnDestroy()
        {
            if (_recieverMicDataCollaborator != null)
                _recieverMicDataCollaborator.OnBase64EncodeGenerated -= PerformBase64Input;

            if (_eventBusService != null)
                _eventBusService.Unsubscribe<string>(nameof(WebsocketEventEnums.OnWebsocketResponse), OnWBResponse);

        }

        private void OnWBResponse(string response)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new GenericMessageConverter() }
            };
            GenericMessage message = JsonConvert.DeserializeObject<GenericMessage>(response, settings);

            if (message != null && _responses.TryGetValue(message.Type, out IAPIResponseHandler result))
            {
                result.PerformResponse(message.Data.ToString());
            }
        }

        private void OnHandlerRequestResponse(AIAPIResponse response)
        {
            OnRequestCompleted?.Invoke(response);
        }

        private void PerformBase64Input(string base64)
        {
            if (!_websocket.WebSocketIsConnected)// For Voice input make sure that only can send data when wss is open
                return;

            _inputAudio.SetAudioChunk(base64);
            _websocket?.SendMessageToWebsocket(_inputAudio.ToJson());
        }

        [Button]
        public void HandleRequest(string input = null)
        {
            _inputText.SetText(input);
            _userTrancriptionResponse.UpdateText(_inputText.Text);
            OnHandlerRequestResponse(_userTrancriptionResponse);
            _websocket?.SendMessageToWebsocket(_inputText.ToJson());
        }

        public void SetNextHandler(IAPIRequestHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }
    }
}
