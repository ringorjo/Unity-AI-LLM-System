using Sirenix.OdinInspector;
using UnityEngine;

namespace Services.AI
{
    public class RealtimeInputBufferApend : AIWSRequestHandlerBase
    {
        [SerializeField, Required]
        private IRecieverMicDataCollaborator _recieverDataCollaborator;

        private InputBufferData _audioBufferData;
        private const string APPEND_BUFFER_TYPE = "input_audio_buffer.append";

        private void Awake()
        {
            _audioBufferData = new InputBufferData("AppendBuffer_001", APPEND_BUFFER_TYPE);
        }

        protected override void Start()
        {
            base.Start();
            _recieverDataCollaborator.OnBase64EncodeGenerated += HandleRequest;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _recieverDataCollaborator.OnBase64EncodeGenerated -= HandleRequest;
        }

        public override void HandleRequest(string input = null)
        {
            if (!_realtimeWS.WebSocketIsConnected)
                return;

            _audioBufferData.audio = input;
            _realtimeWS.SendMessageToWebsocket(_audioBufferData.GetJson());
        }
    }
}