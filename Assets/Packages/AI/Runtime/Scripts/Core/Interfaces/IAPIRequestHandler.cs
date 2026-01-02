using System;

namespace Services.AI
{
    public interface IAPIRequestHandler
    {
        public event Action<AIAPIResponse> OnRequestCompleted;
        public IAPIRequestHandler NextHandler { get; }
        public void SetNextHandler(IAPIRequestHandler nextHandler);

        public void HandleRequest(string input = default);
    }
}
