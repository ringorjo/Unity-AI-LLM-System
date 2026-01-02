using System.Collections.Generic;

namespace Services.AI
{
    public class UserMessageSendResponse
    {
        public string id;
        public string @object;
        public int? created_at;
        public string assistant_id;
        public string thread_id;
        public string run_id;
        public string role;
        public List<Content> content;
        public List<object> attachments;
    }
}