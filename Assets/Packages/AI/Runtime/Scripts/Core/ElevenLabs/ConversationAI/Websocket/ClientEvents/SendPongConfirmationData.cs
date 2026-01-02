using System.Collections;
using UnityEngine;
using Services.AI;
using Services.AI.ElevenLabs.ConversationAI.Websocket.Data.RequestData;
using Xennial.Services;

public class SendPongConfirmationData : MonoBehaviour
{
    [SerializeField]
    private float _pongConfirmationTime = 5f;

    private IWebsocketHandler _websocket;
    private EventBusService _eventBusService;
    private SendPongMessage _sendPongMessage;

    private void Start()
    {
        if (ServiceLocator.Instance.Exist<EventBusService>())
        {
            _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
            _eventBusService.Subscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), GetOnWebsocketConnected);
            _eventBusService.Subscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnWebsocketClosed);
            //_websocketHandler = ServiceLocator.Instance.Get<IWebsocketHandler>();
            //_websocketHandler.OnWebsocketClosed += OnDisconnection;
            //_websocketHandler.OnWebsocketConnected += OnConnected;
        }
        else
        {
            Debug.LogError($"{nameof(SendPongConfirmationData)} Error: EventBus Service is not register in the ServiceLocator ");
        }


        //_websocket = ServiceLocator.Instance.Get<IWebsocketHandler>();
        //_websocket.OnWebsocketConnected += GetOnWebsocketConnected;
        //_websocket.OnWebsocketClosed += OnWebsocketClosed;
        _sendPongMessage = new SendPongMessage();
    }
    private void OnDestroy()
    {
        if (_eventBusService != null)
        {
            _eventBusService.Unsubscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), GetOnWebsocketConnected);
            _eventBusService.Unsubscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnWebsocketClosed);

        }
    }


    private void GetOnWebsocketConnected()
    {
        StartCoroutine(SendPongValidation());
    }
    private void OnWebsocketClosed(WebsocketDesconectionType type)
    {
        StopAllCoroutines();
    }

    private IEnumerator SendPongValidation()
    {
        yield return null;
        while (_websocket.WebSocketIsConnected)
        {
            yield return new WaitForSeconds(_pongConfirmationTime);
            _sendPongMessage.SetEventId(Random.Range(1, 10000));
            _websocket.SendMessageToWebsocket(_sendPongMessage.ToJson());
        }
        Debug.Log("Websocket connection is closed, stopping pong confirmation.");
    }
}
