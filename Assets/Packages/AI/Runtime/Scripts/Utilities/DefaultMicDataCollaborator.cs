using System;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class DefaultMicDataCollaborator : MonoBehaviour, IRecieverMicDataCollaborator
    {
        public event Action<string> OnBase64EncodeGenerated;
        public event Action<byte[]> OnGetRecordData;
        public event Action<bool> OnCollaboratorStateChanged;

        private void Start()
        {
            ServiceLocator.Instance.Get<MicDataSenderMediator>().SetDefaultCollaborator(this);

        }
        public void DisposeCollaborator()
        {
            OnCollaboratorStateChanged?.Invoke(false);
        }

        public void EnableCollaborator(MicrophoneRecorder microphone)
        {
            OnCollaboratorStateChanged?.Invoke(true);
        }

        public void MicEnded()
        {
        }

        public void MicStarted()
        {
           
        }

        public void RecieveBase64Data(string base64)
        {
            OnBase64EncodeGenerated?.Invoke(base64);
        }

        public void RecieveByteData(byte[] data)
        {
            OnGetRecordData?.Invoke(data);
        }
    }
}
