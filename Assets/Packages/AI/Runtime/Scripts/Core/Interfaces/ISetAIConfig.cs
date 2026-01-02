namespace Services.AI
{
    public interface ISetAIConfig
    {
        public void OverrideIAConfig(AIConfigData aiconfig);
        public void Init();
    }

}
