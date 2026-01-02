using System;
using Services.AI;

public interface IRecieverMicDataCollaborator
{
    public event Action<string> OnBase64EncodeGenerated;
    public event Action<byte[]> OnGetRecordData;
    public event Action<bool> OnCollaboratorStateChanged;

    void RecieveBase64Data(string base64);
    void RecieveByteData(byte[] data);
    void EnableCollaborator(MicrophoneRecorder microphone);

    void DisposeCollaborator();

    void MicStarted();

    void MicEnded();


}
