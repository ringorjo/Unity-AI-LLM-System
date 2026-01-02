
using System;
namespace Services.AI
{
    public interface ISpeechToTextHandler
    {
        public event Action<string> OnPartialTranscription;
        public event Action<string> OnFullTranscription;
        public void StartDictation();
        public void StopDictation();

        public void Init();
        public void Dispose();

    }
}
