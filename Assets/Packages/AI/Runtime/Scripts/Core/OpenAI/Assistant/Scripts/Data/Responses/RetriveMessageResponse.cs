using System.Collections.Generic;

namespace Services.AI
{
    #region Thread And Message Create

    public class RetriveMessageResponse
    {
        public string @object;
        public List<UserMessageSendResponse> data;
        public string first_id;
        public string last_id;
        public bool has_more;

        public UserMessageSendResponse GetResponseByRole(string role)
        {
            return data.Find(x => x.role == role);
        }
    }

    #endregion
}