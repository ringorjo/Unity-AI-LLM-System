using Sirenix.OdinInspector;
using UnityEngine;
using Services.AI.ConversationAI.Websocket.Data.RequestData;
using Xennial.Services;

namespace Services.AI
{
    public class ConversationInizalitazion : SerializedMonoBehaviour
    {
        private ConversationData _conversationData;
        private IWebsocketHandler _websocket;
        private EventBusService _eventBusService;


        private void Awake()
        {
            _conversationData = new ConversationData();
        }

        private void Start()
        {
            _websocket = ServiceLocator.Instance.Get<IWebsocketHandler>();

            if (ServiceLocator.Instance.Exist<EventBusService>())
            {
                _eventBusService = ServiceLocator.Instance.Get<EventBusService>();
                _eventBusService.Subscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnCreateConversation);
            }
            else
            {
                Debug.LogError($"{nameof(ConversationInizalitazion)} Error: EventBus Service is not register in the ServiceLocator ");
            }
        }

        private void OnDestroy()
        {
            if (_eventBusService != null)
            {
                _eventBusService.Unsubscribe(nameof(WebsocketEventEnums.OnWebsocketConnected), OnCreateConversation);
            }
        }

        [Button]
        private void OnCreateConversation()
        {
            _websocket.SendMessageToWebsocket(_conversationData.GetJson());
        }
    }
}