using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services.AI
{
    public class RealtimeConversationItemCreate : AIWSRequestHandlerBase
    {
        [SerializeField, PropertyOrder(-1)]
        private ConversationCreateData _conversationCreate;
        [SerializeField, Required]
        private IRecieverMicDataCollaborator _recieverDataCollaborator;

        public override event Action<AIAPIResponse> OnRequestCompleted;

        #region LifeCycle
        protected override void Start()
        {
            base.Start();
            _conversationCreate.item.content = new List<IContenItem>();
            _recieverDataCollaborator.OnBase64EncodeGenerated += PerfromBase64Input;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _recieverDataCollaborator.OnBase64EncodeGenerated -= PerfromBase64Input;
        }
        #endregion

        #region Handle Input
        [Button]
        public override void HandleRequest(string input = null)
        {
            PerformTextInput(input);

        }
        #endregion

        private void PerfromBase64Input(string input)
        {
            if (!_realtimeWS.WebSocketIsConnected)
                return;

            _conversationCreate.item.content = new List<IContenItem>
            {
                new AudioContentItem
                {
                    type = "input_audio",
                    audio = input
                }
            };
            _realtimeWS.SendMessageToWebsocket(_conversationCreate.GetJson());
            _nextHandler?.HandleRequest();
        }

        private void PerformTextInput(string input)
        {
            _conversationCreate.item.content = new List<IContenItem> {
            new TextContentItem
                {
                    type = "input_text",
                    text = input
                }
            };
            _realtimeWS.SendMessageToWebsocket(_conversationCreate.GetJson());
            _nextHandler?.HandleRequest();
            OnRequestCompleted?.Invoke(new AIAPIResponse(AIResponseType.UserTranscription, input, true));
        }
    }
}