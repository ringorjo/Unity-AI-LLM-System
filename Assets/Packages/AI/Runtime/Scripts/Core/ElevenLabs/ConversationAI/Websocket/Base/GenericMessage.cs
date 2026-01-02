
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class GenericMessage
{
    [JsonProperty("type")]
    public string Type;
    public string Data;
}

public class GenericMessageConverter : JsonConverter<GenericMessage>
{
    public override GenericMessage ReadJson(JsonReader reader, Type objectType, GenericMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        var result = new GenericMessage
        {
            Type = jObject["type"]?.ToString()
        };

        var dataProperty = jObject.Properties().FirstOrDefault(p => p.Name != "type");
        if (dataProperty != null)
        {
            result.Data = dataProperty.Value.ToString();
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, GenericMessage value, JsonSerializer serializer)
    {
       
    }
}
