using Sirenix.OdinInspector;
using System.Collections.Concurrent;
using UnityEngine;
using Services.AI;
using Xennial.API;

[RequireComponent(typeof(AudioSource))]
public class ElevenLabsTTS : Request<ElevenLabsTTSServiceData>, ITextToSpeechHandler
{
    [SerializeField]
    private AIConfigData _ttsConfig;
    [SerializeField]
    private ElevenLabVoices _elevenLabVoices;
    [SerializeField, ElevenLabsVoiceSelector]
    private string _voice;
    [SerializeField]
    private int _inputSampleRate = 16000;
    private int _outputSampleRate;
    private string _inputText;
    private string _voiceId;
    private ConcurrentQueue<float> _audioBuffer = new ConcurrentQueue<float>();

    protected override object[] RequestParams
    {
        get
        {
            return new object[] { _ttsConfig, _voiceId, _inputText };
        }
    }


    private void Start()
    {
        _outputSampleRate=AudioSettings.outputSampleRate;
    }


    private void OnAudioFilterRead(float[] data, int channels)
    {

        if (_audioBuffer.Count <= 0)
            return;

        for (var i = 0; i < data.Length; i += channels)
        {
            if (_audioBuffer.TryDequeue(out var sample))
            {
                for (var j = 0; j < channels; j++)
                {
                    data[i + j] = sample;
                }
            }
        }
    }
    [Button]
    public void ProcessSpeech(string textToSpeak)
    {
        _inputText = textToSpeak;
        if (string.IsNullOrEmpty(_voiceId))
            _voiceId = _elevenLabVoices.GetVoiceId(_voice);

        SendRequestForFile();
    }

    public void TruncateSpeech()
    {
        _audioBuffer.Clear();
    }

    protected override void OnResponseReceived(string response) { }

    protected override void OnResponseReceived(byte[] dataRecieved)
    {
        float[] samples = WavUtility.Resample(WavUtility.PCMDecode(dataRecieved), null, _inputSampleRate, _outputSampleRate);
        foreach (float pcmSample in samples)
            _audioBuffer.Enqueue(pcmSample);
    }

}
