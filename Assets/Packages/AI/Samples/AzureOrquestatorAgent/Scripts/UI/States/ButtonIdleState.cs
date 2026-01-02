using UnityEngine;
using UnityEngine.UI;

namespace Services.AI.AIProcedureHelp
{
    [CreateAssetMenu(fileName = "Button Idle State", menuName = "Xennial Digital/AI/UI/Button State/Button Idle State", order = 1)]

    public class ButtonIdleState : ButtonState
    {
        [SerializeField]
        private Sprite _iconSprite;
        [SerializeField]
        private Color _color;

        public override void Init(Button button, Image icon, AudioSource audioSource)
        {
            base.Init(button, icon, audioSource);
            _icon.sprite = _iconSprite;
            _icon.color = _color;
        }
        public override void Update() { }

    }

}
