using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

namespace Services.AI
{
    public enum role { user, system, assistant };
    [Serializable]
    public class Message
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public role role;
        [TextArea(4, 10)]
        public string content;

        public Message()
        {
        }

        public Message(string content)
        {
            this.role = role.user;
            this.content = content;
        }

        public Message(role role, string content)
        {
            this.role = role;
            this.content = content;
        }

        public void UpdateContent(string content)
        {
            this.content = content;
        }
    }
}

