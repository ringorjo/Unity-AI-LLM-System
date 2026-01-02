using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Xennial.Services;

namespace Services.AI.AIProcedureHelp
{
    public class HelperAIEvents : SerializedMonoBehaviour
    {
        [SerializeField]
        private UnityEvent<bool> _onHelperAIActivationStatusChanged;
        private IRecieverMicDataCollaborator _micDataCollaborator;
        private void Start()
        {
            if (ServiceLocator.Instance.Exist<HelperAIRequest>())
            {
                _micDataCollaborator = ServiceLocator.Instance.Get<HelperAIRequest>();
                _micDataCollaborator.OnCollaboratorStateChanged += OnCollaboratorStatusChanged;
            }
        }

        private void OnCollaboratorStatusChanged(bool isEnabled)
        {
            _onHelperAIActivationStatusChanged?.Invoke(isEnabled);
        }

        private void OnDestroy()
        {
            if (_micDataCollaborator != null)
            {
                _micDataCollaborator.OnCollaboratorStateChanged -= OnCollaboratorStatusChanged;
            }
        }
    }
}

