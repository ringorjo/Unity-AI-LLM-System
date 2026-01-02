namespace Services.AI
{
    public class AIAPIResponse
    {
        public AIResponseType Type;
        public string Text;
        public bool IsSingleResponse;

        public AIAPIResponse(AIResponseType type, string text)
        {
            Type = type;
            Text = text;
        }
        public void UpdateText(string text)
        {
            Text = text;
        }

        public void UpdateData(AIResponseType type, string text)
        {
            Type = type;
            Text = text;
        }
        public AIAPIResponse(AIResponseType type, string text, bool finalResponse) : this(type, text)
        {
            IsSingleResponse = finalResponse;
        }
    }

}
