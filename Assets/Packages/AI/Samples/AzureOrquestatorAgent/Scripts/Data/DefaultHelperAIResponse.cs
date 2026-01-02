using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Services.AI.AIProcedureHelp
{
    public class DefaultHelperAIResponse
    {
        public List<HelperAction> actions;
        public string transcription;
        public string text_to_speech;
    }
    public class AIHelperActionsResponse
    {
        public List<HelperAction> actions;

        public HelperAction GetActionByFunctionName(string functionName)
        {
            return actions.FirstOrDefault(f => f.function == functionName);
        }

        public AIHelperActionsResponse(List<HelperAction> actions)
        {
            this.actions = actions;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

