using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xennial.Services;

namespace Services.AI.AIProcedureHelp
{
    public class AIHelperUIButton : SerializedMonoBehaviour
    {
        [SerializeField]
        private HelperButtonState _currentState;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private Dictionary<HelperButtonState, ButtonState> _states;
        private IRecieverMicDataCollaborator _micDataCollaborator;
        private ButtonState _currentBtnState;
        private EventBusService _busService;


        private void Start()
        {
            if (ServiceLocator.Instance.Exist<HelperAIRequest>())
            {
                _micDataCollaborator = ServiceLocator.Instance.Get<HelperAIRequest>();
                _micDataCollaborator.OnCollaboratorStateChanged += OnCollaboratorStatusChanged;
            }
            _button.onClick.AddListener(OnHelperAI);

            _busService = ServiceLocator.Instance.Get<EventBusService>();
            _currentState = HelperButtonState.Idle;
            _currentBtnState = _states[_currentState];
            PerformState();
            _busService.Subscribe(nameof(AIEventEnums.OnSpeechEnd), ResetState);
            enabled = _currentBtnState.UseUpdate;

        }
        private void OnDestroy()
        {
            if (_micDataCollaborator != null)
                _micDataCollaborator.OnCollaboratorStateChanged -= OnCollaboratorStatusChanged;

            if (_busService != null)
                _busService.Unsubscribe(nameof(AIEventEnums.OnSpeechEnd), ResetState);

            _button.onClick.RemoveListener(OnHelperAI);
        }

        private void OnHelperAI()
        {
            if (ServiceLocator.Instance.Exist<HelperAIRequest>())
                ServiceLocator.Instance.Get<HelperAIRequest>().Input();
        }

        private void ResetState()
        {
            _currentState = HelperButtonState.Idle;
            PerformState();
        }

        private void OnCollaboratorStatusChanged(bool isRunning)
        {
            _currentState = isRunning ? HelperButtonState.Processing : HelperButtonState.Speaking;
            PerformState();

        }

        private void PerformState()
        {
            if (_states.TryGetValue(_currentState, out _currentBtnState))
            {
                enabled = _currentBtnState.UseUpdate;
                _currentBtnState.Init(_button, _icon, _audioSource);
            }
        }

        private void Update()
        {
            _currentBtnState?.Update();
        }
    }

}
