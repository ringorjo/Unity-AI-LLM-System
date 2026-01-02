using Newtonsoft.Json;
using System;
using UnityEngine;
namespace Services.AI
{
    public class RealtimeResponseAudioDelta : IAPIResponseHandler
    {
        public event Action<AIAPIResponse> OnAPIResponse;

        [SerializeField]
        private bool _streamResponse;
        [SerializeField]
        private bool _lastResponse;
        private string _base64Output;
        private AIAPIResponse _response;
        public string IdResponse => "response.audio.delta";
        public bool IsLastResponse => _lastResponse;

        public void Dispose()
        {
            if (!_streamResponse)
            {
                _response.UpdateText(_base64Output);
                OnAPIResponse?.Invoke(_response);

            }
            Debug.Log("Audio Delta Response Completed");

            _base64Output = string.Empty;
        }

        public void PerformResponse(string data)
        {
            if (_response == null)
            {
                _response = new AIAPIResponse(AIResponseType.Audio, string.Empty);
            }
            ResponseAudioData chunckdata = JsonConvert.DeserializeObject<ResponseAudioData>(data);
            if (chunckdata != null)
            {
                if (_streamResponse)
                {
                    _response.UpdateText(chunckdata.delta);
                    OnAPIResponse?.Invoke(_response);

                }
                _base64Output += chunckdata.delta;
            }
        }
    }

    public class ResponseAudioData
    {
        public string event_id;
        public string type;
        public string response_id;
        public int output_index;
        public int content_index;
        public string delta;
    }
}
