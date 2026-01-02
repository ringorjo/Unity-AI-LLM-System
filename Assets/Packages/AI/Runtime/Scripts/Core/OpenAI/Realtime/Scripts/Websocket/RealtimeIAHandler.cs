namespace Services.AI
{
    public class RealtimeIAHandler : AIHandlerBase
    {
        public override void Input(string input)
        {
            foreach (var request in _responsabilityAPIRequests)
            {
                if (request is RealtimeConversationItemCreate)
                {
                    request.HandleRequest(input);
                    break;
                }
            }
        }
    }
}
