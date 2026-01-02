using System;
using System.Collections.Generic;

namespace Services.AI
{
    #region Run Create
    [Serializable]
    public class RunCreatedReponse
    {
        public string id;
        public string @object;
        public int? created_at;
        public string assistant_id;
        public string thread_id;
        public string status;
        public int? started_at;
        public object expires_at;
        public object cancelled_at;
        public object failed_at;
        public int? completed_at;
        public RequiredAction required_action;
        public object last_error;
        public string model;
        public object instructions;
        public object incomplete_details;
        public List<Tools> tools;
        public object usage;
        public double? temperature;
        public double? top_p;
        public int? max_prompt_tokens;
        public int? max_completion_tokens;
        public TruncationStrategy truncation_strategy;
        public string response_format;
        public string tool_choice;
        public bool parallel_tool_calls;
    }

    #endregion
}