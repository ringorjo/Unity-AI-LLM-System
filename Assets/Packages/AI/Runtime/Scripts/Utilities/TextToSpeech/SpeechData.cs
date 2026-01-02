using SnowKore.Services;
using System.Collections.Generic;

namespace Services.AI
{
    public class SpeechData : ServiceData
    {
        private string _input;
        private string _token;
        private string _voice;
        private const string SPEECH_MODEL = "tts-1";
        private const string AUDIO_FORMAT = "pcm";

        public SpeechData(string input, string voice, string token)
        {
            _input = input;
            _voice = voice;
            _token = token;
        }
        protected override string BaseURL => "https://api.openai.com/v1/audio";

        protected override Dictionary<string, object> Body
        {
            get
            {
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("model", SPEECH_MODEL);
                body.Add("input", _input);
                body.Add("voice", _voice);
                body.Add("response_format", AUDIO_FORMAT);
                return body;
            }
        }
        protected override string ServiceURL
        {
            get => "/speech";
        }
        protected override Dictionary<string, object> Params
        {
            get
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                return param;
            }
        }
        protected override Dictionary<string, string> Headers
        {
            get
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", $"Bearer {_token}");
                headers.Add("Content-Type", "application/json");
                return headers;
            }
        }
        protected override ServiceType ServiceType => ServiceType.POST;
    }

}
