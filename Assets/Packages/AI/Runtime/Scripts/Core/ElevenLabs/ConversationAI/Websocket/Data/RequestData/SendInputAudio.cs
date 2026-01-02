

using Newtonsoft.Json;

namespace Services.AI
{
    public class SendInputAudio
    {
        [JsonProperty("user_audio_chunk")]
        public string ChunkAudio;

        public void SetAudioChunk(string base64)
        {
            ChunkAudio = base64;
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
