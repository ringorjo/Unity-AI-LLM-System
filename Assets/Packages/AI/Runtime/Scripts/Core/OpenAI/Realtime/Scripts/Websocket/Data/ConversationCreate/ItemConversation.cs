using System.Collections.Generic;
namespace Services.AI
{
    public class ItemConversation
    {
        public string id = "Message";
        public string type = "message";
        public string role = "user";
        // [ReadOnly]
        public List<IContenItem> content = new List<IContenItem>();
    }

    public interface IContenItem
    {
        string type { get; set; }
    }


    public class TextContentItem : IContenItem
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    public class AudioContentItem : IContenItem
    {
        public string type { get; set; }
        public string audio { get; set; }
    }
}
