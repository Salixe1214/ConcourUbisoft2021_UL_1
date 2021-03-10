using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace DefaultNamespace
{
    public class ToggleButton:SimpleButton
    {
        [SerializeField] private UnityEvent toggledOnActions;
        [SerializeField] private UnityEvent toggledOffActions;
        private bool toggled = false;
        private void Update()
        {
            base.Update();
            if (toggled)
            {
                _animator.SetTrigger("click");
                Actions?.Invoke();
            }
        }

        protected override void PressButon()
        {
            base.PressButon();
            toggled = !toggled;
            if (toggled) toggledOnActions?.Invoke();
            else toggledOffActions?.Invoke();
        }
    }
    
}