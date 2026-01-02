namespace Services.AI
{
    public interface IWebsocketUIShowStrategy
    {
        void OnConnected();
        void OnDisconnection(WebsocketDesconectionType type);
    }
}

