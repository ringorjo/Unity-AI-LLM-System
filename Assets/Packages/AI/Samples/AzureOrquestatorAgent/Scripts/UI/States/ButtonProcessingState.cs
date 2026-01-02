using UnityEngine;
using UnityEngine.UI;

namespace Services.AI.AIProcedureHelp
{
    [CreateAssetMenu(fileName = "Button Processing State", menuName = "Xennial Digital/AI/UI/Button State/Button Processing State", order = 1)]

    public class ButtonProcessingState : ButtonState
    {
        [SerializeField]
        private Sprite _iconSprite;
        [SerializeField]
        private Color _colorA;
        [SerializeField]
        private Color _colorB;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private AudioClip _enableclip;

        public override void Init(Button button, Image icon, AudioSource audioSource)
        {
            base.Init(button, icon, audioSource);
            _icon.sprite = _iconSprite;
            _audioSource.PlayOneShot(_enableclip);
        }

        public override void Update()
        {
            float t = Mathf.PingPong(Time.time * _speed, 1);
            _icon.color = Color.Lerp(_colorA, _colorB, t);
        }
    }

}
