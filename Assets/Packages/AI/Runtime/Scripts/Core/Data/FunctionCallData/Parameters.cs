using Sirenix.Serialization;
using System.Collections.Generic;

namespace Services.AI
{
    public class Parameters
    {
        public string type;
        [OdinSerialize]
        public Dictionary<string, SchemaTemplate> properties;
        public List<string> required;
    }
}
