using UnityEngine;
using UnityEngine.UI;

namespace Services.AI.AIProcedureHelp
{
    public class ImageOscilateAnimation : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private float _speed = 1.5f;
        [SerializeField]
        private Vector2 _alphaLimit;
        private Color _defaultColor;
        private Color _targetColor;
        private float _currentAlphaValue;

        private void Start()
        {
            _defaultColor = _image.color;
            _targetColor = _image.color;
            enabled = false;
        }
        public void ChangeActivation(bool isEnabled)
        {
            enabled = isEnabled;
            if (!isEnabled)
            {
                _image.color = _defaultColor;
            }
        }
        private void Update()
        {
            _currentAlphaValue = Mathf.PingPong(Time.time * _speed, _alphaLimit.y);
            _targetColor.a = Mathf.Clamp(_currentAlphaValue, _alphaLimit.x, _alphaLimit.y);
            _image.color = _targetColor;
        }
    }

}
