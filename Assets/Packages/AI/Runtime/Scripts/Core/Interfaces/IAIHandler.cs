using System;
using Xennial.Services;
namespace Services.AI
{
    public interface IAIHandler : IService
    {
        /// <summary>
        /// Use this event to Handle single AI Response for example AI Text Response,Transcription Response
        /// </summary>
        public event Action<AIAPIResponse> OnAISingleResponse;
        /// <summary>
        /// Use this event to streaming AI response for example AI Voice generated
        /// </summary>
        public event Action<AIAPIResponse> OnAIStreamingResponse;
        public void Input(string input);
    }

    public interface IAIHandlerExtention
    {
        public AIConfigData GetConfigData { get; }
        public void InjectConfig(AIConfigData config);
    }
}