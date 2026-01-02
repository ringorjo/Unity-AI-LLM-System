using UnityEngine;
using UnityEngine.UI;

namespace Services.AI.AIProcedureHelp
{
    [CreateAssetMenu(fileName = "Button Speak State", menuName = "Xennial Digital/AI/UI/Button State/Button Speak State", order = 1)]

    public class ButtonSpeakAIState : ButtonState
    {
        [SerializeField]
        private Sprite _iconSprite;
        [SerializeField]
        private Color _color;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private AudioClip _confirmationResponseClip;
        private Color _defaultColor;
        public override void Init(Button button, Image icon, AudioSource audioSource)
        {
            base.Init(button, icon, audioSource);
            _icon.color = _color;
            _defaultColor = _color;
            _icon.sprite = _iconSprite;
            _audioSource.PlayOneShot(_confirmationResponseClip);
        }
        public override void Update()
        {
            _defaultColor.a = Mathf.PingPong(Time.time * _speed, 1);
            _icon.color = _defaultColor;
        }
    }

}
