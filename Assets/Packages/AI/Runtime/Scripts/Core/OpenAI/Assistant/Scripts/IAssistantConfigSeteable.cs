using System;

namespace Services.AI
{
    public interface IAssistantConfigSeteable
    {
        public void SetAssistantConfig(Func<AssistantRepositoryDataUtil> config);
    }
}
