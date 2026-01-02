using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services.AI
{
    public class AssistantAIHandler : AIHandlerBase
    {
        [SerializeField]
        [PropertyOrder(-1)]
        private string _assistantId;
        [SerializeField]
        private ISpeechToTextHandler _speechToText;
        private AssistantRepositoryDataUtil _assistantRepositoryData;
        private List<IAssistantConfigSeteable> _seteablesList;

        public ISpeechToTextHandler SpeechToText { get => _speechToText; }

        #region LifeCycle

        protected override void Awake()
        {
            base.Awake();
            InjectAssistantRepository();
            if (_speechToText != null)
            {
                _speechToText.OnFullTranscription += Input;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        private void InjectAssistantRepository()
        {
            _assistantRepositoryData = new AssistantRepositoryDataUtil();
            _seteablesList = GetComponentsInChildren<IAssistantConfigSeteable>().ToList();
            _seteablesList.ForEach(x => x.SetAssistantConfig(() => _assistantRepositoryData));
            Debug.Log("_seteablesList:" + _seteablesList.Count);
            if (string.IsNullOrEmpty(_assistantId))
            {
                Debug.LogError("Assistant ID is not set. Please set it in the Inspector.");
                return;
            }
            _assistantRepositoryData.AddDataToRepository(AssistanceVariables.ASSISTANT_ID_KEY, _assistantId);
        }

        #endregion

        #region API Input Handler
        public override void Input(string input)
        {
            _responsabilityAPIRequests.First().HandleRequest(input);
        }
        #endregion

        #region Debug Methods
        [Button]
        private void Activate()
        {
            _speechToText?.StartDictation();
        }
        [Button]
        private void Desactivate()
        {
            _speechToText?.StopDictation();
        }
        [Button]
        private void GetRepositoryValues()
        {
            foreach (var item in _assistantRepositoryData.RepositoryData)
            {
                Debug.Log(item.Key + " : " + item.Value);
            }
        }

        #endregion


    }
}