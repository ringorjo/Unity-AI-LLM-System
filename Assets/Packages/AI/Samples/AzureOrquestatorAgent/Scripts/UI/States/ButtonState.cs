using UnityEngine;
using UnityEngine.UI;

namespace Services.AI.AIProcedureHelp
{
    public abstract class ButtonState : ScriptableObject
    {
        public bool UseUpdate;
        protected Button _button;
        protected Image _icon;
        protected AudioSource _audioSource;

        public virtual void Init(Button button, Image icon, AudioSource audioSource)
        {
            _button = button;
            _icon = icon;
            _audioSource = audioSource;
        }
        public abstract void Update();
    }

}
