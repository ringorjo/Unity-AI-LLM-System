using System;
using System.Collections;
using System.Collections.Generic;
namespace Services.AI
{
    public interface ITextToSpeechHandler
    {
        void ProcessSpeech(string textToSpeak);

        void TruncateSpeech();
    }

    public interface ITextToSpeechExtension
    {
        void EnqueueTextChunks(string cunks, Action OnSpeechEnd = null);

        void EnqueueTextChunks(List<string> chunks, Action OnSpeechEnd = null);

        IEnumerator PerformSpeechAsync(string textToSpeak, Action OnSpeechEnd = null);

        IEnumerator PerformSpeechAsync(List<string> chunks, Action OnSpeechEnd = null);
    }
}
