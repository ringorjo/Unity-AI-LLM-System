using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Services.AI.AIProcedureHelp
{
    [Serializable]
    public class DefaultHelperAIRequest : AIRequestData
    {
        public List<DefaultAIRequestInteractionData> InteractionsToPerform;

        public void AddInteraction(DefaultAIRequestInteractionData interaction)
        {
            InteractionsToPerform.Add(interaction);
        }
        public void CleanCollection()
        {
            InteractionsToPerform.Clear();
        }
        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    [Serializable]
    public class DefaultAIRequestInteractionData
    {
        public string InteractionName;
        public string Hint;
        public string ObjectId;

        public DefaultAIRequestInteractionData(string interactionName, string hint, string objectId)
        {
            InteractionName = interactionName;
            Hint = hint;
            ObjectId = objectId;
        }
    }
}

