using System;
namespace Services.AI
{
    [Serializable]
    public class AssistantData
    {
        public string Token;
        public string AssistantId;
        public string ThreadId;
        public string AssistantVersion;

        public AssistantData(string token, string assistantId, string threadId, string assistantVersion)
        {
            Token = token;
            AssistantId = assistantId;
            ThreadId = threadId;
            AssistantVersion = assistantVersion;
        }
    }

}

