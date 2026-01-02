using UnityEngine;
namespace Services.AI
{
    public class AIProcessorOutputResponseAudio : AIOutputResponse
    {
        [SerializeField]
        private AudioSource _audioSource;
        private AudioClip _audioClip;

        private void Reset()
        {
            Type = AIResponseType.Audio;
        }
        public override void ProcessAIResponse(AIAPIResponse response)
        {
            _audioClip = Base64EncodeTool.Base64ToAudioClip(response.Text);
            _audioSource.clip = _audioClip;
            _audioSource.Play();
        }
    }
}