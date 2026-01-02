using Sirenix.OdinInspector;
using UnityEngine;

using Xennial.Services;

namespace Services.AI
{
    public class WebsocketConnectionMonitorUI : SerializedMonoBehaviour
    {
        [SerializeField]
        private IWebsocketUIShowStrategy _websocketUIStrategy;
        private IWebsocketHandler _websocketHandler;
        private EventBusService _eventBusService;

        private void Start()
        {
            if (ServiceLocator.Instance.Exist<EventBusService>())
            {
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
                _eventBusService.Subscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnConnected);
                _eventBusService.Subscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnDisconnection);
                //_websocketHandler = ServiceLocator.Instance.Get<IWebsocketHandler>();
                //_websocketHandler.OnWebsocketClosed += OnDisconnection;
                //_websocketHandler.OnWebsocketConnected += OnConnected;
            }
            else
            {
                Debug.LogError($"{nameof(WebsocketConnectionMonitorUI)} Error: EventBus Service is not register in the ServiceLocator ");
            }
        }

        private void OnConnected()
        {
            _websocketUIStrategy?.OnConnected();
        }

        private void OnDisconnection(WebsocketDesconectionType type)
        {
            _websocketUIStrategy?.OnDisconnection(type);
        }


        private void OnDestroy()
        {
            if (_eventBusService != null)
            {
                _eventBusService.Unsubscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnConnected);
                _eventBusService.Unsubscribe<WebsocketDesconectionType>(nameof(WebsocketEventEnums.OnWebsocketClosed), OnDisconnection);
                //_websocketHandler.OnWebsocketClosed -= OnDisconnection;
                //_websocketHandler.OnWebsocketConnected -= OnConnected;

            }
        }

    }
}

