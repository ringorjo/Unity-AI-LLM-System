using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeResponseFunctionOutput : IAPIResponseHandler
    {
        [SerializeField]
        private bool _isLastResponse;
        public bool IsLastResponse => _isLastResponse;

        private FunctionOutputResponse _response;

        public string IdResponse => "response.function_call_arguments.done";

        public event Action<AIAPIResponse> OnAPIResponse;

        public void Dispose()
        {
            _response = null;
        }

        public void PerformResponse(string data)
        {
            _response = JsonConvert.DeserializeObject<FunctionOutputResponse>(data);
            if (_response != null)
            {
#if UNITY_EDITOR
                Debug.Log($"{nameof(IAPIResponseHandler)}: {IdResponse} Result: {_response.arguments} ");
#endif
                OnAPIResponse?.Invoke(new AIAPIResponse(AIResponseType.Json_extraData, _response.arguments, _isLastResponse));
            }
        }


        internal class FunctionOutputResponse
        {
            public string event_id;
            public string type;
            public string response_id;
            public string item_id;
            public int output_index;
            public string call_id;
            public string arguments;
        }

    }
}
