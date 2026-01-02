using UnityEngine;
using UnityEngine.UI;
using Xennial.Services;

namespace Services.AI
{
    public class MicUI : MonoBehaviour
    {
        [SerializeField]
        private Image _levelIcon;
        [SerializeField]
        private Image _buttonIcon;
        [SerializeField]
        private Color _disableMicColor;
        [SerializeField]
        private float _micInMin;
        [SerializeField]
        private float _micInMax;
        [SerializeField]
        private Button _micButton;
        private MicDataSenderMediator _mediator;
        private float _fillValue;
        private const float SPEED_INTERPOLATION = 10;
        private Color _defaultButtonColor;

        private void Start()
        {
            _micButton.onClick.AddListener(OnClic);
            _defaultButtonColor = _buttonIcon.color;
            _mediator = ServiceLocator.Instance.Get<MicDataSenderMediator>();
            _mediator.OnSamplesUpdated += OnMicSamplesUpdated;
            if (_buttonIcon != null)
                _buttonIcon.color = _mediator.IsMicEnable ? _defaultButtonColor : _disableMicColor;
            enabled = false;
        }
        private void Reset()
        {
            _micInMin = 0.02f;
            _micInMax = 0.15f;
            _disableMicColor = Color.gray;

        }
        private void Update()
        {
            _levelIcon.fillAmount = Mathf.Lerp(_levelIcon.fillAmount, _fillValue, SPEED_INTERPOLATION * Time.deltaTime);
        }

        private void OnMicSamplesUpdated(float samples)
        {
            if(samples==0)
                enabled = false;

            if (samples > _micInMin && !enabled)
            {
                enabled = true;
                if (_buttonIcon != null)
                    _buttonIcon.color = _defaultButtonColor;
            }
          
            _fillValue = MathFunctions.Remap(samples, _micInMin, _micInMax, 0, 1);
        }

        private void OnClic()
        {
            _levelIcon.fillAmount = 0;
            enabled = !_mediator.IsMicEnable;
            if (_buttonIcon != null)
                _buttonIcon.color = enabled ? _defaultButtonColor : _disableMicColor;
            _mediator?.ChangeMicUsage(!_mediator.IsMicEnable);
        }
    }
}

