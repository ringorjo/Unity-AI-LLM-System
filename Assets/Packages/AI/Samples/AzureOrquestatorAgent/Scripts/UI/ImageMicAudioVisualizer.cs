using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageMicAudioVisualizer : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private UnityEvent<Color> _color;
    [SerializeField]
    private float _offsetOscilation;
    [SerializeField]
    private Color _colorA;
    [SerializeField] private Color _colorB;
    [SerializeField]
    private float _speedAnimation;
    [SerializeField]
    private float _thresholdActivation;
    [SerializeField]
    private Vector2 _limitAnimation;
    [SerializeField, ReadOnly]
    private float _oscillationValue;
    private Color _defaultColor;

    private void Start()
    {
        _defaultColor = _image.color;
        enabled = false;
    }
    public void UpdateValue(float value)
    {
        enabled = value > _thresholdActivation ? true : false;
        float reScaledValue = value + _offsetOscilation;
        _oscillationValue = Mathf.Clamp((1 + (_limitAnimation.x * reScaledValue)), _limitAnimation.x, _limitAnimation.y);
        if (!enabled)
        {
            _image.transform.localScale = Vector3.one * _oscillationValue;
            _image.color = _defaultColor;
        }

    }

    private void Update()
    {
        _image.transform.localScale = Vector3.one * Mathf.Lerp(_image.transform.localScale.x, _oscillationValue, Time.deltaTime * _speedAnimation);
        _image.color = Color.Lerp(_colorA, _colorB, Mathf.PingPong(Time.time * _oscillationValue, 1));
      
    }
}


