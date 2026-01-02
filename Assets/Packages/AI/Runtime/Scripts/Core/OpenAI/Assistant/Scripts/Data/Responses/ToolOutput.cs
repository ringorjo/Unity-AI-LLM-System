using System;
namespace Services.AI
{
    [Serializable]
    public class ToolOutput
    {
        public string tool_call_id;
        public string output;

        public ToolOutput(string tool_call_id, string output)
        {
            this.tool_call_id = tool_call_id;
            this.output = output;
        }
    }

}

