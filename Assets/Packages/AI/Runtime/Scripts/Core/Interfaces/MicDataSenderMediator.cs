using System;
using UnityEngine;
using Xennial.Services;

namespace Services.AI
{
    public class MicDataSenderMediator : IService
    {
        public event Action<float> OnSamplesUpdated;

        private MicrophoneRecorder _microphoneRecorder;
        private IRecieverMicDataCollaborator _collaborator;
        private IRecieverMicDataCollaborator _defaultCollaborator;

        public bool IsMicEnable
        {
            get => _microphoneRecorder.IsMicEnabled;
        }
        public MicDataSenderMediator(MicrophoneRecorder microphoneService)
        {
            Register();
            _microphoneRecorder = microphoneService;
            _microphoneRecorder.OnMicMaxSampleUpdated += OnMaxSamplesUpdated;
            _microphoneRecorder.OnBase64EncodeGenerated += OnMicBase64Data;
            _microphoneRecorder.OnGetRecordData += OnMicSendBytesData;
            _microphoneRecorder.OnStartRecording += OnMicStarted;
            _microphoneRecorder.OnStopRecording += OnMicStoped;
        }

        private void OnMaxSamplesUpdated(float maxSamples) => OnSamplesUpdated?.Invoke(maxSamples);
        private void OnMicSendBytesData(byte[] data) => _collaborator?.RecieveByteData(data);
        private void OnMicStoped() => _collaborator?.MicEnded();
        private void OnMicStarted() => _collaborator?.MicStarted();
        private void OnMicBase64Data(string base64) => _collaborator?.RecieveBase64Data(base64);
        public void StartRecording() => _microphoneRecorder.StartRecord();
        public void StopRecordinfg() => _microphoneRecorder.StopRecord();
        public void ReturnDefaultCollaborator() => SetReciverCollaborator(_defaultCollaborator);
        public void Register() => ServiceLocator.Instance.Register(this);



        public void SetDefaultCollaborator(IRecieverMicDataCollaborator collaborator)
        {
            _defaultCollaborator = collaborator;
            SetReciverCollaborator(collaborator);
        }

        public void ChangeMicUsage(bool enable)
        {
            if (enable)
                _microphoneRecorder.Init();
            else
            {
                _microphoneRecorder.Dispose();
                OnSamplesUpdated?.Invoke(0);
            }
        }
        public void SetReciverCollaborator(IRecieverMicDataCollaborator collaborator)
        {
            if (collaborator == _collaborator)
                return;

            _collaborator?.DisposeCollaborator();
            _collaborator = collaborator;
            Debug.Log($"Setting new Collaborator: {_collaborator.ToString()} ");
            _collaborator.EnableCollaborator(_microphoneRecorder);
        }
        public void Unregister()
        {
            ServiceLocator.Instance.Unregister(this);
            if (_microphoneRecorder == null)
                return;

            _microphoneRecorder.OnMicMaxSampleUpdated -= OnMaxSamplesUpdated;
            _microphoneRecorder.OnBase64EncodeGenerated -= OnMicBase64Data;
            _microphoneRecorder.OnGetRecordData -= OnMicSendBytesData;
            _microphoneRecorder.OnStartRecording -= OnMicStarted;
            _microphoneRecorder.OnStopRecording -= OnMicStoped;
        }
    }
}