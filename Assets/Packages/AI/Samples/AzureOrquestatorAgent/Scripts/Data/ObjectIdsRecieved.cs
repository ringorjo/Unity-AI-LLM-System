using Newtonsoft.Json;
using System.Collections.Generic;

namespace Services.AI.AIProcedureHelp
{
    public class ObjectIdsRecieved : AIRequestData
    {
        public List<string> object_id;

        public override string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

