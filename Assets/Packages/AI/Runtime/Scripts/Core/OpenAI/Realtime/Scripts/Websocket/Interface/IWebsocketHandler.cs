using System;
using System.Threading.Tasks;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public interface IWebsocketHandler : IService
    {
        //public event Action OnWebsocketConnected;
        //public event Action<WebsocketDesconectionType> OnWebsocketClosed;
        //public event Action<string> OnWebSocketResponse;
        //public event Action<Color, string> OnWebSocketStatus;
        public bool WebSocketIsConnected { get; }


        public Task StartConnection();
        public Task CloseConnection();
        public void SendMessageToWebsocket(string message);

        public void ForceCloseConnection();
    }

}
