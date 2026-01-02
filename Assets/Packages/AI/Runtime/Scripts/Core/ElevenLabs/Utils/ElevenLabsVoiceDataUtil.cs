using UnityEngine;

public static class ElevenLabsVoiceDataUtil
{
    private static ElevenLabVoices _elevenLabsVoiceData;
    private const string ELEVEN_LABS_VOICE_DATA_PATH = "ElevenLabVoices";

    public static ElevenLabVoices GetElevenLabsVoiceData()
    {
        if (_elevenLabsVoiceData == null)
        {
            _elevenLabsVoiceData = Resources.Load<ElevenLabVoices>(ELEVEN_LABS_VOICE_DATA_PATH);
            if (_elevenLabsVoiceData == null)
            {
                Debug.LogError($"Error: The scriptable Object {ELEVEN_LABS_VOICE_DATA_PATH} does not exist in Resources.");
                return null;
            }
        }
        return _elevenLabsVoiceData;
    }
}
