using System.Linq;

namespace Services.AI
{
    public class ConversationalAIHandler : AIHandlerBase
    {
        public override void Input(string input)
        {
            _responsabilityAPIRequests?.First().HandleRequest(input);
        }
    }
}

