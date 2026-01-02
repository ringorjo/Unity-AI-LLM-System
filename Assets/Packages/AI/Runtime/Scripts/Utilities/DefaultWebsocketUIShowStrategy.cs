using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Services.AI
{
    public class DefaultWebsocketUIShowStrategy : IWebsocketUIShowStrategy
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Sprite _connectedState;
        [SerializeField]
        private Sprite _disconnectedState;
        [SerializeField]
        private Sprite _idleState;



        public void OnConnected()
        {
            _icon.sprite = _connectedState;
        }

        public void OnDisconnection(WebsocketDesconectionType type)
        {
            if (type == WebsocketDesconectionType.ByRequest)
            {
                _icon.sprite = _idleState;
                return;
            }
            _icon.sprite = _disconnectedState;

        }
    }
}

