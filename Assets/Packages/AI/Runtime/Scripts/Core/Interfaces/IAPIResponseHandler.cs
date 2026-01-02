using System;
namespace Services.AI
{
    public interface IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;
        public bool IsLastResponse { get; }
        public string IdResponse { get; }
        public void PerformResponse(string data);

        public void Dispose();
    }
}
